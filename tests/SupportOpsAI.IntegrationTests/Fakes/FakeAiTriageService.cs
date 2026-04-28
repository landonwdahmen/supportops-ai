using SupportOpsAI.Application.DTOs.Triage;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.IntegrationTests.Fakes;

public class FakeAiTriageService : IAiTriageService
{
    public bool ThrowOnAnalyze { get; set; }

    public Task<AiTriageResult> AnalyzeAsync(AiTriageRequest request, CancellationToken cancellationToken = default)
    {
        if (ThrowOnAnalyze)
        {
            throw new InvalidOperationException("AI failed.");
        }

        return Task.FromResult(new AiTriageResult(
            TicketCategory.Technical,
            TicketPriority.High,
            0.9,
            "Fake AI triage result.",
            "Collect logs and route to technical support."));
    }
}
