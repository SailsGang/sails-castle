namespace SailsEnergy.Application.Abstractions;

public interface IIdempotencyService
{
    Task<T?> GetCachedResponseAsync<T>(string idempotencyKey, CancellationToken ct = default) where T : class;
    Task CacheResponseAsync<T>(string idempotencyKey, T response, TimeSpan? expiry = null, CancellationToken ct = default) where T : class;
    Task<bool> IsProcessingAsync(string idempotencyKey, CancellationToken ct = default);
    Task SetProcessingAsync(string idempotencyKey, TimeSpan lockDuration, CancellationToken ct = default);
}
