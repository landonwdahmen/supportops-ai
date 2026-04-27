using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Application.DTOs.Auth;

public record AuthResponse(
    Guid UserId,
    string Email,
    string DisplayName,
    UserRole Role,
    string AccessToken,
    DateTimeOffset ExpiresAt);
