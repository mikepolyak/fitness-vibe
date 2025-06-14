using FitnessVibe.Application.Commands.Activities;
using FitnessVibe.Application.Exceptions;
using FitnessVibe.Application.Handlers.Activities;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Enums;
using FitnessVibe.Domain.Repositories;
using FluentAssertions;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FitnessVibe.Application.Tests.Handlers.Activities;

public class AddRoutePointCommandHandlerTests
{
    private readonly Mock<IActivityRepository> _activityRepositoryMock;
    private readonly AddRoutePointCommandHandler _handler;

    public AddRoutePointCommandHandlerTests()
    {
        _activityRepositoryMock = new Mock<IActivityRepository>();
        _handler = new AddRoutePointCommandHandler(_activityRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCommand_ShouldAddRoutePoint()
    {
        // Arrange
        var activityId = Guid.NewGuid();
        var activity = new Activity
        {
            Id = activityId,
            Status = ActivityStatus.InProgress
        };

        var command = new AddRoutePointCommand
        {
            ActivityId = activityId,
            Latitude = 40.7128,
            Longitude = -74.0060,
            Elevation = 10,
            Timestamp = DateTime.UtcNow
        };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId))
            .ReturnsAsync(activity);

        _activityRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Activity>()))
            .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _activityRepositoryMock.Verify(x => x.UpdateAsync(It.Is<Activity>(a => 
            a.Id == activityId &&
            a.Route.Points.Count == 1 &&
            a.Route.Points[0].Latitude == command.Latitude &&
            a.Route.Points[0].Longitude == command.Longitude &&
            a.Route.Points[0].Elevation == command.Elevation &&
            a.Route.Points[0].Timestamp == command.Timestamp)), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentActivity_ShouldThrowNotFoundException()
    {
        // Arrange
        var activityId = Guid.NewGuid();
        var command = new AddRoutePointCommand { ActivityId = activityId };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId))
            .ReturnsAsync((Activity)null);

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Activity with ID {activityId} not found");
    }

    [Fact]
    public async Task Handle_WithCompletedActivity_ShouldThrowUnauthorizedOperationException()
    {
        // Arrange
        var activityId = Guid.NewGuid();
        var activity = new Activity
        {
            Id = activityId,
            Status = ActivityStatus.Completed
        };

        var command = new AddRoutePointCommand { ActivityId = activityId };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId))
            .ReturnsAsync(activity);

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<UnauthorizedOperationException>()
            .WithMessage("Cannot add route points to a completed activity");
    }
}
