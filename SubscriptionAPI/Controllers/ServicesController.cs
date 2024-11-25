using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubscriptionAPI.Features.Services.Queries;
using SubscriptionAPI.Common;

namespace SubscriptionAPI.Controllers;

/// <summary>
/// Controller for handling service-related operations.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ServicesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ServicesController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServicesController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending queries.</param>
    /// <param name="logger">The logger instance for logging information.</param>
    public ServicesController(
        IMediator mediator,
        ILogger<ServicesController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all services.
    /// </summary>
    /// <returns>A list of services if successful; otherwise, a bad request response.</returns>
    [HttpGet]
    [Idempotency(30)]
    public async Task<IActionResult> GetServices()
    {
        _logger.LogInformation("Retrieving all services");
        var result = await _mediator.Send(new GetServicesQuery());
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Retrieves a service by its ID.
    /// </summary>
    /// <param name="id">The ID of the service to retrieve.</param>
    /// <returns>The service if found; otherwise, a not found response.</returns>
    [HttpGet("{id}")]
    [Idempotency(30)]
    public async Task<IActionResult> GetServiceById(int id)
    {
        _logger.LogInformation("Retrieving service with ID: {ServiceId}", id);
        var result = await _mediator.Send(new GetServiceByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }
}