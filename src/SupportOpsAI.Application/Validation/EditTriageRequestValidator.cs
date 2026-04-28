using FluentValidation;
using SupportOpsAI.Application.DTOs.Triage;

namespace SupportOpsAI.Application.Validation;

public class EditTriageRequestValidator : AbstractValidator<EditTriageRequest>
{
    public EditTriageRequestValidator()
    {
        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
