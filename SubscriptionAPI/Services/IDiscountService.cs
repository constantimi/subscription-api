using SubscriptionAPI.Models;

namespace SubscriptionAPI.Services;

/// <summary>
/// Service interface for calculating subscription discounts.
/// </summary>
public interface IDiscountService
{
    /// <summary>
    /// Calculates applicable discounts for a collection of subscriptions.
    /// </summary>
    /// <param name="subscriptions">The subscriptions to calculate discounts for.</param>
    /// <returns>A list of discount details.</returns>
    /// <remarks>
    /// Applies the following discount rules:
    /// 1. Service Pair Promotion: Health&Lifestyle free for one month when subscribed with Magazines and News
    /// 2. Quantity-Based Discount: 10% off total when subscribing to 3 or more services
    /// 3. Bundled Discount: €5 off when subscribing to both Gaming+ and eLearning Portal
    /// 4. Upfront Subscription Bonus: One month free when subscribing for 5 or more months
    /// </remarks>
    Task<List<DiscountDetail>> CalculateDiscounts(List<Subscription> subscriptions);
}

