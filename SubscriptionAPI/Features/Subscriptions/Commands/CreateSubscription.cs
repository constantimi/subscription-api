using FluentValidation;
using MediatR;
using SubscriptionAPI.Common;
using SubscriptionAPI.Common.Exceptions;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Infrastructure.Factories;
using SubscriptionAPI.Models;

namespace SubscriptionAPI.Features.Subscriptions.Commands;

public record CreateSubscriptionCommand(
    string CustomerPhoneNumber,
    int ServiceId,
    int DurationMonths) : IRequest<Result<Subscription>>;

public class CreateSubscriptionValidator : AbstractValidator<CreateSubscriptionCommand>
{
    public CreateSubscriptionValidator()
    {
        RuleFor(x => x.CustomerPhoneNumber)
            .NotEmpty()
            .Matches(@"^\+[1-9]\d{1,14}$")
            .WithMessage("Phone number must be in E.164 format");

        RuleFor(x => x.ServiceId)
            .GreaterThan(0);

        RuleFor(x => x.DurationMonths)
            .GreaterThan(0)
            .LessThanOrEqualTo(12);
    }
}

/// <summary>
/// Handles the creation of a new subscription.
/// </summary>
public class CreateSubscriptionHandler : IRequestHandler<CreateSubscriptionCommand, Result<Subscription>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISubscriptionFactory _subscriptionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSubscriptionHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="subscriptionFactory">The subscription factory.</param>
    public CreateSubscriptionHandler(
        IUnitOfWork unitOfWork,
        ISubscriptionFactory subscriptionFactory)
    {
        _unitOfWork = unitOfWork;
        _subscriptionFactory = subscriptionFactory;
    }

    /// <summary>
    /// Handles the creation of a new subscription.
    /// </summary>
    /// <param name="request">The create subscription command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the result of the subscription creation.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown when the service with the specified ID is not found.</exception>
    /// <exception cref="DuplicateSubscriptionException">Thrown when a duplicate subscription is found.</exception>
    public async Task<Result<Subscription>> Handle(
        CreateSubscriptionCommand request,
        CancellationToken cancellationToken)
    {

        var service = await _unitOfWork.Services.GetByIdAsync(request.ServiceId) ?? throw new ServiceNotFoundException(request.ServiceId);

        var existingSubscription = await _unitOfWork.Subscriptions
            .FindAsync(s =>
                s.CustomerPhoneNumber == request.CustomerPhoneNumber &&
                s.ServiceId == request.ServiceId);

        if (existingSubscription != null)
            throw new DuplicateSubscriptionException(request.CustomerPhoneNumber, service.Name);

        var subscription = _subscriptionFactory.Create(
            request.CustomerPhoneNumber,
            service,
            request.DurationMonths);

        await _unitOfWork.Subscriptions.AddAsync(subscription);
        await _unitOfWork.SaveChangesAsync();

        return Result<Subscription>.Success(subscription);
    }
}