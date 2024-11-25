using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Infrastructure.Data.Repositories;

/// <summary>
/// Represents the repository for managing <see cref="Subscription"/> entities.
/// </summary>
public class SubscriptionRepository : Repository<Subscription>, ISubscriptionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionRepository"/> class.
    /// </summary>
    /// <param name="context">The database context to be used by the repository.</param>
    public SubscriptionRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Asynchronously finds all <see cref="Subscription"/> entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the subscriptions.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="Subscription"/> entities.</returns>
    public override async Task<IEnumerable<Subscription>> FindAllAsync(
        Expression<Func<Subscription, bool>> predicate)
    {
        return await Context.Subscriptions
            .Include(s => s.Service)
            .Where(predicate)
            .ToListAsync();
    }
}