namespace SubscriptionAPI.Models;

/// <summary>
/// Represents a subscription made by a customer.
/// </summary>
public class Subscription
{
    /// <summary>
    /// Gets or sets the unique identifier for the subscription.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the customer.
    /// </summary>
    public string CustomerPhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the unique identifier for the service.
    /// </summary>
    public int ServiceId { get; set; }

    /// <summary>
    /// Gets or sets the service associated with the subscription.
    /// </summary>
    public Service? Service { get; set; }

    /// <summary>
    /// Gets or sets the duration of the subscription in months.
    /// </summary>
    public int DurationMonths { get; set; }

    /// <summary>
    /// Gets or sets the date when the subscription was made.
    /// </summary>
    public DateTime SubscriptionDate { get; set; }
}