using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Infrastructure.Data.Configurations;

public class EnergyLogConfiguration : IEntityTypeConfiguration<EnergyLog>
{
    public void Configure(EntityTypeBuilder<EnergyLog> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EnergyKwh)
            .HasPrecision(10, 2);

        builder.HasIndex(e => e.GangId);
        builder.HasIndex(e => e.PeriodId);
        builder.HasIndex(e => e.LoggedByUserId);
        builder.HasIndex(e => new { e.GangId, e.PeriodId });
    }
}
