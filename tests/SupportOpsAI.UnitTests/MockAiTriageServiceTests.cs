using SupportOpsAI.Application.DTOs.Triage;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.AI;

namespace SupportOpsAI.UnitTests;

public class MockAiTriageServiceTests
{
    [Fact]
    public async Task AnalyzeAsync_returns_valid_structured_result()
    {
        var service = new MockAiTriageService();

        var result = await service.AnalyzeAsync(new AiTriageRequest(
            Guid.NewGuid(),
            "Billing page error",
            "Customer sees an error while paying an invoice."));

        Assert.True(Enum.IsDefined(result.SuggestedCategory));
        Assert.True(Enum.IsDefined(result.SuggestedPriority));
        Assert.InRange(result.ConfidenceScore, 0, 1);
        Assert.False(string.IsNullOrWhiteSpace(result.ReasoningSummary));
        Assert.False(string.IsNullOrWhiteSpace(result.SuggestedSteps));
        Assert.Equal(TicketCategory.Billing, result.SuggestedCategory);
    }
}
