using Microsoft.EntityFrameworkCore;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Services;
using Xunit;

namespace SubscriptionAPI.Tests.Services;

/// <summary>
/// Unit tests for the <see cref="SubscriptionService"/>.
/// These tests verify the functionality of subscription creation, subscription removal, 
/// and subscription summary calculations, including discounts.
/// </summary>
public class SubscriptionServiceTests
{
    private readonly AppDbContext _context;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IDiscountService _discountService;

    /// <summary>
    /// Initializes the test setup, creating an in-memory database and initializing required services.
    /// </summary>
    public SubscriptionServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _discountService = new DiscountService(_context);
        _subscriptionService = new SubscriptionService(_context, _discountService);

        DbInitializer.Initialize(_context);
    }

    /// <summary>
    /// Tests the <see cref="SubscriptionService.Subscribe"/> method with valid input to ensure 
    /// that a subscription is successfully created.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous test execution.
    /// </returns>
    [Fact]
    public async Task Subscribe_ValidInput_CreatesSubscription()
    {
        // Arrange
        var phoneNumber = "+1234567890";
        var serviceId = 1;
        var duration = 1;

        // Act
        var subscription = await _subscriptionService.Subscribe(phoneNumber, serviceId, duration);

        // Assert
        Assert.NotNull(subscription);
        Assert.Equal(phoneNumber, subscription.CustomerPhoneNumber);
        Assert.Equal(serviceId, subscription.ServiceId);
        Assert.Equal(duration, subscription.DurationMonths);
    }

    /// <summary>
    /// Tests the <see cref="SubscriptionService.Subscribe"/> method to ensure that attempting 
    /// to subscribe a customer to the same service twice results in a duplicate subscription exception.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous test execution.
    /// </returns>
    [Fact]
    public async Task Subscribe_DuplicateSubscription_ThrowsException()
    {
        // Arrange
        var phoneNumber = "+1234567890";
        var serviceId = 1;
        var duration = 1;

        // Act & Assert
        await _subscriptionService.Subscribe(phoneNumber, serviceId, duration);
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _subscriptionService.Subscribe(phoneNumber, serviceId, duration));
    }

    /// <summary>
    /// Tests the <see cref="SubscriptionService.GetSubscriptionSummary"/> method to ensure that
    /// the subscription summary includes discounts, and the final cost is calculated correctly.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous test execution.
    /// </returns>
    [Fact]
    public async Task GetSubscriptionSummary_WithDiscounts_CalculatesCorrectly()
    {
        // Arrange
        var phoneNumber = "+1234567890";

        // Subscribe to Gaming+ and eLearning (Bundled discount)
        await _subscriptionService.Subscribe(phoneNumber, 3, 1); // Gaming+
        await _subscriptionService.Subscribe(phoneNumber, 1, 1); // eLearning

        // Act
        var summary = await _subscriptionService.GetSubscriptionSummary(phoneNumber);

        // Assert
        Assert.Equal(25m, summary.TotalCostBeforeDiscounts); // 15 + 10
        Assert.Equal(5m, summary.TotalDiscounts); // Bundled discount
        Assert.Equal(20m, summary.FinalCost);
    }

    /// <summary>
    /// Tests the <see cref="SubscriptionService.Unsubscribe"/> method to ensure that an existing 
    /// subscription is successfully removed when unsubscribed.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous test execution.
    /// </returns>
    [Fact]
    public async Task Unsubscribe_ExistingSubscription_RemovesSubscription()
    {
        // Arrange
        var phoneNumber = "+1234567890";
        var serviceId = 1;
        await _subscriptionService.Subscribe(phoneNumber, serviceId, 1);

        // Act
        await _subscriptionService.Unsubscribe(phoneNumber, serviceId);

        // Assert
        var summary = await _subscriptionService.GetSubscriptionSummary(phoneNumber);
        Assert.Empty(summary.Subscriptions);
    }
}
