using Marten;
using Microsoft.Extensions.Logging;
using SailsEnergy.Application.Abstractions;
using SailsEnergy.Infrastructure.Data;

namespace SailsEnergy.Infrastructure.Services;

/// <summary>
/// Coordinates operations across EF Core and Marten using saga pattern with compensation.
/// </summary>
public class UnitOfWorkCoordinator(
    AppDbContext dbContext,
    IDocumentSession documentSession,
    ICacheService cache,
    ILogger<UnitOfWorkCoordinator> logger)
    : IUnitOfWorkCoordinator
{
    public async Task<T> ExecuteAsync<T>(
        Guid gangId,
        Func<IUnitOfWorkContext, Task<T>> operation,
        CancellationToken ct = default)
    {
        var lockKey = $"uow:gang:{gangId}";

        var lockAcquired = await TryAcquireLockAsync(lockKey, ct);
        if (!lockAcquired) throw new InvalidOperationException("Unable to acquire lock for this operation. Please try again.");

        var context = new UnitOfWorkContext(dbContext, documentSession);

        try
        {
            var result = await operation(context);

            await context.SaveEfChangesAsync(ct);

            try
            {
                await context.SaveMartenChangesAsync(ct);
            }
            catch (Exception martenEx)
            {
                logger.LogError(martenEx, "Marten save failed for gang {GangId}. Executing compensations.", gangId);

                await context.ExecuteCompensationsAsync(ct);

                throw;
            }

            return result;
        }
        finally
        {
            await ReleaseLockAsync(lockKey, ct);
        }
    }

    private async Task<bool> TryAcquireLockAsync(string lockKey, CancellationToken ct)
    {
        var existing = await cache.GetAsync<LockValue>(lockKey, ct);
        if (existing is not null)
            return false;

        await cache.SetAsync(lockKey, new LockValue(DateTimeOffset.UtcNow), TimeSpan.FromSeconds(30), ct);
        return true;
    }

    private async Task ReleaseLockAsync(string lockKey, CancellationToken ct) => await cache.RemoveAsync(lockKey, ct);
}

internal class UnitOfWorkContext(AppDbContext dbContext, IDocumentSession documentSession) : IUnitOfWorkContext
{
    private readonly List<Func<CancellationToken, Task>> _compensations = [];

    public async Task SaveEfChangesAsync(CancellationToken ct = default) => await dbContext.SaveChangesAsync(ct);

    public async Task SaveMartenChangesAsync(CancellationToken ct = default) => await documentSession.SaveChangesAsync(ct);

    public void RegisterCompensation(Func<CancellationToken, Task> compensation) => _compensations.Add(compensation);

    public async Task ExecuteCompensationsAsync(CancellationToken ct)
    {
        for (var i = _compensations.Count - 1; i >= 0; i--)
        {
            try
            {
                await _compensations[i](ct);
            }
            catch
            {
                // Log but continue with other compensations
            }
        }
    }
}

/// <summary>
/// Wrapper for lock value to satisfy cache reference type constraint.
/// </summary>
internal sealed record LockValue(DateTimeOffset AcquiredAt);
