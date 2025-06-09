using FluentValidation;
using FitnessVibe.Application.Commands.Activities;

namespace FitnessVibe.Application.Validators.Activities
{
    /// <summary>
    /// Validator for the resume activity command
    /// </summary>
    public class ResumeActivityCommandValidator : AbstractValidator<ResumeActivityCommand>
    {
        /// <summary>
        /// Initializes a new instance of the validator
        /// </summary>
        public ResumeActivityCommandValidator()
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
