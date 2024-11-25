using SubscriptionAPI.Infrastructure.Data.Repositories;

namespace SubscriptionAPI.Infrastructure.Data;

/// <summary>
/// Represents the unit of work pattern for managing repositories and saving changes to the database.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private IServiceRepository? _serviceRepository;
    private ISubscriptionRepository? _subscriptionRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class with the specified database context.
    /// </summary>
    /// <param name="context">The database context to be used by the unit of work.</param>
    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets the service repository.
    /// </summary>
    public IServiceRepository Services =>
        _serviceRepository ??= new ServiceRepository(_context);

    /// <summary>
    /// Gets the subscription repository.
    /// </summary>
    public ISubscriptionRepository Subscriptions =>
        _subscriptionRepository ??= new SubscriptionRepository(_context);

    /// <summary>
    /// Asynchronously saves all changes made in this unit of work to the database.
    /// </summary>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Disposes the unit of work and releases the database context.
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }
}