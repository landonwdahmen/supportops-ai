using FluentValidation;
using SupportOpsAI.Application.DTOs.Triage;

namespace SupportOpsAI.Application.Validation;

public class RejectTriageRequestValidator : AbstractValidator<RejectTriageRequest>
{
    public RejectTriageRequestValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().MaximumLength(2000);
    }
}
