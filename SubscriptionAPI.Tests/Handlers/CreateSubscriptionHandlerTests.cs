using Xunit;
using Moq;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SubscriptionAPI.Features.Subscriptions.Commands;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Models;
using SubscriptionAPI.Common;
using SubscriptionAPI.Infrastructure.Factories;
using SubscriptionAPI.Common.Exceptions;

namespace SubscriptionAPI.Tests.Handlers;

public class CreateSubscriptionHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ISubscriptionFactory> _subscriptionFactoryMock;
    private readonly CreateSubscriptionHandler _handler;

    public CreateSubscriptionHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _subscriptionFactoryMock = new Mock<ISubscriptionFactory>();
        _handler = new CreateSubscriptionHandler(_unitOfWorkMock.Object, _subscriptionFactoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateSubscription_WhenValidRequest()
    {
        // Arrange
        var request = new CreateSubscriptionCommand("+1234567890", 1, 6);
        var service = new Service { Id = 1, Name = "Test Service" };
        var subscription = new Subscription { CustomerPhoneNumber = request.CustomerPhoneNumber, Service = service };

        _unitOfWorkMock.Setup(u => u.ServicesRepository.GetByIdAsync(request.ServiceId))
            .ReturnsAsync(service);
        _unitOfWorkMock.Setup(u => u.SubscriptionsRepository.FindAsync(It.IsAny<Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync((Subscription?)null);
        _subscriptionFactoryMock.Setup(f => f.Create(request.CustomerPhoneNumber, service, request.DurationMonths))
            .Returns(subscription);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SubscriptionsRepository.AddAsync(subscription), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowServiceNotFoundException_WhenServiceDoesNotExist()
    {
        // Arrange
        var request = new CreateSubscriptionCommand("+1234567890", 999, 6);
        _unitOfWorkMock.Setup(u => u.ServicesRepository.GetByIdAsync(request.ServiceId))
            .ReturnsAsync((Service?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ServiceNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }
}
