namespace SupportOpsAI.Domain.Enums;

public enum AuditEventType
{
    UserRegistered = 0,
    UserLoggedIn = 1,
    TicketCreated = 2,
    TicketViewed = 3,
    TicketCommentAdded = 4,
    TicketAssigned = 5,
    TicketStatusChanged = 6,
    TriageRequested = 7,
    TriageJobQueued = 8,
    TriageJobFailed = 9,
    TriageCompleted = 10,
    TriageApproved = 11,
    TriageEdited = 12,
    TriageRejected = 13,
    TriageRetried = 14
}
