using FluentValidation;
using SupportOpsAI.Application.DTOs.Tickets;

namespace SupportOpsAI.Application.Validation;

public class TicketCommentRequestValidator : AbstractValidator<TicketCommentRequest>
{
    public TicketCommentRequestValidator()
    {
        RuleFor(x => x.Body).NotEmpty().MaximumLength(2000);
    }
}
