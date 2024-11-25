using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Infrastructure.Data.Repositories;

/// <summary>
/// Repository class for managing <see cref="Service"/> entities.
/// </summary>
public class ServiceRepository : Repository<Service>, IServiceRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceRepository"/> class.
    /// </summary>
    /// <param name="context">The database context to be used by the repository.</param>
    public ServiceRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Asynchronously retrieves a <see cref="Service"/> entity by its name.
    /// </summary>
    /// <param name="name">The name of the service to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the <see cref="Service"/> entity if found; otherwise, <c>null</c>.</returns>
    public async Task<Service?> GetByNameAsync(string name)
    {
        return await Context.Services
            .FirstOrDefaultAsync(s => s.Name == name);
    }

    public async Task<IEnumerable<Service>> GetAllAsync()
    {
        return await Context.Services.ToListAsync();
    }

    public override async Task<Service?> FindAsync(Expression<Func<Service, bool>> predicate)
    {
        return await Context.Services.FirstOrDefaultAsync(predicate);
    }

    public override async Task<IEnumerable<Service>> FindAllAsync(Expression<Func<Service, bool>> predicate)
    {
        return await Context.Services.Where(predicate).ToListAsync();
    }
}