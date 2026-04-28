using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Domain.Entities;

public class TriageJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");
    public TriageJobStatus Status { get; set; } = TriageJobStatus.Queued;
    public int AttemptCount { get; set; }
    public string? ErrorMessage { get; set; }
    public string? LastError { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public Ticket? Ticket { get; set; }
}
