using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportOpsAI.Domain.Entities;

namespace SupportOpsAI.Infrastructure.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("tickets");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(5000).IsRequired();
        builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Priority).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(64).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(x => x.CreatedByUser)
            .WithMany(x => x.CreatedTickets)
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AssignedToUser)
            .WithMany(x => x.AssignedTickets)
            .HasForeignKey(x => x.AssignedToUserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Priority);
        builder.HasIndex(x => x.Category);
        builder.HasIndex(x => x.CreatedAt);
        builder.HasIndex(x => x.AssignedToUserId);
    }
}
