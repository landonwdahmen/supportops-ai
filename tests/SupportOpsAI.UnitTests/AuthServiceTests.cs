using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SupportOpsAI.Application.Auth;
using SupportOpsAI.Application.DTOs.Auth;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Data;
using SupportOpsAI.Infrastructure.Services;
using SupportOpsAI.UnitTests.Fakes;

namespace SupportOpsAI.UnitTests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_creates_user_and_returns_token()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);

        var response = await service.RegisterAsync(new RegisterRequest(
            "person@example.com",
            "Person Example",
            "Password123!",
            UserRole.Customer));

        Assert.NotEqual(Guid.Empty, response.UserId);
        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
        Assert.Equal("person@example.com", response.Email);
        Assert.Equal(1, await dbContext.Users.CountAsync());
    }

    [Fact]
    public async Task LoginAsync_returns_token_for_valid_credentials()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(dbContext);
        await service.RegisterAsync(new RegisterRequest("agent@example.com", "Agent", "Password123!", UserRole.Agent));

        var response = await service.LoginAsync(new LoginRequest("agent@example.com", "Password123!"));

        Assert.Equal(UserRole.Agent, response.Role);
        Assert.False(string.IsNullOrWhiteSpace(response.AccessToken));
    }

    private static SupportOpsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SupportOpsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SupportOpsDbContext(options);
    }

    private static AuthService CreateService(SupportOpsDbContext dbContext)
    {
        var auditLogService = new AuditLogService(dbContext);
        var currentUser = new FakeCurrentUserService();
        var options = Options.Create(new JwtOptions
        {
            Issuer = "SupportOpsAI.Tests",
            Audience = "SupportOpsAI.Tests",
            SigningKey = "test-signing-key-with-at-least-32-characters",
            ExpiresMinutes = 30
        });

        return new AuthService(dbContext, currentUser, options, auditLogService);
    }
}
