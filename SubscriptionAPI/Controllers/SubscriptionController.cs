using MediatR;
using Microsoft.AspNetCore.Mvc;
using SubscriptionAPI.Common;
using SubscriptionAPI.Features.Subscriptions.Commands;
using SubscriptionAPI.Features.Subscriptions.Queries;

namespace SubscriptionAPI.Controllers;

/// <summary>
/// Controller for handling subscription-related operations.
/// </summary>
[ApiController]
[Route("[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubscriptionController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionController"/> class.
    /// </summary>
    /// <param name="mediator">The mediator instance for sending commands and queries.</param>
    /// <param name="logger">The logger instance for logging information.</param>
    public SubscriptionController(IMediator mediator, ILogger<SubscriptionController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Subscribes a customer to a service.
    /// </summary>
    /// <param name="command">The command containing subscription details.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [HttpPost("subscribe")]
    [Idempotency(30)]
    public async Task<IActionResult> Subscribe([FromBody] CreateSubscriptionCommand command)
    {
        _logger.LogInformation(
            "Processing subscription request for customer {CustomerPhone} to service {ServiceId}",
            command.CustomerPhoneNumber,
            command.ServiceId);

        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    /// <summary>
    /// Unsubscribes a customer from a service.
    /// </summary>
    /// <param name="command">The command containing unsubscription details.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [HttpPost("unsubscribe")]
    [Idempotency(30)]
    public async Task<IActionResult> Unsubscribe([FromBody] UnsubscribeCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok() : NotFound(result.Error);
    }

    /// <summary>
    /// Gets the subscription summary for a customer.
    /// </summary>
    /// <param name="customerPhoneNumber">The phone number of the customer.</param>
    /// <returns>An <see cref="IActionResult"/> containing the subscription summary.</returns>
    [HttpGet("subscription-summary")]
    public async Task<IActionResult> GetSubscriptionSummary([FromQuery] string customerPhoneNumber)
    {
        var query = new GetSubscriptionSummaryQuery(customerPhoneNumber);
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}