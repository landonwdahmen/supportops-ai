using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SupportOpsAI.Application.DTOs.Triage;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Application.Messaging;
using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Data;

namespace SupportOpsAI.Infrastructure.Services;

public class TriageJobProcessor(
    SupportOpsDbContext dbContext,
    IAiTriageService aiTriageService,
    ITriageQueuePublisher triageQueuePublisher,
    IAuditLogService auditLogService,
    ILogger<TriageJobProcessor> logger) : ITriageJobProcessor
{
    private const int MaxAttempts = 3;

    public async Task ProcessAsync(TriageJobMessage message, CancellationToken cancellationToken = default)
    {
        var job = await dbContext.TriageJobs
            .Include(x => x.Ticket)
            .SingleOrDefaultAsync(x => x.Id == message.TriageJobId, cancellationToken);

        if (job?.Ticket is null)
        {
            logger.LogWarning("Triage job {TriageJobId} could not be processed because it was not found.", message.TriageJobId);
            return;
        }

        job.Status = TriageJobStatus.Processing;
        job.AttemptCount = Math.Max(job.AttemptCount + 1, message.Attempt);
        job.StartedAt = DateTimeOffset.UtcNow;
        job.LastError = null;
        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            var aiResult = await aiTriageService.AnalyzeAsync(
                new AiTriageRequest(job.Ticket.Id, job.Ticket.Title, job.Ticket.Description),
                cancellationToken);

            dbContext.TicketTriageResults.Add(new TicketTriageResult
            {
                TicketId = job.TicketId,
                SuggestedCategory = aiResult.SuggestedCategory,
                SuggestedPriority = aiResult.SuggestedPriority,
                ConfidenceScore = aiResult.ConfidenceScore,
                Summary = aiResult.ReasoningSummary,
                Reasoning = aiResult.ReasoningSummary,
                SuggestedSteps = aiResult.SuggestedSteps,
                ReviewStatus = TriageReviewStatus.PendingReview
            });

            job.Status = TriageJobStatus.Completed;
            job.CompletedAt = DateTimeOffset.UtcNow;
            job.Ticket.Status = TicketStatus.TriageCompleted;
            job.Ticket.UpdatedAt = DateTimeOffset.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
            await auditLogService.RecordAsync(AuditEventType.TriageCompleted, "AI triage completed.", ticketId: job.TicketId, cancellationToken: cancellationToken);
        }
        catch (Exception exception)
        {
            job.LastError = exception.Message;
            job.ErrorMessage = exception.Message;
            job.Status = job.AttemptCount >= MaxAttempts ? TriageJobStatus.Failed : TriageJobStatus.Queued;
            await dbContext.SaveChangesAsync(cancellationToken);
            await auditLogService.RecordAsync(AuditEventType.TriageJobFailed, "AI triage failed.", ticketId: job.TicketId, cancellationToken: cancellationToken);

            logger.LogWarning(exception, "Triage job {TriageJobId} failed on attempt {AttemptCount}.", job.Id, job.AttemptCount);

            if (job.Status == TriageJobStatus.Queued)
            {
                await triageQueuePublisher.PublishAsync(
                    TriageJobMessage.Create(job.Id, job.TicketId, job.AttemptCount + 1, job.CorrelationId),
                    cancellationToken);
            }
        }
    }
}
