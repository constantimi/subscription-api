using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MediatR;
using Moq;
using SubscriptionAPI.Controllers;
using SubscriptionAPI.Features.Services.Queries;
using SubscriptionAPI.Models;
using SubscriptionAPI.Common;
using Xunit;

namespace SubscriptionAPI.Tests.Services;

public class ServicesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ServicesController>> _loggerMock;
    private readonly ServicesController _controller;

    public ServicesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ServicesController>>();
        _controller = new ServicesController(_mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetServices_ReturnsOkResult_WhenServicesExist()
    {
        // Arrange
        var services = new List<Service>
        {
            new() { Id = 1, Name = "Test Service", MonthlyPrice = 10 }
        };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServicesQuery>(), default))
            .ReturnsAsync(Result<IEnumerable<Service>>.Success(services));

        // Act
        var result = await _controller.GetServices();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedServices = Assert.IsAssignableFrom<IEnumerable<Service>>(okResult.Value);
        Assert.Single(returnedServices);
    }

    [Fact]
    public async Task GetServiceById_ReturnsOkResult_WhenServiceExists()
    {
        // Arrange
        var service = new Service { Id = 1, Name = "Test Service", MonthlyPrice = 10 };
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServiceByIdQuery>(), default))
            .ReturnsAsync(Result<Service>.Success(service));

        // Act
        var result = await _controller.GetServiceById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedService = Assert.IsType<Service>(okResult.Value);
        Assert.Equal(1, returnedService.Id);
    }

    [Fact]
    public async Task GetServiceById_ReturnsNotFound_WhenServiceDoesNotExist()
    {
        // Arrange
        _mediatorMock.Setup(m => m.Send(It.IsAny<GetServiceByIdQuery>(), default))
            .ReturnsAsync(Result<Service>.Failure("Service not found"));

        // Act
        var result = await _controller.GetServiceById(999);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}