using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Application.DTOs.Tickets;

public record CreateTicketRequest(
    string Title,
    string Description,
    TicketPriority Priority = TicketPriority.Medium,
    TicketCategory Category = TicketCategory.General);
