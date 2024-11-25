using FluentValidation;
using MediatR;
using SubscriptionAPI.Common;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Models;
using SubscriptionAPI.Services;

namespace SubscriptionAPI.Features.Subscriptions.Queries;

public record GetSubscriptionSummaryQuery(string CustomerPhoneNumber)
    : IRequest<Result<SubscriptionSummary>>;

public class GetSubscriptionSummaryValidator : AbstractValidator<GetSubscriptionSummaryQuery>
{
    public GetSubscriptionSummaryValidator()
    {
        RuleFor(x => x.CustomerPhoneNumber)
            .NotEmpty()
            .Matches(@"^\+[1-9]\d{1,14}$")
            .WithMessage("Phone number must be in E.164 format");
    }
}

/// <summary>
/// Handles the GetSubscriptionSummaryQuery to retrieve a summary of subscriptions for a customer.
/// </summary>
public class GetSubscriptionSummaryHandler : IRequestHandler<GetSubscriptionSummaryQuery, Result<SubscriptionSummary>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDiscountService _discountService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSubscriptionSummaryHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to access the subscriptions repository.</param>
    /// <param name="discountService">The discount service to calculate applicable discounts.</param>
    public GetSubscriptionSummaryHandler(IUnitOfWork unitOfWork, IDiscountService discountService)
    {
        _unitOfWork = unitOfWork;
        _discountService = discountService;
    }

    /// <summary>
    /// Handles the request to get the subscription summary.
    /// </summary>
    /// <param name="request">The request containing the customer phone number.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription summary.</returns>
    public async Task<Result<SubscriptionSummary>> Handle(GetSubscriptionSummaryQuery request, CancellationToken cancellationToken)
    {
        var subscriptions = await _unitOfWork.Subscriptions
           .FindAllAsync(s => s.CustomerPhoneNumber == request.CustomerPhoneNumber);

        var summary = new SubscriptionSummary
        {
            CustomerPhoneNumber = request.CustomerPhoneNumber,
            Subscriptions = subscriptions.Select(s => new SubscriptionDetail
            {
                ServiceName = s.Service!.Name,
                DurationMonths = s.DurationMonths,
                MonthlyPrice = s.Service.MonthlyPrice
            }).ToList()
        };

        var discounts = await _discountService.CalculateDiscounts(subscriptions.ToList());

        summary.TotalCostBeforeDiscounts = subscriptions
            .Sum(s => s.Service!.MonthlyPrice * s.DurationMonths);
        summary.AppliedDiscounts = discounts;
        summary.TotalDiscounts = discounts.Sum(d => d.Amount);
        summary.FinalCost = summary.TotalCostBeforeDiscounts - summary.TotalDiscounts;

        return Result<SubscriptionSummary>.Success(summary);
    }
}