using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public AuditEventType EventType { get; set; }
    public Guid? UserId { get; set; }
    public Guid? TicketId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public User? User { get; set; }
    public Ticket? Ticket { get; set; }
}
