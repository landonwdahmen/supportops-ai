using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportOpsAI.Application.DTOs.Auth;
using SupportOpsAI.Application.Interfaces;

namespace SupportOpsAI.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    IValidator<RegisterRequest> registerValidator,
    IValidator<LoginRequest> loginValidator) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        await registerValidator.ValidateAndThrowAsync(request, cancellationToken);
        return Ok(await authService.RegisterAsync(request, cancellationToken));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        await loginValidator.ValidateAndThrowAsync(request, cancellationToken);
        return Ok(await authService.LoginAsync(request, cancellationToken));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<AuthResponse>> Me(CancellationToken cancellationToken)
    {
        return Ok(await authService.GetCurrentUserAsync(cancellationToken));
    }
}
