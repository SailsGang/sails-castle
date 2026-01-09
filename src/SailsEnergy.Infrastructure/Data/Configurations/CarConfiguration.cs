using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Infrastructure.Data.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("cars");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => e.OwnerId);

        builder.Property(e => e.Name).HasMaxLength(100);
        builder.Property(e => e.Model).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Manufacturer).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LicensePlate).HasMaxLength(20);

        builder.Ignore(e => e.DomainEvents);
    }
}
