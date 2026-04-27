using SupportOpsAI.Application.DTOs.Tickets;

namespace SupportOpsAI.Application.Interfaces;

public interface ITicketService
{
    Task<TicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TicketResponse>> GetTicketsAsync(CancellationToken cancellationToken = default);
    Task<TicketDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TicketCommentResponse> AddCommentAsync(Guid ticketId, TicketCommentRequest request, CancellationToken cancellationToken = default);
}
