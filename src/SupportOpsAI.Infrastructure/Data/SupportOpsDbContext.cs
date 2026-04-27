using Microsoft.EntityFrameworkCore;
using SupportOpsAI.Domain.Entities;

namespace SupportOpsAI.Infrastructure.Data;

public class SupportOpsDbContext(DbContextOptions<SupportOpsDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();
    public DbSet<TicketTriageResult> TicketTriageResults => Set<TicketTriageResult>();
    public DbSet<TriageJob> TriageJobs => Set<TriageJob>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SupportOpsDbContext).Assembly);
    }
}
