using Microsoft.EntityFrameworkCore;
using SupportOpsAI.Application.DTOs.Triage;
using SupportOpsAI.Application.Exceptions;
using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Data;
using SupportOpsAI.Infrastructure.Services;
using SupportOpsAI.IntegrationTests.Fakes;

namespace SupportOpsAI.IntegrationTests;

public class TriageReviewServiceTests
{
    [Fact]
    public async Task ApproveAsync_applies_suggested_category_and_priority()
    {
        await using var dbContext = CreateDbContext();
        var (ticket, agent) = await SeedReviewedTicketAsync(dbContext);
        var service = CreateService(dbContext, agent, UserRole.Agent);

        var result = await service.ApproveAsync(ticket.Id, new ApproveTriageRequest("Looks right."));

        var savedTicket = await dbContext.Tickets.SingleAsync();
        Assert.Equal(TriageReviewStatus.Accepted, result.ReviewStatus);
        Assert.Equal(TicketCategory.Technical, savedTicket.Category);
        Assert.Equal(TicketPriority.High, savedTicket.Priority);
    }

    [Fact]
    public async Task EditAsync_overrides_category_and_priority()
    {
        await using var dbContext = CreateDbContext();
        var (ticket, admin) = await SeedReviewedTicketAsync(dbContext, UserRole.Admin);
        var service = CreateService(dbContext, admin, UserRole.Admin);

        var result = await service.EditAsync(ticket.Id, new EditTriageRequest(TicketCategory.Bug, TicketPriority.Urgent, "Escalate."));

        Assert.Equal(TriageReviewStatus.Revised, result.ReviewStatus);
        Assert.Equal(TicketCategory.Bug, result.SuggestedCategory);
        Assert.Equal(TicketPriority.Urgent, result.SuggestedPriority);
    }

    [Fact]
    public async Task RejectAsync_marks_result_rejected()
    {
        await using var dbContext = CreateDbContext();
        var (ticket, agent) = await SeedReviewedTicketAsync(dbContext);
        var service = CreateService(dbContext, agent, UserRole.Agent);

        var result = await service.RejectAsync(ticket.Id, new RejectTriageRequest("Wrong queue."));

        Assert.Equal(TriageReviewStatus.Rejected, result.ReviewStatus);
        Assert.Equal("Wrong queue.", result.ReviewNotes);
    }

    [Fact]
    public async Task Customer_cannot_approve_edit_or_reject()
    {
        await using var dbContext = CreateDbContext();
        var (ticket, customer) = await SeedReviewedTicketAsync(dbContext);
        var service = CreateService(dbContext, customer, UserRole.Customer);

        await Assert.ThrowsAsync<ForbiddenException>(() => service.ApproveAsync(ticket.Id, new ApproveTriageRequest(null)));
        await Assert.ThrowsAsync<ForbiddenException>(() => service.EditAsync(ticket.Id, new EditTriageRequest(TicketCategory.Bug, TicketPriority.High, null)));
        await Assert.ThrowsAsync<ForbiddenException>(() => service.RejectAsync(ticket.Id, new RejectTriageRequest("No")));
    }

    private static SupportOpsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SupportOpsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SupportOpsDbContext(options);
    }

    private static async Task<(Ticket Ticket, User User)> SeedReviewedTicketAsync(SupportOpsDbContext dbContext, UserRole role = UserRole.Agent)
    {
        var user = new User
        {
            Email = "reviewer@example.com",
            DisplayName = "Reviewer",
            PasswordHash = "not-used",
            Role = role
        };
        var ticket = new Ticket
        {
            Title = "Login error",
            Description = "Login returns an error.",
            CreatedByUserId = user.Id
        };
        var result = new TicketTriageResult
        {
            TicketId = ticket.Id,
            SuggestedCategory = TicketCategory.Technical,
            SuggestedPriority = TicketPriority.High,
            ConfidenceScore = 0.9,
            Summary = "Technical login issue.",
            Reasoning = "Technical login issue.",
            SuggestedSteps = "Collect logs."
        };

        dbContext.Users.Add(user);
        dbContext.Tickets.Add(ticket);
        dbContext.TicketTriageResults.Add(result);
        await dbContext.SaveChangesAsync();

        return (ticket, user);
    }

    private static TriageReviewService CreateService(SupportOpsDbContext dbContext, User user, UserRole role)
    {
        var currentUser = new FakeCurrentUserService { UserId = user.Id, Email = user.Email, Role = role };
        return new TriageReviewService(dbContext, currentUser, new FakeTriageQueuePublisher(), new AuditLogService(dbContext));
    }
}
