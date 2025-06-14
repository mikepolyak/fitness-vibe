using FitnessVibe.Application.Commands.Activities;
using FitnessVibe.Application.Validators.Activities;
using FluentValidation.TestHelper;
using System;
using Xunit;

namespace FitnessVibe.Application.Tests.Validators.Activities;

public class AddRoutePointCommandValidatorTests
{
    private readonly AddRoutePointCommandValidator _validator;

    public AddRoutePointCommandValidatorTests()
    {
        _validator = new AddRoutePointCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldHaveNoValidationErrors()
    {
        // Arrange
        var command = new AddRoutePointCommand
        {
            ActivityId = Guid.NewGuid(),
            Latitude = 40.7128,
            Longitude = -74.0060,
            Elevation = 10,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Validate_WithInvalidLatitude_ShouldHaveValidationError(double latitude)
    {
        // Arrange
        var command = new AddRoutePointCommand
        {
            ActivityId = Guid.NewGuid(),
            Latitude = latitude,
            Longitude = -74.0060,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Validate_WithInvalidLongitude_ShouldHaveValidationError(double longitude)
    {
        // Arrange
        var command = new AddRoutePointCommand
        {
            ActivityId = Guid.NewGuid(),
            Latitude = 40.7128,
            Longitude = longitude,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Longitude);
    }

    [Theory]
    [InlineData(-501)]
    [InlineData(10001)]
    public void Validate_WithInvalidElevation_ShouldHaveValidationError(double elevation)
    {
        // Arrange
        var command = new AddRoutePointCommand
        {
            ActivityId = Guid.NewGuid(),
            Latitude = 40.7128,
            Longitude = -74.0060,
            Elevation = elevation,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Elevation);
    }

    [Fact]
    public void Validate_WithEmptyActivityId_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddRoutePointCommand
        {
            ActivityId = Guid.Empty,
            Latitude = 40.7128,
            Longitude = -74.0060,
            Timestamp = DateTime.UtcNow
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ActivityId);
    }

    [Fact]
    public void Validate_WithFutureTimestamp_ShouldHaveValidationError()
    {
        // Arrange
        var command = new AddRoutePointCommand
        {
            ActivityId = Guid.NewGuid(),
            Latitude = 40.7128,
            Longitude = -74.0060,
            Timestamp = DateTime.UtcNow.AddDays(1)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Timestamp);
    }
}
