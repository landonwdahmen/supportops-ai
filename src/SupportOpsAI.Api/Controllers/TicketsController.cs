using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportOpsAI.Application.DTOs.Tickets;
using SupportOpsAI.Application.Interfaces;

namespace SupportOpsAI.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tickets")]
public class TicketsController(
    ITicketService ticketService,
    IValidator<CreateTicketRequest> createTicketValidator,
    IValidator<TicketCommentRequest> commentValidator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<TicketResponse>> Create(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        await createTicketValidator.ValidateAndThrowAsync(request, cancellationToken);
        var ticket = await ticketService.CreateAsync(request, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<TicketResponse>>> GetTickets(CancellationToken cancellationToken)
    {
        return Ok(await ticketService.GetTicketsAsync(cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TicketDetailResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await ticketService.GetByIdAsync(id, cancellationToken));
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<ActionResult<TicketCommentResponse>> AddComment(
        Guid id,
        TicketCommentRequest request,
        CancellationToken cancellationToken)
    {
        await commentValidator.ValidateAndThrowAsync(request, cancellationToken);
        return Ok(await ticketService.AddCommentAsync(id, request, cancellationToken));
    }
}
