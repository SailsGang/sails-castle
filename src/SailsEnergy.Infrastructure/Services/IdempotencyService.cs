using SailsEnergy.Application.Abstractions;

namespace SailsEnergy.Infrastructure.Services;

public class IdempotencyService(ICacheService cache) : IIdempotencyService
{
    private static readonly TimeSpan _defaultExpiry = TimeSpan.FromHours(24);

    public async Task<T?> GetCachedResponseAsync<T>(string idempotencyKey, CancellationToken ct = default) where T : class
    {
        var cacheKey = $"idempotency:response:{idempotencyKey}";

        return await cache.GetAsync<T>(cacheKey, ct);
    }

    public async Task CacheResponseAsync<T>(string idempotencyKey, T response, TimeSpan? expiry = null, CancellationToken ct = default) where T : class
    {
        var cacheKey = $"idempotency:response:{idempotencyKey}";
        await cache.SetAsync(cacheKey, response, expiry ?? _defaultExpiry, ct);

        var lockKey = $"idempotency:lock:{idempotencyKey}";
        await cache.RemoveAsync(lockKey, ct);
    }

    public async Task<bool> IsProcessingAsync(string idempotencyKey, CancellationToken ct = default)
    {
        var lockKey = $"idempotency:lock:{idempotencyKey}";
        var result = await cache.GetAsync<string>(lockKey, ct);

        return result is not null;
    }

    public async Task SetProcessingAsync(string idempotencyKey, TimeSpan lockDuration, CancellationToken ct = default)
    {
        var lockKey = $"idempotency:lock:{idempotencyKey}";
        await cache.SetAsync(lockKey, "processing", lockDuration, ct);
    }
}
