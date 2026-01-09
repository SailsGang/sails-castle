using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Infrastructure.Data.Configurations;

public class GangConfiguration : IEntityTypeConfiguration<Gang>
{
    public void Configure(EntityTypeBuilder<Gang> builder)
    {
        builder.ToTable("gangs");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.OwnerId);

        builder.Property(e => e.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description).HasMaxLength(500);

        builder.Ignore(e => e.DomainEvents);
    }
}
