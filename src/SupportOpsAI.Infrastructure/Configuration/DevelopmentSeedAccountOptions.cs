namespace SupportOpsAI.Infrastructure.Configuration;

public class DevelopmentSeedAccountOptions
{
    public const string SectionName = "DevelopmentSeedAccounts";

    public bool Enabled { get; set; }
    public SeedAccountOptions Admin { get; set; } = new();
    public SeedAccountOptions Agent { get; set; } = new();
    public string AdminPassword
    {
        get => Admin.Password;
        set => Admin.Password = value ?? string.Empty;
    }

    public string AgentPassword
    {
        get => Agent.Password;
        set => Agent.Password = value ?? string.Empty;
    }
}

public class SeedAccountOptions
{
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
