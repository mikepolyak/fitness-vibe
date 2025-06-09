using FluentValidation;
using FitnessVibe.Application.Commands.Activities;

namespace FitnessVibe.Application.Validators.Activities
{
    /// <summary>
    /// Validator for the cancel activity command
    /// </summary>
    public class CancelActivityCommandValidator : AbstractValidator<CancelActivityCommand>
    {
        /// <summary>
        /// Initializes a new instance of the validator
        /// </summary>
        public CancelActivityCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            RuleFor(x => x.SessionId)
                .NotEmpty()
                .WithMessage("Session ID is required");

            When(x => x.CancellationReason != null, () =>
            {
                RuleFor(x => x.CancellationReason)
                    .MaximumLength(500)
                    .WithMessage("Cancellation reason cannot exceed 500 characters");
            });
        }
    }
}
