using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Infrastructure.Data.Configurations;

public class TariffConfiguration : IEntityTypeConfiguration<Tariff>
{
    public void Configure(EntityTypeBuilder<Tariff> builder)
    {
        builder.ToTable("Tariffs");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.GangId)
            .IsRequired();

        builder.Property(t => t.PricePerKwh)
            .IsRequired()
            .HasPrecision(18, 4);

        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(t => t.EffectiveFrom)
            .IsRequired();

        builder.Property(t => t.SetByUserId)
            .IsRequired();

        // Indexes
        builder.HasIndex(t => t.GangId);
        builder.HasIndex(t => new { t.GangId, t.EffectiveFrom });

        // Audit fields
        builder.Property(t => t.CreatedAt).IsRequired();
        builder.Property(t => t.CreatedBy).IsRequired();
    }
}
