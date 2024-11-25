namespace SubscriptionAPI.Models;

/// <summary>
/// Represents a service with an ID, name, and monthly price.
/// </summary>
public class Service
{
    /// <summary>
    /// Gets or sets the unique identifier for the service.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the monthly price of the service.
    /// </summary>
    public decimal MonthlyPrice { get; set; }
}