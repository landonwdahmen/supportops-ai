namespace SupportOpsAI.Application.DTOs.Triage;

public record AiTriageRequest(Guid TicketId, string Title, string Description);
