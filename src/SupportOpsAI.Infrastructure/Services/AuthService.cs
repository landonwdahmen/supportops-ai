using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SupportOpsAI.Application.Auth;
using SupportOpsAI.Application.DTOs.Auth;
using SupportOpsAI.Application.Exceptions;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Data;
using SupportOpsAI.Infrastructure.Security;

namespace SupportOpsAI.Infrastructure.Services;

public class AuthService(
    SupportOpsDbContext dbContext,
    ICurrentUserService currentUserService,
    IOptions<JwtOptions> jwtOptions,
    IAuditLogService auditLogService) : IAuthService
{
    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var exists = await dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken);
        if (exists)
        {
            throw new ConflictException("A user with this email already exists.");
        }

        var user = new User
        {
            Email = email,
            DisplayName = request.DisplayName.Trim(),
            PasswordHash = PasswordHasher.Hash(request.Password),
            Role = request.Role
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.RecordAsync(AuditEventType.UserRegistered, "User registered.", user.Id, cancellationToken: cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        await auditLogService.RecordAsync(AuditEventType.UserLoggedIn, "User logged in.", user.Id, cancellationToken: cancellationToken);

        return CreateAuthResponse(user);
    }

    public async Task<AuthResponse> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException("Authentication is required.");
        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new NotFoundException("Current user was not found.");

        return CreateAuthResponse(user);
    }

    private AuthResponse CreateAuthResponse(User user)
    {
        var options = jwtOptions.Value;
        if (string.IsNullOrWhiteSpace(options.SigningKey))
        {
            throw new InvalidOperationException("JWT signing key is not configured.");
        }

        var expiresAt = DateTimeOffset.UtcNow.AddMinutes(options.ExpiresMinutes);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SigningKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expiresAt.UtcDateTime,
            signingCredentials: credentials);

        return new AuthResponse(
            user.Id,
            user.Email,
            user.DisplayName,
            user.Role,
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);
    }
}
