using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Application.DTOs.Triage;

public record AiTriageResult(
    TicketCategory SuggestedCategory,
    TicketPriority SuggestedPriority,
    double ConfidenceScore,
    string ReasoningSummary,
    string SuggestedSteps);
