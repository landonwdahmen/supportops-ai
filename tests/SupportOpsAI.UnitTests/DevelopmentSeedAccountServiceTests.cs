using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using SupportOpsAI.Application.Auth;
using SupportOpsAI.Application.DTOs.Auth;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Configuration;
using SupportOpsAI.Infrastructure.Data;
using SupportOpsAI.Infrastructure.Services;
using SupportOpsAI.UnitTests.Fakes;

namespace SupportOpsAI.UnitTests;

public class DevelopmentSeedAccountServiceTests
{
    [Fact]
    public void DevelopmentSeedAccountOptions_bind_flat_password_keys_from_configuration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["DevelopmentSeedAccounts:Enabled"] = "true",
                ["DevelopmentSeedAccounts:AdminPassword"] = "AdminPassword123!",
                ["DevelopmentSeedAccounts:AgentPassword"] = "AgentPassword123!",
                ["DevelopmentSeedAccounts:Admin:Email"] = "admin@supportops.local",
                ["DevelopmentSeedAccounts:Admin:DisplayName"] = "SupportOps Admin",
                ["DevelopmentSeedAccounts:Agent:Email"] = "agent@supportops.local",
                ["DevelopmentSeedAccounts:Agent:DisplayName"] = "SupportOps Agent"
            })
            .Build();

        var options = configuration
            .GetSection(DevelopmentSeedAccountOptions.SectionName)
            .Get<DevelopmentSeedAccountOptions>();

        Assert.NotNull(options);
        Assert.True(options.Enabled);
        Assert.Equal("admin@supportops.local", options.Admin.Email);
        Assert.Equal("SupportOps Admin", options.Admin.DisplayName);
        Assert.Equal("AdminPassword123!", options.Admin.Password);
        Assert.Equal("AdminPassword123!", options.AdminPassword);
        Assert.Equal("agent@supportops.local", options.Agent.Email);
        Assert.Equal("SupportOps Agent", options.Agent.DisplayName);
        Assert.Equal("AgentPassword123!", options.Agent.Password);
        Assert.Equal("AgentPassword123!", options.AgentPassword);
    }

    [Fact]
    public async Task SeedAsync_creates_admin_and_agent_when_passwords_are_configured()
    {
        await using var dbContext = CreateDbContext();
        var service = CreateService(
            dbContext,
            new DevelopmentSeedAccountOptions
            {
                Enabled = true,
                Admin = new SeedAccountOptions
                {
                    Email = "admin@supportops.local",
                    DisplayName = "SupportOps Admin",
                    Password = "AdminPassword123!"
                },
                Agent = new SeedAccountOptions
                {
                    Email = "agent@supportops.local",
                    DisplayName = "SupportOps Agent",
                    Password = "AgentPassword123!"
                }
            });

        await service.SeedAsync();

        var users = await dbContext.Users.OrderBy(x => x.Email).ToListAsync();
        Assert.Collection(
            users,
            user =>
            {
                Assert.Equal("admin@supportops.local", user.Email);
                Assert.Equal("SupportOps Admin", user.DisplayName);
                Assert.Equal(UserRole.Admin, user.Role);
            },
            user =>
            {
                Assert.Equal("agent@supportops.local", user.Email);
                Assert.Equal("SupportOps Agent", user.DisplayName);
                Assert.Equal(UserRole.Agent, user.Role);
            });

        var authService = CreateAuthService(dbContext);
        var adminLogin = await authService.LoginAsync(new LoginRequest("admin@supportops.local", "AdminPassword123!"));
        var agentLogin = await authService.LoginAsync(new LoginRequest("agent@supportops.local", "AgentPassword123!"));

        Assert.Equal(UserRole.Admin, adminLogin.Role);
        Assert.Equal(UserRole.Agent, agentLogin.Role);
    }

    [Fact]
    public async Task SeedAsync_updates_existing_seed_account_and_skips_missing_password()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Users.Add(new Domain.Entities.User
        {
            Email = "admin@supportops.local",
            DisplayName = "Old Admin",
            PasswordHash = "PBKDF2-SHA256.100000.Bd4f7pAE7I6uvmTJ9eJ6SQ==.rX7vWlCby2IkXbQ4mMNmw4sI0M2bVdGfFD88R6oD1uo=",
            Role = UserRole.Customer
        });
        await dbContext.SaveChangesAsync();

        var service = CreateService(
            dbContext,
            new DevelopmentSeedAccountOptions
            {
                Enabled = true,
                Admin = new SeedAccountOptions
                {
                    Email = "admin@supportops.local",
                    DisplayName = "SupportOps Admin",
                    Password = "NewAdminPassword123!"
                },
                Agent = new SeedAccountOptions
                {
                    Email = "agent@supportops.local",
                    DisplayName = "SupportOps Agent",
                    Password = string.Empty
                }
            });

        await service.SeedAsync();

        var users = await dbContext.Users.OrderBy(x => x.Email).ToListAsync();
        Assert.Single(users);
        Assert.Equal("SupportOps Admin", users[0].DisplayName);
        Assert.Equal(UserRole.Admin, users[0].Role);

        var authService = CreateAuthService(dbContext);
        var adminLogin = await authService.LoginAsync(new LoginRequest("admin@supportops.local", "NewAdminPassword123!"));

        Assert.Equal(UserRole.Admin, adminLogin.Role);
    }

    private static SupportOpsDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SupportOpsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new SupportOpsDbContext(options);
    }

    private static DevelopmentSeedAccountService CreateService(
        SupportOpsDbContext dbContext,
        DevelopmentSeedAccountOptions options)
    {
        return new DevelopmentSeedAccountService(
            dbContext,
            Options.Create(options),
            NullLogger<DevelopmentSeedAccountService>.Instance);
    }

    private static AuthService CreateAuthService(SupportOpsDbContext dbContext)
    {
        return new AuthService(
            dbContext,
            new FakeCurrentUserService(),
            Options.Create(new JwtOptions
            {
                Issuer = "SupportOpsAI.Tests",
                Audience = "SupportOpsAI.Tests",
                SigningKey = "test-signing-key-with-at-least-32-characters",
                ExpiresMinutes = 30
            }),
            new AuditLogService(dbContext));
    }
}
