using SubscriptionAPI.Models;

namespace SubscriptionAPI.Infrastructure.Data;

/// <summary>
/// Provides methods to initialize the database with default data.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Initializes the database with default services if no services exist.
    /// </summary>
    /// <param name="context">The database context to be initialized.</param>
    public static void Initialize(SubscriptionAPI.Infrastructure.Data.AppDbContext context)
    {
        if (context.Services.Any())
            return;

        var services = new Service[]
        {
            new() { Id = 1, Name = "eLearning Portal", MonthlyPrice = 10 },
            new() { Id = 2, Name = "Health&Lifestyle", MonthlyPrice = 12 },
            new() { Id = 3, Name = "Gaming+ Catalogue", MonthlyPrice = 15 },
            new() { Id = 4, Name = "Magazines and News", MonthlyPrice = 8 }
        };

        context.Services.AddRange(services);
        context.SaveChanges();
    }
}