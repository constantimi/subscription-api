using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using SubscriptionAPI.Common;

namespace SubscriptionAPI.Infrastructure.Middleware;

/// <summary>
/// Middleware to handle idempotent requests. Ensures that identical requests with the same
/// idempotency key are processed only once, and subsequent identical requests return the cached response.
/// </summary>
public class IdempotencyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _cache;
    private readonly ILogger<IdempotencyMiddleware> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="IdempotencyMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="cache">The distributed cache to store idempotent responses.</param>
    /// <param name="logger">The logger to log information and errors.</param>
    public IdempotencyMiddleware(
        RequestDelegate next,
        IDistributedCache cache,
        ILogger<IdempotencyMiddleware> logger)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware to process the HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context of the current request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var endpoint = context.GetEndpoint();
        var idempotencyAttribute = endpoint?.Metadata
            .GetMetadata<IdempotencyAttribute>();

        if (idempotencyAttribute == null)
        {
            await _next(context);
            return;
        }

        var idempotencyKey = context.Request.Headers["Idempotency-Key"].ToString();

        if (string.IsNullOrEmpty(idempotencyKey))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new { error = "Missing Idempotency-Key header" });
            return;
        }

        try
        {
            // Read the request body and create a unique key based on the endpoint and body
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var requestBody = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            var endpoint_path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
            var compositeKey = $"{endpoint_path}:{idempotencyKey}:{requestBody}";

            _logger.LogDebug("Processing request with idempotency key: {Key}", compositeKey);

            var cachedResponse = await _cache.GetStringAsync(compositeKey);

            if (cachedResponse != null)
            {
                _logger.LogInformation("Retrieved cached response for key: {Key}", compositeKey);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(cachedResponse);
                return;
            }

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            responseBody.Seek(0, SeekOrigin.Begin);
            var response = await new StreamReader(responseBody).ReadToEndAsync();
            responseBody.Seek(0, SeekOrigin.Begin);

            if (context.Response.StatusCode == StatusCodes.Status200OK)
            {
                await _cache.SetStringAsync(
                    compositeKey,
                    response,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = idempotencyAttribute.ExpirationTime
                    });

                _logger.LogDebug("Cached response for key: {Key}", compositeKey);
            }

            await responseBody.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing idempotent request");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "An error occurred processing the request" });
        }
        finally
        {
            context.Request.Body.Position = 0;
        }
    }
}