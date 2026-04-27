using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportOpsAI.Domain.Entities;

namespace SupportOpsAI.Infrastructure.Data.Configurations;

public class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.ToTable("ticket_comments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Body).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasOne(x => x.Ticket)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AuthorUser)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.AuthorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TicketId);
        builder.HasIndex(x => x.AuthorUserId);
    }
}
