using SupportOpsAI.Application.DTOs.Triage;

namespace SupportOpsAI.Application.Interfaces;

public interface IAiTriageService
{
    Task<AiTriageResult> AnalyzeAsync(AiTriageRequest request, CancellationToken cancellationToken = default);
}
