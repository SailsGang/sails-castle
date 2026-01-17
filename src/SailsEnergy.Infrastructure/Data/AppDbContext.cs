using Microsoft.EntityFrameworkCore;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options), IAppDbContext
{
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<Gang> Gangs => Set<Gang>();
    public DbSet<GangMember> GangMembers => Set<GangMember>();
    public DbSet<Car> Cars => Set<Car>();
    public DbSet<GangCar> GangCars => Set<GangCar>();
    public DbSet<Period> Periods => Set<Period>();
    public DbSet<Tariff> Tariffs => Set<Tariff>();
    public DbSet<EnergyLog> EnergyLogs => Set<EnergyLog>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        modelBuilder.Entity<UserProfile>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Gang>().HasQueryFilter(e => !e.IsDeleted);
        modelBuilder.Entity<Car>().HasQueryFilter(e => !e.IsDeleted);

        modelBuilder.Entity<GangMember>().HasQueryFilter(e => e.IsActive);
        modelBuilder.Entity<GangCar>().HasQueryFilter(e => e.IsDeleted);
    }
}
