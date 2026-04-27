using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Data;

namespace SupportOpsAI.Infrastructure.Services;

public class AuditLogService(SupportOpsDbContext dbContext) : IAuditLogService
{
    public async Task RecordAsync(
        AuditEventType eventType,
        string message,
        Guid? userId = null,
        Guid? ticketId = null,
        CancellationToken cancellationToken = default)
    {
        dbContext.AuditLogs.Add(new AuditLog
        {
            EventType = eventType,
            Message = message,
            UserId = userId,
            TicketId = ticketId
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
