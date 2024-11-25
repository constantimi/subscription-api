using SubscriptionAPI.Models;

namespace SubscriptionAPI.Infrastructure.Factories;

/// <summary>
/// Factory interface for creating Subscription entities.
/// </summary>
public interface ISubscriptionFactory
{
    /// <summary>
    /// Creates a new subscription instance.
    /// </summary>
    /// <param name="customerPhoneNumber">The customer's phone number.</param>
    /// <param name="service">The service being subscribed to.</param>
    /// <param name="durationMonths">The subscription duration in months.</param>
    /// <returns>A new subscription instance.</returns>
    Subscription Create(string customerPhoneNumber, Service service, int durationMonths);
}