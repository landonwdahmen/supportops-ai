using FluentValidation;
using SupportOpsAI.Application.DTOs.Tickets;

namespace SupportOpsAI.Application.Validation;

public class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.Category).IsInEnum();
    }
}
