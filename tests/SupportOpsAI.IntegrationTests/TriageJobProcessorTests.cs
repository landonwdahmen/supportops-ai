using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SupportOpsAI.Application.Messaging;
using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Data;
using SupportOpsAI.Infrastructure.Services;
using SupportOpsAI.IntegrationTests.Fakes;

namespace SupportOpsAI.IntegrationTests;

public class TriageJobProcessorTests
{
    [Fact]
    public async Task ProcessAsync_saves_result_and_marks_job_completed()
    {
        await using var dbContext = CreateDbContext();
        var (ticket, job) = await SeedTicketAndJobAsync(dbContext);
        var processor = CreateProcessor(dbContext);

        await processor.ProcessAsync(TriageJobMessage.Create(job.Id, ticket.Id, 1, job.CorrelationId));

        var savedJob = await dbContext.TriageJobs.SingleAsync();
        var savedTicket = await dbContext.Tickets.SingleAsync();
        Assert.Equal(TriageJobStatus.Completed, savedJob.Status);
        Assert.Equal(TicketStatus.TriageCompleted, savedTicket.Status);
        Assert.Equal(1, await dbContext.TicketTriageResults.CountAsync());
    }

    [Fact]
    public async Task ProcessAsync_marks_job_failed_after_max_attempts()
    {
        await using var dbContext = CreateDbContext();
        var (ticket, job) = await SeedTicketAndJobAsync(dbContext);
        job.AttemptCount = 2;
        await dbContext.SaveChangesAsync();
        var processor = CreateProcessor(dbContext, aiService: new FakeAiTriageService { ThrowOnAnalyze = true });

        await processor.ProcessAsync(TriageJobMessage.Create(job.Id, ticket.Id, 3, job.CorrelationId));

        var savedJob = await dbContext.TriageJobs.SingleAsync();
        Assert.Equal(TriageJobStatus.Failed, savedJob.Status);
        Assert.False(string.IsNullOrWhiteSpace(savedJob.LastError));
    }

    private static SupportOpsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SupportOpsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SupportOpsDbContext(options);
    }

    private static async Task<(Ticket Ticket, TriageJob Job)> SeedTicketAndJobAsync(SupportOpsDbContext dbContext)
    {
        var user = new User
        {
            Email = "customer@example.com",
            DisplayName = "Customer",
            PasswordHash = "not-used"
        };
        var ticket = new Ticket
        {
            Title = "Production error",
            Description = "The production login flow is down.",
            CreatedByUserId = user.Id
        };
        var job = new TriageJob
        {
            TicketId = ticket.Id,
            CorrelationId = Guid.NewGuid().ToString("N")
        };

        dbContext.Users.Add(user);
        dbContext.Tickets.Add(ticket);
        dbContext.TriageJobs.Add(job);
        await dbContext.SaveChangesAsync();

        return (ticket, job);
    }

    private static TriageJobProcessor CreateProcessor(SupportOpsDbContext dbContext, FakeAiTriageService? aiService = null)
    {
        return new TriageJobProcessor(
            dbContext,
            aiService ?? new FakeAiTriageService(),
            new FakeTriageQueuePublisher(),
            new AuditLogService(dbContext),
            NullLogger<TriageJobProcessor>.Instance);
    }
}
