namespace SupportOpsAI.Application.DTOs.Triage;

public record RetryTriageResponse(Guid TriageJobId, Guid TicketId, string Status, string CorrelationId);
