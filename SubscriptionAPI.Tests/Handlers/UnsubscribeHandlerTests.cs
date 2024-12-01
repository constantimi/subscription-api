using Xunit;
using Moq;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SubscriptionAPI.Features.Subscriptions.Commands;
using SubscriptionAPI.Common.Exceptions;
using SubscriptionAPI.Infrastructure.Data;
using SubscriptionAPI.Models;
using SubscriptionAPI.Common;

namespace SubscriptionAPI.Tests.Handlers;

public class UnsubscribeHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UnsubscribeHandler _handler;

    public UnsubscribeHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UnsubscribeHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldRemoveSubscription_WhenValidRequest()
    {
        // Arrange
        var request = new UnsubscribeCommand("+1234567890", 1);
        var service = new Service { Id = 1, Name = "Test Service" };
        var subscription = new Subscription { CustomerPhoneNumber = request.CustomerPhoneNumber, Service = service };

        _unitOfWorkMock.Setup(u => u.ServicesRepository.GetByIdAsync(request.ServiceId)).ReturnsAsync(service);
        _unitOfWorkMock.Setup(u => u.SubscriptionsRepository.FindAsync(It.IsAny<Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync(subscription);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SubscriptionsRepository.Remove(subscription), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowSubscriptionNotFoundException_WhenSubscriptionDoesNotExist()
    {
        // Arrange
        var request = new UnsubscribeCommand("+1234567890", 1);
        var service = new Service { Id = 1, Name = "Test Service" };

        _unitOfWorkMock.Setup(u => u.ServicesRepository.GetByIdAsync(request.ServiceId)).ReturnsAsync(service);
        _unitOfWorkMock.Setup(u => u.SubscriptionsRepository.FindAsync(It.IsAny<Expression<Func<Subscription, bool>>>()))
            .ReturnsAsync((Subscription?)null);

        // Act & Assert
        await Assert.ThrowsAsync<SubscriptionNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }
}
