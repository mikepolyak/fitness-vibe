using AutoMapper;
using FitnessVibe.Application.DTOs.Activities;
using FitnessVibe.Application.Exceptions;
using FitnessVibe.Application.Handlers.Activities;
using FitnessVibe.Application.Queries.Activities;
using FitnessVibe.Domain.Entities.Activities;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace FitnessVibe.Application.Tests.Handlers.Activities;

public class GetActivityRouteQueryHandlerTests
{
    private readonly Mock<IActivityRepository> _activityRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetActivityRouteQueryHandler _handler;

    public GetActivityRouteQueryHandlerTests()
    {
        _activityRepositoryMock = new Mock<IActivityRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetActivityRouteQueryHandler(_activityRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidQuery_ShouldReturnActivityRouteDto()
    {
        // Arrange
        var activityId = Guid.NewGuid();
        var route = new ActivityRoute
        {
            Points = new List<GpsPoint>
            {
                new GpsPoint(40.7128, -74.0060, 10, DateTime.UtcNow)
            }
        };

        var activity = new Activity
        {
            Id = activityId,
            Route = route
        };

        var expectedDto = new ActivityRouteDto
        {
            Points = new List<GpsPointDto>
            {
                new GpsPointDto { Latitude = 40.7128, Longitude = -74.0060, Elevation = 10 }
            }
        };

        var query = new GetActivityRouteQuery { ActivityId = activityId };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId))
            .ReturnsAsync(activity);

        _mapperMock.Setup(x => x.Map<ActivityRouteDto>(route))
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
        _mapperMock.Verify(x => x.Map<ActivityRouteDto>(route), Times.Once);
    }

    [Fact]
    public async Task Handle_WithNonExistentActivity_ShouldThrowNotFoundException()
    {
        // Arrange
        var activityId = Guid.NewGuid();
        var query = new GetActivityRouteQuery { ActivityId = activityId };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId))
            .ReturnsAsync((Activity)null);

        // Act
        var action = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Activity with ID {activityId} not found");
    }

    [Fact]
    public async Task Handle_WithActivityWithoutRoute_ShouldReturnEmptyRoute()
    {
        // Arrange
        var activityId = Guid.NewGuid();
        var activity = new Activity { Id = activityId };
        var query = new GetActivityRouteQuery { ActivityId = activityId };

        _activityRepositoryMock.Setup(x => x.GetByIdAsync(activityId))
            .ReturnsAsync(activity);

        var emptyRouteDto = new ActivityRouteDto { Points = new List<GpsPointDto>() };
        _mapperMock.Setup(x => x.Map<ActivityRouteDto>(It.IsAny<ActivityRoute>()))
            .Returns(emptyRouteDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Points.Should().BeEmpty();
    }
}
