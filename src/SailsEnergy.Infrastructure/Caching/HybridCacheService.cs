using Microsoft.Extensions.Caching.Hybrid;
using SailsEnergy.Application.Abstractions;

namespace SailsEnergy.Infrastructure.Caching;

public class HybridCacheService(HybridCache cache) : ICacheService
{
    private static readonly HybridCacheEntryOptions _defaultOptions = new()
    {
        Expiration = TimeSpan.FromMinutes(5),
        LocalCacheExpiration = TimeSpan.FromMinutes(1)
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class
        => await cache.GetOrCreateAsync<T?>(key, _ => ValueTask.FromResult<T?>(null), cancellationToken: ct);

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default) where T : class
    {
        var options = expiration.HasValue
            ? new HybridCacheEntryOptions { Expiration = expiration.Value }
            : _defaultOptions;

        await cache.SetAsync(key, value, options, cancellationToken: ct);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default) => await cache.RemoveAsync(key, ct);

    private static string EntityKey<T>(Guid id) => $"{typeof(T).Name.ToLower()}:{id}";

    public Task<T?> GetEntityAsync<T>(Guid id, CancellationToken ct = default) where T : class
        => GetAsync<T>(EntityKey<T>(id), ct);

    public Task SetEntityAsync<T>(Guid id, T value, CancellationToken ct = default) where T : class
        => SetAsync(EntityKey<T>(id), value, null, ct);

    public Task InvalidateEntityAsync<T>(Guid id, CancellationToken ct = default) where T : class
        => RemoveAsync(EntityKey<T>(id), ct);
}
