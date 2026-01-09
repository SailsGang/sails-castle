using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Infrastructure.Data.Configurations;

public class GangMemberConfiguration : IEntityTypeConfiguration<GangMember>
{
    public void Configure(EntityTypeBuilder<GangMember> builder)
    {
        builder.ToTable("gang_members");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.GangId);
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.GangId, e.UserId });

        builder.Property(e => e.Role)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Ignore(e => e.DomainEvents);
    }
}
