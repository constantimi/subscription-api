using System.Net;
using SubscriptionAPI.Common.Exceptions;

namespace SubscriptionAPI.Infrastructure.Middleware;

/// <summary>
/// Middleware to handle exceptions that occur during the request processing pipeline.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger to log exception details.</param>
    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to handle exceptions.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the completion of request processing.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    /// <summary>
    /// Handles the exception and writes an appropriate response.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="exception">The exception that occurred.</param>
    /// <returns>A task that represents the completion of exception handling.</returns>
    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ServiceNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            DuplicateSubscriptionException => (StatusCodes.Status409Conflict, exception.Message),
            SubscriptionNotFoundException => (StatusCodes.Status404NotFound, exception.Message),
            InvalidSubscriptionDurationException => (StatusCodes.Status400BadRequest, exception.Message),
            InvalidPhoneNumberException => (StatusCodes.Status400BadRequest, exception.Message),
            SubscriptionException => (StatusCodes.Status400BadRequest, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        var response = new
        {
            statusCode,
            message,
            error = exception.GetType().Name
        };

        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(response);
    }
}