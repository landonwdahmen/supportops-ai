using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportOpsAI.Domain.Entities;

namespace SupportOpsAI.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.EventType).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(x => x.User)
            .WithMany(x => x.AuditLogs)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.AuditLogs)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.TicketId);
        builder.HasIndex(x => x.EventType);
        builder.HasIndex(x => x.CreatedAt);
    }
}
