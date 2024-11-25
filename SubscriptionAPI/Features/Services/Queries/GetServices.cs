using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionAPI.Common;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Features.Services.Queries;

public record GetServicesQuery : IRequest<Result<IEnumerable<Service>>>;

/// <summary>
/// Handles the retrieval of services.
/// </summary>
public class GetServicesHandler : IRequestHandler<GetServicesQuery, Result<IEnumerable<Service>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetServicesHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetServicesHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="logger">The logger.</param>
    public GetServicesHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetServicesHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the request to get all services.
    /// </summary>
    /// <param name="request">The request to get services.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> with the list of services.</returns>
    public async Task<Result<IEnumerable<Service>>> Handle(
        GetServicesQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var services = await _unitOfWork.Services.GetAllAsync();
            return Result<IEnumerable<Service>>.Success(services);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving services");
            return Result<IEnumerable<Service>>.Failure("Failed to retrieve services");
        }
    }
}