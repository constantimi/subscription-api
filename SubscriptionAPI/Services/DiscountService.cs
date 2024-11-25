using Microsoft.EntityFrameworkCore;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Services;

/// <summary>
/// Service to calculate various discounts for subscriptions.
/// </summary>
public class DiscountService : IDiscountService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DiscountService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public DiscountService(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Calculates the discounts for a list of subscriptions.
    /// </summary>
    /// <param name="subscriptions">The list of subscriptions.</param>
    /// <returns>A list of discount details.</returns>
    public async Task<List<DiscountDetail>> CalculateDiscounts(List<Subscription> subscriptions)
    {
        var discounts = new List<DiscountDetail>();

        if (!subscriptions.Any())
            return await Task.FromResult(discounts);

        // 1. Service Pair Promotion
        var healthSubscription = subscriptions.FirstOrDefault(s => s.Service?.Name == "Health&Lifestyle");
        var magazineSubscription = subscriptions.FirstOrDefault(s => s.Service?.Name == "Magazines and News");

        if (healthSubscription != null && magazineSubscription != null)
        {
            // Get one month of Health&Lifestyle free
            var monthlyPrice = healthSubscription.Service!.MonthlyPrice;
            discounts.Add(new DiscountDetail
            {
                DiscountName = "Service Pair Promotion (Health&Lifestyle + Magazines)",
                Amount = monthlyPrice // One month free
            });
        }

        // 2. Quantity-Based Discount (3 or more services)
        if (subscriptions.Count >= 3)
        {
            var totalCost = subscriptions.Sum(s => s.Service!.MonthlyPrice * s.DurationMonths);
            var quantityDiscount = Math.Round(totalCost * 0.10m, 2); // 10% discount
            discounts.Add(new DiscountDetail
            {
                DiscountName = "Quantity-Based Discount (10% off for 3+ services)",
                Amount = quantityDiscount
            });
        }

        // 3. Bundled Discount (Gaming+ and eLearning)
        var hasGaming = subscriptions.Any(s => s.Service?.Name == "Gaming+ Catalogue");
        var hasElearning = subscriptions.Any(s => s.Service?.Name == "eLearning Portal");

        if (hasGaming && hasElearning)
        {
            discounts.Add(new DiscountDetail
            {
                DiscountName = "Bundle Discount (Gaming+ + eLearning)",
                Amount = 5.00m // Flat €5 discount
            });
        }

        // 4. Upfront Subscription Bonus (5+ months gets 6th month free)
        foreach (var subscription in subscriptions.Where(s => s.DurationMonths >= 5))
        {
            var monthlyPrice = subscription.Service!.MonthlyPrice;
            discounts.Add(new DiscountDetail
            {
                DiscountName = $"Upfront Subscription Bonus ({subscription.Service.Name})",
                Amount = monthlyPrice // One month free
            });
        }

        return discounts;
    }
}