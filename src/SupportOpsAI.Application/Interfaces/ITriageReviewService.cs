using SupportOpsAI.Application.DTOs.Triage;

namespace SupportOpsAI.Application.Interfaces;

public interface ITriageReviewService
{
    Task<TriageResultResponse> GetLatestAsync(Guid ticketId, CancellationToken cancellationToken = default);
    Task<TriageResultResponse> ApproveAsync(Guid ticketId, ApproveTriageRequest request, CancellationToken cancellationToken = default);
    Task<TriageResultResponse> EditAsync(Guid ticketId, EditTriageRequest request, CancellationToken cancellationToken = default);
    Task<TriageResultResponse> RejectAsync(Guid ticketId, RejectTriageRequest request, CancellationToken cancellationToken = default);
    Task<RetryTriageResponse> RetryAsync(Guid ticketId, CancellationToken cancellationToken = default);
}
