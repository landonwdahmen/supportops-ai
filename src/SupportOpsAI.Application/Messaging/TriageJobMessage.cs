namespace SupportOpsAI.Application.Messaging;

public record TriageJobMessage(
    int Version,
    Guid TriageJobId,
    Guid TicketId,
    DateTimeOffset CreatedAt,
    int Attempt,
    string CorrelationId)
{
    public static TriageJobMessage Create(Guid triageJobId, Guid ticketId, int attempt, string correlationId)
    {
        return new TriageJobMessage(1, triageJobId, ticketId, DateTimeOffset.UtcNow, attempt, correlationId);
    }
}
