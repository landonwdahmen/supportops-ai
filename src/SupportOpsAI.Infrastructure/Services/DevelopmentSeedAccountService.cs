using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SupportOpsAI.Domain.Entities;
using SupportOpsAI.Domain.Enums;
using SupportOpsAI.Infrastructure.Configuration;
using SupportOpsAI.Infrastructure.Data;
using SupportOpsAI.Infrastructure.Security;

namespace SupportOpsAI.Infrastructure.Services;

public class DevelopmentSeedAccountService(
    SupportOpsDbContext dbContext,
    IOptions<DevelopmentSeedAccountOptions> options,
    ILogger<DevelopmentSeedAccountService> logger)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var seedOptions = options.Value;
        if (!seedOptions.Enabled)
        {
            return;
        }

        var accounts = new[]
        {
            (Options: seedOptions.Admin, Role: UserRole.Admin),
            (Options: seedOptions.Agent, Role: UserRole.Agent)
        };

        foreach (var account in accounts)
        {
            if (string.IsNullOrWhiteSpace(account.Options.Password))
            {
                logger.LogWarning(
                    "Skipping development seed account for role {Role} because no password was configured.",
                    account.Role);
                continue;
            }

            await UpsertUserAsync(account.Options, account.Role, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task UpsertUserAsync(
        SeedAccountOptions accountOptions,
        UserRole role,
        CancellationToken cancellationToken)
    {
        var email = accountOptions.Email.Trim().ToLowerInvariant();
        var displayName = accountOptions.DisplayName.Trim();

        var user = await dbContext.Users.SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
        if (user is null)
        {
            dbContext.Users.Add(new User
            {
                Email = email,
                DisplayName = displayName,
                PasswordHash = PasswordHasher.Hash(accountOptions.Password),
                Role = role
            });

            logger.LogInformation("Seeded development {Role} account with email {Email}.", role, email);
            return;
        }

        user.DisplayName = displayName;
        user.PasswordHash = PasswordHasher.Hash(accountOptions.Password);
        user.Role = role;

        logger.LogInformation("Updated development {Role} account with email {Email}.", role, email);
    }
}
