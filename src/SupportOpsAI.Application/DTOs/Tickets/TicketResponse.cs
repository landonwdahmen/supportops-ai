using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Application.DTOs.Tickets;

public record TicketResponse(
    Guid Id,
    string Title,
    TicketStatus Status,
    TicketPriority Priority,
    TicketCategory Category,
    Guid CreatedByUserId,
    Guid? AssignedToUserId,
    DateTimeOffset CreatedAt);
