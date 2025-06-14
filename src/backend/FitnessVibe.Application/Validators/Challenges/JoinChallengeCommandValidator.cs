using FluentValidation;
using FitnessVibe.Application.Commands.Challenges;

namespace FitnessVibe.Application.Validators.Challenges;

public class JoinChallengeCommandValidator : AbstractValidator<JoinChallengeCommand>
{
    public JoinChallengeCommandValidator()
    {
        RuleFor(x => x.ChallengeId)
            .NotEmpty();
    }
}
