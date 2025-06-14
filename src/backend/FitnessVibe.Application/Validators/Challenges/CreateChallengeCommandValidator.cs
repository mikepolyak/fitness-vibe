using FluentValidation;
using FitnessVibe.Application.Commands.Challenges;

namespace FitnessVibe.Application.Validators.Challenges;

public class CreateChallengeCommandValidator : AbstractValidator<CreateChallengeCommand>
{
    public CreateChallengeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date);

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .GreaterThan(x => x.StartDate);

        RuleFor(x => x.GoalValue)
            .GreaterThan(0);
    }
}
