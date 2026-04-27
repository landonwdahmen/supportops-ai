using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SupportOpsAI.Infrastructure.Data;

public class SupportOpsDbContextFactory : IDesignTimeDbContextFactory<SupportOpsDbContext>
{
    public SupportOpsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SupportOpsDbContext>();
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=supportopsai;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new SupportOpsDbContext(optionsBuilder.Options);
    }
}
