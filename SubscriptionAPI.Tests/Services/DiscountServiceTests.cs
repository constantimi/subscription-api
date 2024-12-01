using Microsoft.EntityFrameworkCore;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Models;
using SubscriptionAPI.Services;
using Xunit;

namespace SubscriptionAPI.Tests.Services;

public class DiscountServiceTests
{
    private readonly AppDbContext _context;
    private readonly IDiscountService _discountService;

    public DiscountServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _discountService = new DiscountService(_context);
        DbInitializer.Initialize(_context);
    }

    [Fact]
    public async Task CalculateDiscounts_ServicePairPromotion_AppliesCorrectDiscount()
    {
        // Arrange
        var subscriptions = new List<Subscription>
        {
            new() {
                Service = new Service { Name = "Health&Lifestyle", MonthlyPrice = 12 },
                DurationMonths = 1
            },
            new() {
                Service = new Service { Name = "Magazines and News", MonthlyPrice = 8 },
                DurationMonths = 1
            }
        };

        // Act
        var discounts = await _discountService.CalculateDiscounts(subscriptions);

        // Assert
        var pairDiscount = discounts.FirstOrDefault(d => d.DiscountName.Contains("Service Pair"));
        Assert.NotNull(pairDiscount);
        Assert.Equal(12m, pairDiscount.Amount); // One month of Health&Lifestyle free
    }

    [Fact]
    public async Task CalculateDiscounts_QuantityBasedDiscount_AppliesCorrectDiscount()
    {
        // Arrange
        var subscriptions = new List<Subscription>
        {
            new() {
                Service = new Service { Name = "Health&Lifestyle", MonthlyPrice = 12 },
                DurationMonths = 1
            },
            new() {
                Service = new Service { Name = "Gaming+ Catalogue", MonthlyPrice = 15 },
                DurationMonths = 1
            },
            new() {
                Service = new Service { Name = "eLearning Portal", MonthlyPrice = 10 },
                DurationMonths = 1
            }
        };

        // Act
        var discounts = await _discountService.CalculateDiscounts(subscriptions);

        // Assert
        var quantityDiscount = discounts.FirstOrDefault(d => d.DiscountName.Contains("Quantity"));
        Assert.NotNull(quantityDiscount);
        Assert.Equal(3.70m, quantityDiscount.Amount); // 10% of (12 + 15 + 10)
    }

    [Fact]
    public async Task CalculateDiscounts_BundledDiscount_AppliesCorrectDiscount()
    {
        // Arrange
        var subscriptions = new List<Subscription>
        {
            new() {
                Service = new Service { Name = "Gaming+ Catalogue", MonthlyPrice = 15 },
                DurationMonths = 1
            },
            new() {
                Service = new Service { Name = "eLearning Portal", MonthlyPrice = 10 },
                DurationMonths = 1
            }
        };

        // Act
        var discounts = await _discountService.CalculateDiscounts(subscriptions);

        // Assert
        var bundleDiscount = discounts.FirstOrDefault(d => d.DiscountName.Contains("Bundle"));
        Assert.NotNull(bundleDiscount);
        Assert.Equal(5m, bundleDiscount.Amount);
    }

    [Fact]
    public async Task CalculateDiscounts_UpfrontBonus_AppliesCorrectDiscount()
    {
        // Arrange
        var subscriptions = new List<Subscription>
        {
            new() {
                Service = new Service { Name = "Gaming+ Catalogue", MonthlyPrice = 15 },
                DurationMonths = 5
            }
        };

        // Act
        var discounts = await _discountService.CalculateDiscounts(subscriptions);

        // Assert
        var upfrontDiscount = discounts.FirstOrDefault(d => d.DiscountName.Contains("Upfront"));
        Assert.NotNull(upfrontDiscount);
        Assert.Equal(15m, upfrontDiscount.Amount); // One month free
    }

    [Fact]
    public async Task CalculateDiscounts_MultipleDiscounts_AppliesAllCorrectly()
    {
        // Arrange
        var subscriptions = new List<Subscription>
        {
            new() {
                Service = new Service { Name = "Gaming+ Catalogue", MonthlyPrice = 15 },
                DurationMonths = 5
            },
            new() {
                Service = new Service { Name = "eLearning Portal", MonthlyPrice = 10 },
                DurationMonths = 5
            },
            new() {
                Service = new Service { Name = "Magazines and News", MonthlyPrice = 8 },
                DurationMonths = 5
            }
        };

        // Act
        var discounts = await _discountService.CalculateDiscounts(subscriptions);

        // Assert
        Assert.Contains(discounts, d => d.DiscountName.Contains("Bundle")); // Bundle discount
        Assert.Contains(discounts, d => d.DiscountName.Contains("Quantity")); // 3+ services
        Assert.Contains(discounts, d => d.DiscountName.Contains("Upfront")); // 5+ months subscriptions
    }

    [Fact]
    public async Task CalculateDiscounts_NoSubscriptions_ReturnsEmptyList()
    {
        // Arrange
        var subscriptions = new List<Subscription>();

        // Act
        var discounts = await _discountService.CalculateDiscounts(subscriptions);

        // Assert
        Assert.Empty(discounts);
    }
}