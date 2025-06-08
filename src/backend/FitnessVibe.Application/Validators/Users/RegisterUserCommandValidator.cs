using FluentValidation;
using FitnessVibe.Application.Commands.Users;

namespace FitnessVibe.Application.Validators.Users
{
    /// <summary>
    /// Validator for user registration - the membership application reviewer.
    /// This ensures all new members provide valid information before joining our fitness community,
    /// like having a thorough application process at a premium gym.
    /// </summary>
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            // Email validation - like verifying contact information
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email address is required")
                .EmailAddress()
                .WithMessage("Please provide a valid email address")
                .MaximumLength(255)
                .WithMessage("Email address is too long");

            // Password validation - like setting secure locker combination requirements
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters long")
                .MaximumLength(128)
                .WithMessage("Password is too long")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
                .WithMessage("Password must contain at least one lowercase letter, one uppercase letter, and one number");

            // Name validation - like ensuring proper identification
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(50)
                .WithMessage("First name is too long")
                .Matches(@"^[a-zA-Z\s\-'\.]+$")
                .WithMessage("First name contains invalid characters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .MaximumLength(50)
                .WithMessage("Last name is too long")
                .Matches(@"^[a-zA-Z\s\-'\.]+$")
                .WithMessage("Last name contains invalid characters");

            // Age validation - like checking minimum age requirements
            RuleFor(x => x.DateOfBirth)
                .Must(BeValidAge)
                .When(x => x.DateOfBirth.HasValue)
                .WithMessage("Must be at least 13 years old and no more than 120 years old");

            // Terms acceptance - like requiring signature on gym waiver
            RuleFor(x => x.AcceptTerms)
                .Equal(true)
                .WithMessage("You must accept the terms and conditions to register");

            // Fitness level validation
            RuleFor(x => x.FitnessLevel)
                .IsInEnum()
                .WithMessage("Please select a valid fitness level");

            // Primary goal validation
            RuleFor(x => x.PrimaryGoal)
                .IsInEnum()
                .WithMessage("Please select a valid fitness goal");

            // Favorite activities validation
            RuleFor(x => x.FavoriteActivities)
                .Must(activities => activities == null || activities.Count <= 10)
                .WithMessage("You can select up to 10 favorite activities")
                .Must(activities => activities == null || activities.All(a => !string.IsNullOrWhiteSpace(a) && a.Length <= 50))
                .WithMessage("Activity names must be valid and under 50 characters");

            // Referral code validation
            RuleFor(x => x.ReferralCode)
                .MaximumLength(20)
                .WithMessage("Referral code is too long")
                .Matches(@"^[a-zA-Z0-9]+$")
                .When(x => !string.IsNullOrEmpty(x.ReferralCode))
                .WithMessage("Referral code can only contain letters and numbers");
        }

        /// <summary>
        /// Validates that the user is of appropriate age for fitness membership.
        /// Like checking ID at the gym front desk.
        /// </summary>
        private bool BeValidAge(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue) return true; // Optional field

            var age = DateTime.Today.Year - dateOfBirth.Value.Year;
            if (dateOfBirth.Value.Date > DateTime.Today.AddYears(-age)) age--;

            return age >= 13 && age <= 120;
        }
    }
}
