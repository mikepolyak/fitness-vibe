using FluentValidation;
using FitnessVibe.Application.Commands.Challenges;

namespace FitnessVibe.Application.Validators.Challenges;

public class ActivateChallengeCommandValidator : AbstractValidator<ActivateChallengeCommand>
{
    public ActivateChallengeCommandValidator()
    {
        RuleFor(x => x.ChallengeId)
            .NotEmpty();
    }
}
