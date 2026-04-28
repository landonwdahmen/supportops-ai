using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Application.DTOs.Triage;

public record TriageResultResponse(
    Guid Id,
    Guid TicketId,
    TicketCategory SuggestedCategory,
    TicketPriority SuggestedPriority,
    double ConfidenceScore,
    string ReasoningSummary,
    string SuggestedSteps,
    TriageReviewStatus ReviewStatus,
    Guid? ReviewedByUserId,
    string? ReviewNotes,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ReviewedAt);
