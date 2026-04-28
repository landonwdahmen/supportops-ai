using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Application.DTOs.Triage;

public record EditTriageRequest(
    TicketCategory Category,
    TicketPriority Priority,
    string? Notes);
