using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Domain.Entities;

public class Ticket
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TicketStatus Status { get; set; } = TicketStatus.PendingTriage;
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public TicketCategory Category { get; set; } = TicketCategory.General;
    public Guid CreatedByUserId { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAt { get; set; }

    public User? CreatedByUser { get; set; }
    public User? AssignedToUser { get; set; }
    public ICollection<TicketComment> Comments { get; set; } = new List<TicketComment>();
    public ICollection<TicketTriageResult> TriageResults { get; set; } = new List<TicketTriageResult>();
    public ICollection<TriageJob> TriageJobs { get; set; } = new List<TriageJob>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
