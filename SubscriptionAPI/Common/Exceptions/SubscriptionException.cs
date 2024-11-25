namespace SubscriptionAPI.Common.Exceptions
{
    /// <summary>
    /// Base class for all exceptions related to subscriptions.
    /// </summary>
    public class SubscriptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public SubscriptionException(string message) : base(message) { }
    }

    /// <summary>
    /// Exception thrown when a requested service is not found.
    /// </summary>
    public class ServiceNotFoundException : SubscriptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNotFoundException"/> class with a specified service ID.
        /// </summary>
        /// <param name="serviceId">The ID of the service that was not found.</param>
        public ServiceNotFoundException(int serviceId)
            : base($"Service with ID {serviceId} was not found") { }
    }

    /// <summary>
    /// Exception thrown when a duplicate subscription is detected for a customer.
    /// </summary>
    public class DuplicateSubscriptionException : SubscriptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateSubscriptionException"/> class with the customer's phone number and service name.
        /// </summary>
        /// <param name="customerPhoneNumber">The phone number of the customer who already has a subscription.</param>
        /// <param name="serviceName">The name of the service the customer is already subscribed to.</param>
        public DuplicateSubscriptionException(string customerPhoneNumber, string serviceName)
            : base($"Customer {customerPhoneNumber} is already subscribed to {serviceName}") { }
    }

    /// <summary>
    /// Exception thrown when a subscription for a customer and service is not found.
    /// </summary>
    public class SubscriptionNotFoundException : SubscriptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionNotFoundException"/> class with a specified customer phone number and service name.
        /// </summary>
        /// <param name="customerPhoneNumber">The phone number of the customer.</param>
        /// <param name="serviceName">The name of the service the customer is not subscribed to.</param>
        public SubscriptionNotFoundException(string customerPhoneNumber, string serviceName)
            : base($"Subscription for customer {customerPhoneNumber} to service {serviceName} was not found") { }
    }

    /// <summary>
    /// Exception thrown when an invalid subscription duration is provided.
    /// </summary>
    public class InvalidSubscriptionDurationException : SubscriptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidSubscriptionDurationException"/> class with a specified duration.
        /// </summary>
        /// <param name="duration">The invalid subscription duration.</param>
        public InvalidSubscriptionDurationException(int duration)
            : base($"Invalid subscription duration: {duration}. Duration must be between 1 and 12 months") { }
    }

    /// <summary>
    /// Exception thrown when an invalid phone number format is provided.
    /// </summary>
    public class InvalidPhoneNumberException : SubscriptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidPhoneNumberException"/> class with a specified phone number.
        /// </summary>
        /// <param name="phoneNumber">The invalid phone number that was provided.</param>
        public InvalidPhoneNumberException(string phoneNumber)
            : base($"Invalid phone number format: {phoneNumber}. Phone number must be in E.164 format (e.g., +1234567890)") { }
    }
}
