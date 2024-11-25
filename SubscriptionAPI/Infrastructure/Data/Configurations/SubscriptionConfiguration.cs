using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Infrastructure.Data.Configurations;

/// <summary>
/// Configures the entity of type <see cref="Subscription"/>.
/// </summary>
/// <remarks>
/// This configuration class sets up the properties and relationships for the <see cref="Subscription"/> entity.
/// </remarks>
public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    /// <summary>
    /// Configures the <see cref="Subscription"/> entity.
    /// </summary>
    /// <param name="builder">The builder to be used to configure the entity.</param>
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.CustomerPhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.DurationMonths)
            .IsRequired();

        builder.Property(s => s.SubscriptionDate)
            .IsRequired();

        builder.HasOne(s => s.Service)
            .WithMany()
            .HasForeignKey(s => s.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.CustomerPhoneNumber, s.ServiceId })
            .IsUnique();
    }
}