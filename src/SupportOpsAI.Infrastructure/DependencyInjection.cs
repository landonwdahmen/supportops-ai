using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportOpsAI.Application.Triage;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Infrastructure.Configuration;
using SupportOpsAI.Infrastructure.AI;
using SupportOpsAI.Infrastructure.Data;
using SupportOpsAI.Infrastructure.Messaging;
using SupportOpsAI.Infrastructure.Services;

namespace SupportOpsAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=supportopsai;Username=postgres;Password=postgres";

        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.Configure<AiTriageOptions>(configuration.GetSection(AiTriageOptions.SectionName));
        services.Configure<OpenAiOptions>(configuration.GetSection(OpenAiOptions.SectionName));
        services.Configure<DevelopmentSeedAccountOptions>(configuration.GetSection(DevelopmentSeedAccountOptions.SectionName));
        services.AddDbContext<SupportOpsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<DevelopmentSeedAccountService>();
        services.AddScoped<ITriageQueuePublisher, RabbitMqTriageQueuePublisher>();
        services.AddScoped<ITriageJobProcessor, TriageJobProcessor>();
        services.AddScoped<ITriageReviewService, TriageReviewService>();
        services.AddHttpClient<OpenAiTriageService>();
        services.AddScoped<MockAiTriageService>();
        services.AddScoped<IAiTriageService>(provider =>
        {
            var options = configuration.GetSection(AiTriageOptions.SectionName).Get<AiTriageOptions>() ?? new AiTriageOptions();
            return options.Provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase)
                ? provider.GetRequiredService<OpenAiTriageService>()
                : provider.GetRequiredService<MockAiTriageService>();
        });

        return services;
    }

    public static IServiceCollection AddTriageWorker(this IServiceCollection services)
    {
        services.AddHostedService<RabbitMqTriageJobConsumer>();
        return services;
    }
}
