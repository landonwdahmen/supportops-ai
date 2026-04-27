using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Application.DTOs.Tickets;

public record TicketDetailResponse(
    Guid Id,
    string Title,
    string Description,
    TicketStatus Status,
    TicketPriority Priority,
    TicketCategory Category,
    Guid CreatedByUserId,
    string? CreatedByDisplayName,
    Guid? AssignedToUserId,
    string? AssignedToDisplayName,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    IReadOnlyCollection<TicketCommentResponse> Comments);
