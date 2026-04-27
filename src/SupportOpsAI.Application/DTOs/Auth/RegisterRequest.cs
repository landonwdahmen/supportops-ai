using SupportOpsAI.Domain.Enums;

namespace SupportOpsAI.Application.DTOs.Auth;

public record RegisterRequest(
    string Email,
    string DisplayName,
    string Password,
    UserRole Role = UserRole.Customer);
