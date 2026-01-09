using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.IdentityId).IsUnique();

        builder.Property(e => e.DisplayName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.FirstName).HasMaxLength(100);
        builder.Property(e => e.LastName).HasMaxLength(100);
        builder.Property(e => e.AvatarUrl).HasMaxLength(500);

        builder.Ignore(e => e.DomainEvents);
    }
}
