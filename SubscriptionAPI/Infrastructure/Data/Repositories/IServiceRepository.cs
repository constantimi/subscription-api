using SubscriptionAPI.Models;
using System.Linq.Expressions;

namespace SubscriptionAPI.Infrastructure.Data.Repositories;

/// <summary>
/// Interface for service repository to handle CRUD operations for services.
/// </summary>
public interface IServiceRepository
{
    /// <summary>
    /// Retrieves a service by its unique identifier.
    /// </summary>
    /// <param name="id">The service identifier.</param>
    /// <returns>The service if found; otherwise, null.</returns>
    Task<Service?> GetByIdAsync(int id);

    /// <summary>
    /// Retrieves a service by its name.
    /// </summary>
    /// <param name="name">The service name.</param>
    /// <returns>The service if found; otherwise, null.</returns>
    Task<Service?> GetByNameAsync(string name);

    /// <summary>
    /// Retrieves all services.
    /// </summary>
    /// <returns>A collection of all services.</returns>
    Task<IEnumerable<Service>> GetAllAsync();

    /// <summary>
    /// Finds a service based on a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter services.</param>
    /// <returns>The service if found; otherwise, null.</returns>
    Task<Service?> FindAsync(Expression<Func<Service, bool>> predicate);

    /// <summary>
    /// Finds all services that match a specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter services.</param>
    /// <returns>A collection of services that match the predicate.</returns>
    Task<IEnumerable<Service>> FindAllAsync(Expression<Func<Service, bool>> predicate);
}