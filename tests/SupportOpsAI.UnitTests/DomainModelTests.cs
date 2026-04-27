using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.UnitTests;

public class DomainModelTests
{
    [Fact]
    public void Ticket_defaults_to_pending_triage_with_medium_priority()
    {
        var ticket = new Ticket();

        Assert.Equal(TicketStatus.PendingTriage, ticket.Status);
        Assert.Equal(TicketPriority.Medium, ticket.Priority);
        Assert.Equal(TicketCategory.General, ticket.Category);
    }

    [Fact]
    public void User_defaults_to_customer_role()
    {
        var user = new User();

        Assert.Equal(UserRole.Customer, user.Role);
    }
}
