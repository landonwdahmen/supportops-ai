using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SupportOpsAI.Application.DTOs.Triage;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Infrastructure.AI;

public class OpenAiTriageService(HttpClient httpClient, IOptions<OpenAiOptions> options) : IAiTriageService
{
    public async Task<AiTriageResult> AnalyzeAsync(AiTriageRequest request, CancellationToken cancellationToken = default)
    {
        var settings = options.Value;
        if (string.IsNullOrWhiteSpace(settings.ApiKey))
        {
            throw new InvalidOperationException("OpenAI API key is not configured.");
        }

        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", settings.ApiKey);
        httpRequest.Content = JsonContent(settings.Model, request);

        using var response = await httpClient.SendAsync(httpRequest, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
        var content = document.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new InvalidOperationException("OpenAI returned an empty triage response.");
        }

        return ParseResult(content);
    }

    private static HttpContent JsonContent(string model, AiTriageRequest request)
    {
        var payload = new
        {
            model,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new
                {
                    role = "system",
                    content = "Return strict JSON with suggestedCategory, suggestedPriority, confidenceScore, reasoningSummary, suggestedSteps. Valid categories: General, Billing, Technical, Account, Bug, FeatureRequest. Valid priorities: Low, Medium, High, Urgent."
                },
                new
                {
                    role = "user",
                    content = $"Title: {request.Title}\nDescription: {request.Description}"
                }
            }
        };

        return new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
    }

    private static AiTriageResult ParseResult(string content)
    {
        using var document = JsonDocument.Parse(content);
        var root = document.RootElement;

        var categoryText = root.GetProperty("suggestedCategory").GetString();
        var priorityText = root.GetProperty("suggestedPriority").GetString();
        var confidenceScore = root.GetProperty("confidenceScore").GetDouble();
        var reasoningSummary = root.GetProperty("reasoningSummary").GetString();
        var suggestedSteps = root.GetProperty("suggestedSteps").GetString();

        if (!Enum.TryParse<TicketCategory>(categoryText, ignoreCase: true, out var category) ||
            !Enum.TryParse<TicketPriority>(priorityText, ignoreCase: true, out var priority) ||
            confidenceScore is < 0 or > 1 ||
            string.IsNullOrWhiteSpace(reasoningSummary) ||
            string.IsNullOrWhiteSpace(suggestedSteps))
        {
            throw new InvalidOperationException("OpenAI triage response was invalid.");
        }

        return new AiTriageResult(category, priority, confidenceScore, reasoningSummary, suggestedSteps);
    }
}
