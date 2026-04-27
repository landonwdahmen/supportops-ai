namespace SupportOpsAI.Application.DTOs.Tickets;

public record TicketCommentResponse(
    Guid Id,
    Guid TicketId,
    Guid AuthorUserId,
    string? AuthorDisplayName,
    string Body,
    DateTimeOffset CreatedAt);
