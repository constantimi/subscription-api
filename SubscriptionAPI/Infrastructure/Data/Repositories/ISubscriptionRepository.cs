using System.Linq.Expressions;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Infrastructure.Data.Repositories;

/// <summary>
/// Repository interface for managing Subscription entities.
/// </summary>
public interface ISubscriptionRepository
{
    /// <summary>
    /// Retrieves a subscription by its unique identifier.
    /// </summary>
    /// <param name="id">The subscription identifier.</param>
    /// <returns>The subscription if found; otherwise, null.</returns>
    Task<Subscription?> GetByIdAsync(int id);

    /// <summary>
    /// Finds a subscription based on a predicate.
    /// </summary>
    /// <param name="predicate">The search condition.</param>
    /// <returns>The first subscription matching the condition; otherwise, null.</returns>
    Task<Subscription?> FindAsync(Expression<Func<Subscription, bool>> predicate);

    /// <summary>
    /// Finds all subscriptions matching a predicate.
    /// </summary>
    /// <param name="predicate">The search condition.</param>
    /// <returns>A collection of matching subscriptions.</returns>
    Task<IEnumerable<Subscription>> FindAllAsync(Expression<Func<Subscription, bool>> predicate);

    /// <summary>
    /// Adds a new subscription.
    /// </summary>
    /// <param name="subscription">The subscription to add.</param>
    Task AddAsync(Subscription subscription);

    /// <summary>
    /// Removes an existing subscription.
    /// </summary>
    /// <param name="subscription">The subscription to remove.</param>
    void Remove(Subscription subscription);
}