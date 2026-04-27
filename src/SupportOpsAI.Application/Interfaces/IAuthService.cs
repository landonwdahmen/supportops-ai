using SupportOpsAI.Application.DTOs.Auth;

namespace SupportOpsAI.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<AuthResponse> GetCurrentUserAsync(CancellationToken cancellationToken = default);
}
