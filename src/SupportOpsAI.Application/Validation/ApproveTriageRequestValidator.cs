using FluentValidation;
using SupportOpsAI.Application.DTOs.Triage;

namespace SupportOpsAI.Application.Validation;

public class ApproveTriageRequestValidator : AbstractValidator<ApproveTriageRequest>
{
    public ApproveTriageRequestValidator()
    {
        RuleFor(x => x.Notes).MaximumLength(2000);
    }
}
