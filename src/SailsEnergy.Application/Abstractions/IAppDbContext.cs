using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SailsEnergy.Domain.Entities;

namespace SailsEnergy.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<UserProfile> UserProfiles { get; }
    DbSet<Gang> Gangs { get; }
    DbSet<GangMember> GangMembers { get; }
    DbSet<Car> Cars { get; }
    DbSet<GangCar> GangCars { get; }
    DbSet<Period> Periods { get; }
    DbSet<Tariff> Tariffs { get; }
    DbSet<EnergyLog> EnergyLogs { get; }
    DbSet<AuditLog> AuditLogs { get; }

    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
