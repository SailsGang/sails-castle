using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Application.Tests;

/// <summary>
/// In-memory test implementation of IAppDbContext for unit testing.
/// </summary>
public class TestAppDbContext : IAppDbContext
{
    private readonly InMemoryDbContext _context;

    public TestAppDbContext()
    {
        var options = new DbContextOptionsBuilder<InMemoryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _context = new InMemoryDbContext(options);
    }

    public DbSet<Gang> Gangs => _context.Gangs;
    public DbSet<GangMember> GangMembers => _context.GangMembers;
    public DbSet<Car> Cars => _context.Cars;
    public DbSet<GangCar> GangCars => _context.GangCars;
    public DbSet<EnergyLog> EnergyLogs => _context.EnergyLogs;
    public DbSet<UserProfile> UserProfiles => _context.UserProfiles;
    public DbSet<Period> Periods => _context.Periods;
    public DbSet<Tariff> Tariffs => _context.Tariffs;

    public DatabaseFacade Database => _context.Database;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) => _context.SaveChangesAsync(ct);

    private class InMemoryDbContext(DbContextOptions<InMemoryDbContext> options) : DbContext(options)
    {
        public DbSet<Gang> Gangs => Set<Gang>();
        public DbSet<GangMember> GangMembers => Set<GangMember>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<GangCar> GangCars => Set<GangCar>();
        public DbSet<EnergyLog> EnergyLogs => Set<EnergyLog>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Period> Periods => Set<Period>();
        public DbSet<Tariff> Tariffs => Set<Tariff>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Gang>().HasKey(g => g.Id);
            modelBuilder.Entity<GangMember>().HasKey(m => m.Id);
            modelBuilder.Entity<Car>().HasKey(c => c.Id);
            modelBuilder.Entity<GangCar>().HasKey(gc => gc.Id);
            modelBuilder.Entity<EnergyLog>().HasKey(e => e.Id);
            modelBuilder.Entity<UserProfile>().HasKey(u => u.Id);
            modelBuilder.Entity<Period>().HasKey(p => p.Id);
            modelBuilder.Entity<Tariff>().HasKey(t => t.Id);
        }
    }
}
