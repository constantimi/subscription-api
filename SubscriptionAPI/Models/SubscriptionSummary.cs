namespace SubscriptionAPI.Models;

/// <summary>
/// Represents a summary of a customer's subscription.
/// </summary>
public class SubscriptionSummary
{
    /// <summary>
    /// Gets or sets the customer's phone number.
    /// </summary>
    public string CustomerPhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of subscription details.
    /// </summary>
    public List<SubscriptionDetail> Subscriptions { get; set; } = new();

    /// <summary>
    /// Gets or sets the total cost before applying any discounts.
    /// </summary>
    public decimal TotalCostBeforeDiscounts { get; set; }

    /// <summary>
    /// Gets or sets the total amount of discounts applied.
    /// </summary>
    public decimal TotalDiscounts { get; set; }

    /// <summary>
    /// Gets or sets the final cost after applying discounts.
    /// </summary>
    public decimal FinalCost { get; set; }

    /// <summary>
    /// Gets or sets the list of applied discount details.
    /// </summary>
    public List<DiscountDetail> AppliedDiscounts { get; set; } = new();
}

/// <summary>
/// Represents the details of a subscription.
/// </summary>
public class SubscriptionDetail
{
    /// <summary>
    /// Gets or sets the name of the service.
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration of the subscription in months.
    /// </summary>
    public int DurationMonths { get; set; }

    /// <summary>
    /// Gets or sets the monthly price of the subscription.
    /// </summary>
    public decimal MonthlyPrice { get; set; }
}

/// <summary>
/// Represents the details of a discount applied to a subscription.
/// </summary>
public class DiscountDetail
{
    /// <summary>
    /// Gets or sets the name of the discount.
    /// </summary>
    public string DiscountName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the amount of the discount.
    /// </summary>
    public decimal Amount { get; set; }
}