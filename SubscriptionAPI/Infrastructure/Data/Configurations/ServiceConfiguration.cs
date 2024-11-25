using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the entity of type <see cref="Service"/>.
/// </summary>
/// <remarks>
/// This configuration class sets up the primary key, properties, and indexes for the <see cref="Service"/> entity.
/// </remarks>
public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    /// <summary>
    /// Configures the <see cref="Service"/> entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.MonthlyPrice)
            .HasPrecision(18, 2);

        builder.HasIndex(s => s.Name)
            .IsUnique();
    }
}