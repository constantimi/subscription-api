namespace SubscriptionAPI.Services;

/// <summary>
/// Provides methods to handle idempotency keys for ensuring that operations are not performed more than once.
/// </summary>
public interface IIdempotencyService
{
    /// <summary>
    /// Generates a new idempotency key.
    /// </summary>
    /// <returns>A unique idempotency key as a string.</returns>
    string GenerateIdempotencyKey();

    /// <summary>
    /// Checks if the specified idempotency key has already been used.
    /// </summary>
    /// <param name="key">The idempotency key to check.</param>
    /// <returns>True if the key has been used; otherwise, false.</returns>
    bool IsKeyUsed(string key);

    /// <summary>
    /// Marks the specified idempotency key as used and sets an expiration time for it.
    /// </summary>
    /// <param name="key">The idempotency key to mark as used.</param>
    /// <param name="expirationTime">The time span after which the key will expire.</param>
    void MarkKeyAsUsed(string key, TimeSpan expirationTime);
}