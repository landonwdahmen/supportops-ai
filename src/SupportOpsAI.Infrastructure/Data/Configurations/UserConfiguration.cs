using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SupportOpsAI.Domain.Entities;

namespace SupportOpsAI.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.PasswordHash).HasMaxLength(512).IsRequired();
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();

        builder.HasIndex(x => x.Email).IsUnique();
    }
}
