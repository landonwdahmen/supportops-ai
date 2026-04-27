using FluentValidation;
using SupportOpsAI.Application.DTOs.Auth;

namespace SupportOpsAI.Application.Validation;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(256);
        RuleFor(x => x.Password).NotEmpty();
    }
}
