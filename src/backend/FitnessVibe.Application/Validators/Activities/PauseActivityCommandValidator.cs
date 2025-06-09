using FluentValidation;
using FitnessVibe.Application.Commands.Activities;

namespace FitnessVibe.Application.Validators.Activities
{
    /// <summary>
    /// Validator for the pause activity command
    /// </summary>
    public class PauseActivityCommandValidator : AbstractValidator<PauseActivityCommand>
    {
        /// <summary>
        /// Initializes a new instance of the validator
        /// </summary>
        public PauseActivityCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            RuleFor(x => x.SessionId)
                .NotEmpty()
                .WithMessage("Session ID is required");
        }
    }
}
