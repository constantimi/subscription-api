using FluentValidation;
using MediatR;
using SubscriptionAPI.Common;
using SubscriptionAPI.Common.Exceptions;
using SubscriptionAPI.Infrastructure.Data;

namespace SubscriptionAPI.Features.Subscriptions.Commands;

public record UnsubscribeCommand(
    string CustomerPhoneNumber,
    int ServiceId) : IRequest<Result<Unit>>;

public class UnsubscribeValidator : AbstractValidator<UnsubscribeCommand>
{
    public UnsubscribeValidator()
    {
        RuleFor(x => x.CustomerPhoneNumber)
            .NotEmpty()
            .Matches(@"^\+[1-9]\d{1,14}$")
            .WithMessage("Phone number must be in E.164 format");

        RuleFor(x => x.ServiceId)
            .GreaterThan(0);
    }
}

/// <summary>
/// Handles the unsubscription process for a given service and customer.
/// </summary>
public class UnsubscribeHandler : IRequestHandler<UnsubscribeCommand, Result<Unit>>
{
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnsubscribeHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to interact with the data layer.</param>
    public UnsubscribeHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Handles the unsubscription command.
    /// </summary>
    /// <param name="request">The unsubscription command containing the service ID and customer phone number.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the unsubscription operation.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown when the specified service is not found.</exception>
    /// <exception cref="SubscriptionNotFoundException">Thrown when the subscription for the specified customer and service is not found.</exception>
    public async Task<Result<Unit>> Handle(
        UnsubscribeCommand request,
        CancellationToken cancellationToken)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(request.ServiceId)
          ?? throw new ServiceNotFoundException(request.ServiceId);

        var subscription = await _unitOfWork.Subscriptions
            .FindAsync(s =>
                s.CustomerPhoneNumber == request.CustomerPhoneNumber &&
                s.ServiceId == request.ServiceId)
            ?? throw new SubscriptionNotFoundException(request.CustomerPhoneNumber, service.Name);

        _unitOfWork.Subscriptions.Remove(subscription);
        await _unitOfWork.SaveChangesAsync();

        return Result<Unit>.Success(Unit.Value);
    }
}