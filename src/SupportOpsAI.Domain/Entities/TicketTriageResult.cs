using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Domain.Entities;

public class TicketTriageResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TicketId { get; set; }
    public TicketPriority SuggestedPriority { get; set; } = TicketPriority.Medium;
    public TicketCategory SuggestedCategory { get; set; } = TicketCategory.General;
    public string Summary { get; set; } = string.Empty;
    public string Reasoning { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public TriageReviewStatus ReviewStatus { get; set; } = TriageReviewStatus.PendingReview;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Ticket? Ticket { get; set; }
}
