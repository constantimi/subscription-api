using Microsoft.EntityFrameworkCore;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Services;


/// <summary>
/// Service for managing subscriptions.
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    private readonly AppDbContext _context;
    private readonly IDiscountService _discountService;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionService"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="discountService">The discount service.</param>
    public SubscriptionService(AppDbContext context, IDiscountService discountService)
    {
        _context = context;
        _discountService = discountService;
    }

    /// <summary>
    /// Subscribes a customer to a service.
    /// </summary>
    /// <param name="customerPhoneNumber">The customer's phone number.</param>
    /// <param name="serviceId">The service identifier.</param>
    /// <param name="durationMonths">The duration of the subscription in months.</param>
    /// <returns>The created subscription.</returns>
    /// <exception cref="ArgumentException">Thrown when the service is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the customer is already subscribed to the service.</exception>
    public async Task<Subscription> Subscribe(string customerPhoneNumber, int serviceId, int durationMonths)
    {
        var service = await _context.Services.FindAsync(serviceId)
            ?? throw new ArgumentException("Service not found");

        var existingSubscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.CustomerPhoneNumber == customerPhoneNumber && s.ServiceId == serviceId);

        if (existingSubscription != null)
            throw new InvalidOperationException("Customer already subscribed to this service");

        var subscription = new Subscription
        {
            CustomerPhoneNumber = customerPhoneNumber,
            ServiceId = serviceId,
            DurationMonths = durationMonths,
            SubscriptionDate = DateTime.UtcNow
        };

        _context.Subscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return subscription;
    }

    /// <summary>
    /// Unsubscribes a customer from a service.
    /// </summary>
    /// <param name="customerPhoneNumber">The customer's phone number.</param>
    /// <param name="serviceId">The service identifier.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when the subscription is not found.</exception>
    public async Task Unsubscribe(string customerPhoneNumber, int serviceId)
    {
        var subscription = await _context.Subscriptions
            .FirstOrDefaultAsync(s => s.CustomerPhoneNumber == customerPhoneNumber && s.ServiceId == serviceId)
            ?? throw new ArgumentException("Subscription not found");

        _context.Subscriptions.Remove(subscription);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the subscription summary for a customer.
    /// </summary>
    /// <param name="customerPhoneNumber">The customer's phone number.</param>
    /// <returns>The subscription summary.</returns>
    public async Task<SubscriptionSummary> GetSubscriptionSummary(string customerPhoneNumber)
    {
        var subscriptions = await _context.Subscriptions
            .Include(s => s.Service)
            .Where(s => s.CustomerPhoneNumber == customerPhoneNumber)
            .ToListAsync();

        var summary = new SubscriptionSummary
        {
            CustomerPhoneNumber = customerPhoneNumber,
            Subscriptions = subscriptions.Select(s => new SubscriptionDetail
            {
                ServiceName = s.Service!.Name,
                DurationMonths = s.DurationMonths,
                MonthlyPrice = s.Service.MonthlyPrice
            }).ToList()
        };

        var discounts = await _discountService.CalculateDiscounts(subscriptions);

        summary.TotalCostBeforeDiscounts = subscriptions.Sum(s => s.Service!.MonthlyPrice * s.DurationMonths);
        summary.AppliedDiscounts = discounts;
        summary.TotalDiscounts = discounts.Sum(d => d.Amount);
        summary.FinalCost = summary.TotalCostBeforeDiscounts - summary.TotalDiscounts;

        return summary;
    }
}