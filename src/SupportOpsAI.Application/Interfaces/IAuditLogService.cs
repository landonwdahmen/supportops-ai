using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Application.Interfaces;

public interface IAuditLogService
{
    Task RecordAsync(
        AuditEventType eventType,
        string message,
        Guid? userId = null,
        Guid? ticketId = null,
        CancellationToken cancellationToken = default);
}
