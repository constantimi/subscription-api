namespace SubscriptionAPI.Common
{
    /// <summary>
    /// Represents the result of an operation, including whether the operation was successful,
    /// the resulting value (if successful), and an error message (if failed).
    /// </summary>
    /// <typeparam name="T">The type of the value returned when the operation is successful.</typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Gets a value indicating whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Gets the value returned by the operation if it was successful.
        /// Returns <c>null</c> if the operation failed.
        /// </summary>
        public T? Value { get; }

        /// <summary>
        /// Gets the error message if the operation failed.
        /// Returns an empty string if the operation was successful.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Result{T}"/> class.
        /// This constructor is used internally to create success or failure results.
        /// </summary>
        /// <param name="isSuccess">Indicates whether the operation was successful.</param>
        /// <param name="value">The value returned if the operation was successful. <c>null</c> if the operation failed.</param>
        /// <param name="error">The error message if the operation failed. An empty string if successful.</param>
        protected Result(bool isSuccess, T? value, string error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        /// <summary>
        /// Creates a successful result with the given value.
        /// </summary>
        /// <param name="value">The value returned by the successful operation.</param>
        /// <returns>A <see cref="Result{T}"/> representing a successful operation.</returns>
        public static Result<T> Success(T value)
        {
            return new Result<T>(true, value, string.Empty);
        }

        /// <summary>
        /// Creates a failure result with the given error message.
        /// </summary>
        /// <param name="error">The error message explaining why the operation failed.</param>
        /// <returns>A <see cref="Result{T}"/> representing a failed operation.</returns>
        public static Result<T> Failure(string error)
        {
            return new Result<T>(false, default, error);
        }
    }
}
