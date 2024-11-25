using System.Text.RegularExpressions;

namespace SubscriptionAPI.Common
{
    /// <summary>
    /// Custom attribute to define an idempotency key for HTTP methods.
    /// This attribute can be applied to controller actions to indicate that 
    /// a unique idempotency key is required for that action.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IdempotencyAttribute : Attribute
    {
        /// <summary>
        /// Gets the expiration time of the idempotency key.
        /// </summary>
        public TimeSpan ExpirationTime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdempotencyAttribute"/> class.
        /// The default expiration time is set to 30 minutes.
        /// </summary>
        /// <param name="minutes">The expiration time in minutes. Defaults to 30 minutes if not provided.</param>
        public IdempotencyAttribute(int minutes = 30)
        {
            ExpirationTime = TimeSpan.FromMinutes(minutes);
        }
    }

    /// <summary>
    /// Represents a unique Idempotency key used for ensuring idempotent requests.
    /// This key is used to prevent repeated submissions of the same request within
    /// a short period of time.
    /// </summary>
    public class IdempotencyKey
    {
        private const string KeyPattern = @"^[A-Za-z0-9\-_]+$";

        /// <summary>
        /// Gets the value of the Idempotency key.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IdempotencyKey"/> class with the specified value.
        /// The value must follow a specific pattern (alphanumeric characters, hyphens, and underscores).
        /// </summary>
        /// <param name="value">The idempotency key value to be validated and set.</param>
        /// <exception cref="ArgumentException">Thrown when the value is null, empty, or does not match the required format.</exception>
        public IdempotencyKey(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Idempotency key cannot be empty", nameof(value));

            if (!Regex.IsMatch(value, KeyPattern))
                throw new ArgumentException("Invalid idempotency key format", nameof(value));

            Value = value;
        }
    }
}
