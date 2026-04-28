namespace SupportOpsAI.Domain.Enums;

public enum TicketStatus
{
    New = 0,
    PendingTriage = 1,
    Open = 2,
    InProgress = 3,
    TriageCompleted = 4,
    Resolved = 5,
    Closed = 6
}
