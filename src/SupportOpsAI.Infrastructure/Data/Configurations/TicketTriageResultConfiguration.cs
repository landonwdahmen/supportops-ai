using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportOpsAI.Domain.Entities;

namespace SupportOpsAI.Infrastructure.Data.Configurations;

public class TicketTriageResultConfiguration : IEntityTypeConfiguration<TicketTriageResult>
{
    public void Configure(EntityTypeBuilder<TicketTriageResult> builder)
    {
        builder.ToTable("ticket_triage_results");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SuggestedPriority).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.SuggestedCategory).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(x => x.ReviewStatus).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Summary).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Reasoning).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.TriageResults)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.TicketId);
    }
}
