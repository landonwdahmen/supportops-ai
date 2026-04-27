using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportOpsAI.Application.Interfaces;
using SupportOpsAI.Infrastructure.Data;
using SupportOpsAI.Infrastructure.Services;

namespace SupportOpsAI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=supportopsai;Username=postgres;Password=postgres";

        services.AddDbContext<SupportOpsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IAuditLogService, AuditLogService>();

        return services;
    }
}
