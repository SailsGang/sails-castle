using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Infrastructure.Data.Configurations;

public class GangCarConfiguration : IEntityTypeConfiguration<GangCar>
{
    public void Configure(EntityTypeBuilder<GangCar> builder)
    {
        builder.ToTable("gang_cars");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.GangId);
        builder.HasIndex(e => e.CarId);
        builder.HasIndex(e => new { e.GangId, e.CarId });

        builder.Ignore(e => e.DomainEvents);
    }
}
