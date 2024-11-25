using SubscriptionAPI.Models;

namespace SubscriptionAPI.Infrastructure.Factories;

/// <summary>
/// Factory class for creating <see cref="Subscription"/> instances.
/// </summary>
public class SubscriptionFactory : ISubscriptionFactory
{
    /// <summary>
    /// Creates a new <see cref="Subscription"/> instance.
    /// </summary>
    /// <param name="customerPhoneNumber">The phone number of the customer.</param>
    /// <param name="service">The service associated with the subscription.</param>
    /// <param name="durationMonths">The duration of the subscription in months.</param>
    /// <returns>A new <see cref="Subscription"/> instance.</returns>
    public Subscription Create(string customerPhoneNumber, Service service, int durationMonths)
    {
        return new Subscription
        {
            CustomerPhoneNumber = customerPhoneNumber,
            ServiceId = service.Id,
            Service = service,
            DurationMonths = durationMonths,
            SubscriptionDate = DateTime.UtcNow
        };
    }
}