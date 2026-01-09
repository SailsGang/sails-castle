namespace SailsEnergy.Application.Abstractions;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default) where T : class;
    Task RemoveAsync(string key, CancellationToken ct = default);

    Task<T?> GetEntityAsync<T>(Guid id, CancellationToken ct = default) where T : class;
    Task SetEntityAsync<T>(Guid id, T value, CancellationToken ct = default) where T : class;
    Task InvalidateEntityAsync<T>(Guid id, CancellationToken ct = default) where T : class;
}
