using MediatR;
using SubscriptionAPI.Common;
using SubscriptionAPI.Common.Exceptions;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Features.Services.Queries;

public record GetServiceByIdQuery(int Id) : IRequest<Result<Service>>;

/// <summary>
/// Handles the retrieval of a service by its ID.
/// </summary>
public class GetServiceByIdHandler : IRequestHandler<GetServiceByIdQuery, Result<Service>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetServiceByIdHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetServiceByIdHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="logger">The logger.</param>
    public GetServiceByIdHandler(
        IUnitOfWork unitOfWork,
        ILogger<GetServiceByIdHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Handles the request to retrieve a service by its ID.
    /// </summary>
    /// <param name="request">The request containing the service ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the service retrieval.</returns>
    public async Task<Result<Service>> Handle(
        GetServiceByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var service = await _unitOfWork.Services.GetByIdAsync(request.Id);

            if (service == null)
                throw new ServiceNotFoundException(request.Id);

            return Result<Service>.Success(service);
        }
        catch (ServiceNotFoundException ex)
        {
            _logger.LogWarning(ex, "Service not found: {ServiceId}", request.Id);
            return Result<Service>.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving service: {ServiceId}", request.Id);
            return Result<Service>.Failure("Failed to retrieve service");
        }
    }
}