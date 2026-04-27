using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportOpsAI.Domain.Entities;

namespace SupportOpsAI.Infrastructure.Data.Configurations;

public class TriageJobConfiguration : IEntityTypeConfiguration<TriageJob>
{
    public void Configure(EntityTypeBuilder<TriageJob> builder)
    {
        builder.ToTable("triage_jobs");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.ErrorMessage).HasMaxLength(2000);
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.TriageJobs)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TicketId);
        builder.HasIndex(x => x.Status);
    }
}
