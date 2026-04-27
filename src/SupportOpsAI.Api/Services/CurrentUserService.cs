using System.Security.Claims;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var userId) ? userId : null;
        }
    }

    public string? Email => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

    public UserRole? Role
    {
        get
        {
            var value = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            return Enum.TryParse<UserRole>(value, out var role) ? role : null;
        }
    }

    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true;
}
