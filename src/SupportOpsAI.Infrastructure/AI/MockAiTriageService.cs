using SupportOpsAI.Application.DTOs.Triage;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Infrastructure.AI;

public class MockAiTriageService : IAiTriageService
{
    public Task<AiTriageResult> AnalyzeAsync(AiTriageRequest request, CancellationToken cancellationToken = default)
    {
        var text = $"{request.Title} {request.Description}".ToLowerInvariant();
        var category = text.Contains("bill") || text.Contains("invoice") || text.Contains("payment")
            ? TicketCategory.Billing
            : text.Contains("bug") || text.Contains("error") || text.Contains("crash")
                ? TicketCategory.Bug
                : text.Contains("login") || text.Contains("password") || text.Contains("account")
                    ? TicketCategory.Account
                    : TicketCategory.Technical;

        var priority = text.Contains("urgent") || text.Contains("down") || text.Contains("production")
            ? TicketPriority.Urgent
            : text.Contains("blocked") || text.Contains("error")
                ? TicketPriority.High
                : TicketPriority.Medium;

        var result = new AiTriageResult(
            category,
            priority,
            0.82,
            "Mock triage classified the ticket from keyword signals in the title and description.",
            "Confirm recent account or system changes, gather reproduction details, and route to the appropriate support queue.");

        return Task.FromResult(result);
    }
}
