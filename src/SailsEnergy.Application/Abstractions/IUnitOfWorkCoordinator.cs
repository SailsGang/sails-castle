namespace SailsEnergy.Application.Abstractions;

/// <summary>
/// Coordinates operations across multiple data stores (EF Core + Marten)
/// using a saga pattern with compensation for atomic operations.
/// </summary>
public interface IUnitOfWorkCoordinator
{
    /// <summary>
    /// Executes operations across EF Core and Marten with compensation on failure.
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    /// <param name="gangId">Gang ID for distributed locking</param>
    /// <param name="operation">The operation to execute</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Operation result</returns>
    Task<T> ExecuteAsync<T>(
        Guid gangId,
        Func<IUnitOfWorkContext, Task<T>> operation,
        CancellationToken ct = default);
}

/// <summary>
/// Context for coordinated unit of work operations.
/// </summary>
public interface IUnitOfWorkContext
{
    /// <summary>
    /// Saves all pending changes to EF Core.
    /// </summary>
    Task SaveEfChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Saves all pending changes to Marten document store.
    /// </summary>
    Task SaveMartenChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Stores a document in Marten.
    /// </summary>
    void StoreDocument<T>(T document) where T : notnull;

    /// <summary>
    /// Registers a compensation action to be executed if the operation fails.
    /// </summary>
    void RegisterCompensation(Func<CancellationToken, Task> compensation);
}
