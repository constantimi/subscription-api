using SubscriptionAPI.Models;

namespace SubscriptionAPI.Services;

/// <summary>
/// Defines the contract for subscription services.
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Subscribes a customer to a service.
    /// </summary>
    /// <param name="customerPhoneNumber">The phone number of the customer.</param>
    /// <param name="serviceId">The identifier of the service.</param>
    /// <param name="durationMonths">The duration of the subscription in months.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription details.</returns>
    Task<Subscription> Subscribe(string customerPhoneNumber, int serviceId, int durationMonths);

    /// <summary>
    /// Unsubscribes a customer from a service.
    /// </summary>
    /// <param name="customerPhoneNumber">The phone number of the customer.</param>
    /// <param name="serviceId">The identifier of the service.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task Unsubscribe(string customerPhoneNumber, int serviceId);

    /// <summary>
    /// Gets the subscription summary for a customer.
    /// </summary>
    /// <param name="customerPhoneNumber">The phone number of the customer.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription summary.</returns>
    Task<SubscriptionSummary> GetSubscriptionSummary(string customerPhoneNumber);
}