using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportOpsAI.Application.DTOs.Triage;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tickets/{ticketId:guid}/triage")]
public class TriageController(
    ITriageReviewService triageReviewService,
    IValidator<ApproveTriageRequest> approveValidator,
    IValidator<EditTriageRequest> editValidator,
    IValidator<RejectTriageRequest> rejectValidator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<TriageResultResponse>> Get(Guid ticketId, CancellationToken cancellationToken)
    {
        return Ok(await triageReviewService.GetLatestAsync(ticketId, cancellationToken));
    }

    [HttpPost("approve")]
    [Authorize(Roles = $"{nameof(UserRole.Agent)},{nameof(UserRole.Admin)}")]
    public async Task<ActionResult<TriageResultResponse>> Approve(
        Guid ticketId,
        ApproveTriageRequest request,
        CancellationToken cancellationToken)
    {
        await approveValidator.ValidateAndThrowAsync(request, cancellationToken);
        return Ok(await triageReviewService.ApproveAsync(ticketId, request, cancellationToken));
    }

    [HttpPost("edit")]
    [Authorize(Roles = $"{nameof(UserRole.Agent)},{nameof(UserRole.Admin)}")]
    public async Task<ActionResult<TriageResultResponse>> Edit(
        Guid ticketId,
        EditTriageRequest request,
        CancellationToken cancellationToken)
    {
        await editValidator.ValidateAndThrowAsync(request, cancellationToken);
        return Ok(await triageReviewService.EditAsync(ticketId, request, cancellationToken));
    }

    [HttpPost("reject")]
    [Authorize(Roles = $"{nameof(UserRole.Agent)},{nameof(UserRole.Admin)}")]
    public async Task<ActionResult<TriageResultResponse>> Reject(
        Guid ticketId,
        RejectTriageRequest request,
        CancellationToken cancellationToken)
    {
        await rejectValidator.ValidateAndThrowAsync(request, cancellationToken);
        return Ok(await triageReviewService.RejectAsync(ticketId, request, cancellationToken));
    }

    [HttpPost("retry")]
    [Authorize(Roles = $"{nameof(UserRole.Agent)},{nameof(UserRole.Admin)}")]
    public async Task<ActionResult<RetryTriageResponse>> Retry(Guid ticketId, CancellationToken cancellationToken)
    {
        return Ok(await triageReviewService.RetryAsync(ticketId, cancellationToken));
    }
}
