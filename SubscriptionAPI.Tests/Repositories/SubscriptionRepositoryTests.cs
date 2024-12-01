using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using SubscriptionAPI.Infrastructure.Data.Repositories;
using SubscriptionAPI.Models;
using SubscriptionAPI.Infrastructure.Data;
using System;
using System.Linq.Expressions;
using FluentAssertions;

namespace SubscriptionAPI.Tests.Repositories;

public class SubscriptionRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly SubscriptionRepository _repository;

    public SubscriptionRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;

        _context = new AppDbContext(options);
        _repository = new SubscriptionRepository(_context);

        // Seed initial data
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        var service = new Service { Id = 1, Name = "Test Service" };
        _context.Services.Add(service);

        _context.Subscriptions.AddRange(
            new Subscription { Id = 1, CustomerPhoneNumber = "+1234567890", ServiceId = 1, DurationMonths = 6 },
            new Subscription { Id = 2, CustomerPhoneNumber = "+9876543210", ServiceId = 1, DurationMonths = 12 }
        );

        _context.SaveChanges();
    }

    [Fact]
    public async Task FindAllAsync_ShouldReturnFilteredSubscriptions_WhenPredicateIsProvided()
    {
        // Arrange
        Expression<Func<Subscription, bool>> predicate = s => s.CustomerPhoneNumber == "+1234567890";

        // Act
        var result = await _repository.FindAllAsync(predicate);

        // Assert
        result.Should().HaveCount(1);
        result.First().CustomerPhoneNumber.Should().Be("+1234567890");
    }

    [Fact]
    public async Task FindAllAsync_ShouldIncludeServiceEntity_WhenQuerying()
    {
        // Arrange
        Expression<Func<Subscription, bool>> predicate = s => s.ServiceId == 1;

        // Act
        var result = await _repository.FindAllAsync(predicate);

        // Assert
        result.Should().AllSatisfy(sub =>
        {
            sub.Service.Should().NotBeNull();
            sub.Service?.Name.Should().Be("Test Service");
        });
    }

    [Fact]
    public async Task AddAsync_ShouldAddSubscription_ToDatabase()
    {
        // Arrange
        var newSubscription = new Subscription
        {
            CustomerPhoneNumber = "+1122334455",
            ServiceId = 1,
            DurationMonths = 3
        };

        // Act
        await _repository.AddAsync(newSubscription);
        await _context.SaveChangesAsync();

        // Assert
        var addedSubscription = await _context.Subscriptions.FindAsync(newSubscription.Id);
        addedSubscription.Should().NotBeNull();
        addedSubscription.Should().NotBeNull();
        addedSubscription!.CustomerPhoneNumber.Should().Be("+1122334455");
    }

    [Fact]
    public async Task Remove_ShouldDeleteSubscription_FromDatabase()
    {
        // Arrange
        var subscription = await _context.Subscriptions.FirstAsync();

        // Act
        _repository.Remove(subscription);
        await _context.SaveChangesAsync();

        // Assert
        var deletedSubscription = await _context.Subscriptions.FindAsync(subscription.Id);
        deletedSubscription.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
