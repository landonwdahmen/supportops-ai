using Microsoft.EntityFrameworkCore;
using SupportOpsAI.Application.DTOs.Triage;
using SupportOpsAI.Application.Exceptions;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Application.Messaging;
using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Data;

namespace SupportOpsAI.Infrastructure.Services;

public class TriageReviewService(
    SupportOpsDbContext dbContext,
    ICurrentUserService currentUserService,
    ITriageQueuePublisher triageQueuePublisher,
    IAuditLogService auditLogService) : ITriageReviewService
{
    public async Task<TriageResultResponse> GetLatestAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        var ticket = await LoadTicketAsync(ticketId, cancellationToken);
        EnsureCanView(ticket);

        var result = await LatestResultQuery(ticketId).FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("No triage result was found for this ticket.");

        return Map(result);
    }

    public async Task<TriageResultResponse> ApproveAsync(Guid ticketId, ApproveTriageRequest request, CancellationToken cancellationToken = default)
    {
        EnsureAgentOrAdmin();
        var ticket = await LoadTicketAsync(ticketId, cancellationToken);
        var result = await LatestResultQuery(ticketId).FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("No triage result was found for this ticket.");

        ticket.Category = result.SuggestedCategory;
        ticket.Priority = result.SuggestedPriority;
        ticket.UpdatedAt = DateTimeOffset.UtcNow;
        MarkReviewed(result, TriageReviewStatus.Accepted, request.Notes);

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.RecordAsync(AuditEventType.TriageApproved, "Triage recommendation approved.", currentUserService.UserId, ticketId, cancellationToken);

        return Map(result);
    }

    public async Task<TriageResultResponse> EditAsync(Guid ticketId, EditTriageRequest request, CancellationToken cancellationToken = default)
    {
        EnsureAgentOrAdmin();
        var ticket = await LoadTicketAsync(ticketId, cancellationToken);
        var result = await LatestResultQuery(ticketId).FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("No triage result was found for this ticket.");

        ticket.Category = request.Category;
        ticket.Priority = request.Priority;
        ticket.UpdatedAt = DateTimeOffset.UtcNow;
        result.SuggestedCategory = request.Category;
        result.SuggestedPriority = request.Priority;
        MarkReviewed(result, TriageReviewStatus.Revised, request.Notes);

        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.RecordAsync(AuditEventType.TriageEdited, "Triage recommendation edited.", currentUserService.UserId, ticketId, cancellationToken);

        return Map(result);
    }

    public async Task<TriageResultResponse> RejectAsync(Guid ticketId, RejectTriageRequest request, CancellationToken cancellationToken = default)
    {
        EnsureAgentOrAdmin();
        var result = await LatestResultQuery(ticketId).FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("No triage result was found for this ticket.");

        MarkReviewed(result, TriageReviewStatus.Rejected, request.Reason);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.RecordAsync(AuditEventType.TriageRejected, "Triage recommendation rejected.", currentUserService.UserId, ticketId, cancellationToken);

        return Map(result);
    }

    public async Task<RetryTriageResponse> RetryAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        EnsureAgentOrAdmin();
        var ticket = await LoadTicketAsync(ticketId, cancellationToken);
        if (ticket.TriageJobs.Any(x => x.Status is TriageJobStatus.Queued or TriageJobStatus.Processing))
        {
            throw new ConflictException("A triage job is already queued or processing for this ticket.");
        }

        var job = new TriageJob
        {
            TicketId = ticketId,
            Status = TriageJobStatus.Queued,
            CorrelationId = Guid.NewGuid().ToString("N")
        };

        ticket.Status = TicketStatus.PendingTriage;
        ticket.UpdatedAt = DateTimeOffset.UtcNow;
        dbContext.TriageJobs.Add(job);
        await dbContext.SaveChangesAsync(cancellationToken);

        await triageQueuePublisher.PublishAsync(TriageJobMessage.Create(job.Id, ticketId, 1, job.CorrelationId), cancellationToken);
        await auditLogService.RecordAsync(AuditEventType.TriageRetried, "Triage retry queued.", currentUserService.UserId, ticketId, cancellationToken);

        return new RetryTriageResponse(job.Id, ticketId, job.Status.ToString(), job.CorrelationId);
    }

    private async Task<Ticket> LoadTicketAsync(Guid ticketId, CancellationToken cancellationToken)
    {
        return await dbContext.Tickets
            .Include(x => x.TriageJobs)
            .SingleOrDefaultAsync(x => x.Id == ticketId, cancellationToken)
            ?? throw new NotFoundException("Ticket was not found.");
    }

    private IQueryable<TicketTriageResult> LatestResultQuery(Guid ticketId)
    {
        return dbContext.TicketTriageResults
            .Where(x => x.TicketId == ticketId)
            .OrderByDescending(x => x.CreatedAt);
    }

    private void EnsureCanView(Ticket ticket)
    {
        if (currentUserService.Role is UserRole.Agent or UserRole.Admin)
        {
            return;
        }

        if (ticket.CreatedByUserId != currentUserService.UserId && ticket.AssignedToUserId != currentUserService.UserId)
        {
            throw new ForbiddenException("You do not have access to this ticket triage result.");
        }
    }

    private void EnsureAgentOrAdmin()
    {
        if (currentUserService.Role is not (UserRole.Agent or UserRole.Admin))
        {
            throw new ForbiddenException("Agent or admin role is required.");
        }
    }

    private void MarkReviewed(TicketTriageResult result, TriageReviewStatus status, string? notes)
    {
        result.ReviewStatus = status;
        result.ReviewedByUserId = currentUserService.UserId;
        result.ReviewedAt = DateTimeOffset.UtcNow;
        result.ReviewNotes = notes;
    }

    private static TriageResultResponse Map(TicketTriageResult result)
    {
        return new TriageResultResponse(
            result.Id,
            result.TicketId,
            result.SuggestedCategory,
            result.SuggestedPriority,
            result.ConfidenceScore,
            result.Summary,
            result.SuggestedSteps,
            result.ReviewStatus,
            result.ReviewedByUserId,
            result.ReviewNotes,
            result.CreatedAt,
            result.ReviewedAt);
    }
}
