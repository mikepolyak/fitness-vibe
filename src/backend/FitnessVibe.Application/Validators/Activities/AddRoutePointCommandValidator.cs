using FluentValidation;
using FitnessVibe.Application.Commands.Activities;

namespace FitnessVibe.Application.Validators.Activities;

/// <summary>
/// Validator for the AddRoutePointCommand
/// </summary>
public class AddRoutePointCommandValidator : AbstractValidator<AddRoutePointCommand>
{
    public AddRoutePointCommandValidator()
    {
        RuleFor(x => x.ActivityId)
            .NotEmpty()
            .WithMessage("Activity ID is required");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90 degrees");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180 degrees");

        RuleFor(x => x.Elevation)
            .GreaterThan(-500)
            .LessThan(10000)
            .WithMessage("Elevation must be between -500 and 10000 meters")
            .When(x => x.Elevation.HasValue);

        RuleFor(x => x.Timestamp)
            .NotEmpty()
            .WithMessage("Timestamp is required")
            .Must(x => x <= DateTime.UtcNow)
            .WithMessage("Timestamp cannot be in the future");
    }
}
