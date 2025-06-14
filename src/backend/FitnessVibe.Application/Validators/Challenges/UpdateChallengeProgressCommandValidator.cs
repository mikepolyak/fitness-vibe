using FluentValidation;
using FitnessVibe.Application.Commands.Challenges;

namespace FitnessVibe.Application.Validators.Challenges;

public class UpdateChallengeProgressCommandValidator : AbstractValidator<UpdateChallengeProgressCommand>
{
    public UpdateChallengeProgressCommandValidator()
    {
        RuleFor(x => x.ChallengeId)
            .NotEmpty();

        RuleFor(x => x.Progress)
            .GreaterThanOrEqualTo(0);
    }
}
