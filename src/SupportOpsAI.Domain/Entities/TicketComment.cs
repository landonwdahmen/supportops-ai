namespace SupportOpsAI.Domain.Entities;

public class TicketComment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public Guid AuthorUserId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Ticket? Ticket { get; set; }
    public User? AuthorUser { get; set; }
}
