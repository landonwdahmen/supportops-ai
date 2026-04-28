using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using SupportOpsAI.Application.DTOs.Tickets;
using SupportOpsAI.Application.Exceptions;
using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Data;
using SupportOpsAI.Infrastructure.Services;
using SupportOpsAI.IntegrationTests.Fakes;

namespace SupportOpsAI.IntegrationTests;

public class TicketServiceTests
{
    [Fact]
    public async Task CreateAsync_saves_ticket_with_pending_triage_status()
    {
        await using var dbContext = CreateDbContext();
        var user = await AddUserAsync(dbContext, "customer@example.com");
        var currentUser = new FakeCurrentUserService { UserId = user.Id, Email = user.Email, Role = user.Role };
        var publisher = new FakeTriageQueuePublisher();
        var service = CreateService(dbContext, currentUser, publisher);

        var response = await service.CreateAsync(new CreateTicketRequest(
            "Cannot access billing page",
            "The billing page returns an error after login.",
            TicketPriority.High,
            TicketCategory.Billing));

        Assert.Equal(TicketStatus.PendingTriage, response.Status);
        Assert.Equal(user.Id, response.CreatedByUserId);
        Assert.Equal(1, await dbContext.Tickets.CountAsync());
        Assert.Equal(1, await dbContext.TriageJobs.CountAsync());
        Assert.Single(publisher.PublishedMessages);
    }

    [Fact]
    public async Task GetByIdAsync_returns_ticket_for_creator()
    {
        await using var dbContext = CreateDbContext();
        var user = await AddUserAsync(dbContext, "customer@example.com");
        var currentUser = new FakeCurrentUserService { UserId = user.Id, Email = user.Email, Role = user.Role };
        var service = CreateService(dbContext, currentUser);
        var created = await service.CreateAsync(new CreateTicketRequest("Login issue", "I cannot log in."));

        var ticket = await service.GetByIdAsync(created.Id);

        Assert.Equal(created.Id, ticket.Id);
        Assert.Equal("Login issue", ticket.Title);
    }

    [Fact]
    public async Task GetByIdAsync_throws_forbidden_for_unrelated_customer()
    {
        await using var dbContext = CreateDbContext();
        var owner = await AddUserAsync(dbContext, "owner@example.com");
        var other = await AddUserAsync(dbContext, "other@example.com");
        dbContext.Tickets.Add(new Ticket
        {
            Title = "Private ticket",
            Description = "Only the owner should see this.",
            CreatedByUserId = owner.Id
        });
        await dbContext.SaveChangesAsync();

        var ticketId = await dbContext.Tickets.Select(x => x.Id).SingleAsync();
        var currentUser = new FakeCurrentUserService { UserId = other.Id, Email = other.Email, Role = other.Role };
        var service = CreateService(dbContext, currentUser);

        await Assert.ThrowsAsync<ForbiddenException>(() => service.GetByIdAsync(ticketId));
    }

    private static SupportOpsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SupportOpsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SupportOpsDbContext(options);
    }

    private static async Task<User> AddUserAsync(SupportOpsDbContext dbContext, string email, UserRole role = UserRole.Customer)
    {
        var user = new User
        {
            Email = email,
            DisplayName = email,
            PasswordHash = "not-used-in-ticket-tests",
            Role = role
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        return user;
    }

    private static TicketService CreateService(
        SupportOpsDbContext dbContext,
        FakeCurrentUserService currentUser,
        FakeTriageQueuePublisher? publisher = null)
    {
        return new TicketService(
            dbContext,
            currentUser,
            new AuditLogService(dbContext),
            publisher ?? new FakeTriageQueuePublisher(),
            NullLogger<TicketService>.Instance);
    }
}
