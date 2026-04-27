using Microsoft.EntityFrameworkCore;
using SupportOpsAI.Application.DTOs.Tickets;
using SupportOpsAI.Application.Exceptions;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Data;

namespace SupportOpsAI.Infrastructure.Services;

public class TicketService(
    SupportOpsDbContext dbContext,
    ICurrentUserService currentUserService,
    IAuditLogService auditLogService) : ITicketService
{
    public async Task<TicketResponse> CreateAsync(CreateTicketRequest request, CancellationToken cancellationToken = default)
    {
        var userId = RequireUserId();
        var ticket = new Ticket
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            Priority = request.Priority,
            Category = request.Category,
            Status = TicketStatus.PendingTriage,
            CreatedByUserId = userId
        };

        dbContext.Tickets.Add(ticket);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.RecordAsync(AuditEventType.TicketCreated, "Ticket created.", userId, ticket.Id, cancellationToken);

        return MapTicket(ticket);
    }

    public async Task<IReadOnlyCollection<TicketResponse>> GetTicketsAsync(CancellationToken cancellationToken = default)
    {
        var userId = RequireUserId();
        var role = currentUserService.Role;

        var query = dbContext.Tickets.AsNoTracking();
        if (role is not (UserRole.Agent or UserRole.Admin))
        {
            query = query.Where(x => x.CreatedByUserId == userId || x.AssignedToUserId == userId);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => MapTicket(x))
            .ToListAsync(cancellationToken);
    }

    public async Task<TicketDetailResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var ticket = await dbContext.Tickets
            .AsNoTracking()
            .Include(x => x.CreatedByUser)
            .Include(x => x.AssignedToUser)
            .Include(x => x.Comments.OrderBy(c => c.CreatedAt))
                .ThenInclude(x => x.AuthorUser)
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new NotFoundException("Ticket was not found.");

        EnsureCanAccess(ticket);

        return new TicketDetailResponse(
            ticket.Id,
            ticket.Title,
            ticket.Description,
            ticket.Status,
            ticket.Priority,
            ticket.Category,
            ticket.CreatedByUserId,
            ticket.CreatedByUser?.DisplayName,
            ticket.AssignedToUserId,
            ticket.AssignedToUser?.DisplayName,
            ticket.CreatedAt,
            ticket.UpdatedAt,
            ticket.Comments.Select(MapComment).ToList());
    }

    public async Task<TicketCommentResponse> AddCommentAsync(Guid ticketId, TicketCommentRequest request, CancellationToken cancellationToken = default)
    {
        var userId = RequireUserId();
        var ticket = await dbContext.Tickets.SingleOrDefaultAsync(x => x.Id == ticketId, cancellationToken)
            ?? throw new NotFoundException("Ticket was not found.");

        EnsureCanAccess(ticket);

        var comment = new TicketComment
        {
            TicketId = ticketId,
            AuthorUserId = userId,
            Body = request.Body.Trim()
        };

        ticket.UpdatedAt = DateTimeOffset.UtcNow;
        dbContext.TicketComments.Add(comment);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.RecordAsync(AuditEventType.TicketCommentAdded, "Ticket comment added.", userId, ticketId, cancellationToken);

        var authorName = await dbContext.Users
            .Where(x => x.Id == userId)
            .Select(x => x.DisplayName)
            .SingleAsync(cancellationToken);

        return MapComment(comment, authorName);
    }

    private Guid RequireUserId()
    {
        return currentUserService.UserId ?? throw new UnauthorizedAccessException("Authentication is required.");
    }

    private void EnsureCanAccess(Ticket ticket)
    {
        var userId = RequireUserId();
        if (currentUserService.Role is UserRole.Agent or UserRole.Admin)
        {
            return;
        }

        if (ticket.CreatedByUserId != userId && ticket.AssignedToUserId != userId)
        {
            throw new ForbiddenException("You do not have access to this ticket.");
        }
    }

    private static TicketResponse MapTicket(Ticket ticket)
    {
        return new TicketResponse(
            ticket.Id,
            ticket.Title,
            ticket.Status,
            ticket.Priority,
            ticket.Category,
            ticket.CreatedByUserId,
            ticket.AssignedToUserId,
            ticket.CreatedAt);
    }

    private static TicketCommentResponse MapComment(TicketComment comment)
    {
        return MapComment(comment, comment.AuthorUser?.DisplayName);
    }

    private static TicketCommentResponse MapComment(TicketComment comment, string? authorName)
    {
        return new TicketCommentResponse(
            comment.Id,
            comment.TicketId,
            comment.AuthorUserId,
            authorName,
            comment.Body,
            comment.CreatedAt);
    }
}
