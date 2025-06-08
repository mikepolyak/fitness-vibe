using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Users;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;
using FitnessVibe.Domain.Events;

namespace FitnessVibe.Application.Handlers.Users
{
    /// <summary>
    /// Handler for new user registration - the welcoming committee of our fitness community.
    /// This is like having a friendly gym staff member process your membership application,
    /// set up your access card, and give you the grand tour of what's possible.
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ITokenService _tokenService;
        private readonly IGamificationService _gamificationService;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            ITokenService tokenService,
            IGamificationService gamificationService,
            IEmailService emailService,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHashingService = passwordHashingService ?? throw new ArgumentNullException(nameof(passwordHashingService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _gamificationService = gamificationService ?? throw new ArgumentNullException(nameof(gamificationService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting user registration process for email: {Email}", request.Email);

            // Check if user already exists - like checking if someone already has a membership
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed - user already exists: {Email}", request.Email);
                throw new InvalidOperationException("A user with this email address already exists");
            }

            // Hash the password - like creating a secure access code for their gym locker
            var passwordHash = await _passwordHashingService.HashPasswordAsync(request.Password);

            // Create the new user - like issuing a new gym membership
            var user = new User(
                email: request.Email,
                firstName: request.FirstName,
                lastName: request.LastName,
                passwordHash: passwordHash,
                fitnessLevel: request.FitnessLevel,
                primaryGoal: request.PrimaryGoal
            );

            // Set additional profile information if provided
            if (request.DateOfBirth.HasValue || request.Gender.HasValue)
            {
                user.UpdateProfile(
                    request.FirstName,
                    request.LastName,
                    request.DateOfBirth,
                    request.Gender
                );
            }

            // Save the new user to our fitness community
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);

            // Award welcome badges - like giving new members their starter kit
            var welcomeBadges = await _gamificationService.AwardWelcomeBadgesAsync(user.Id);

            // Generate authentication tokens - like creating their digital access pass
            var tokenResult = await _tokenService.GenerateTokensAsync(user);

            // Send welcome email - like sending a friendly welcome package
            try
            {
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);
                _logger.LogInformation("Welcome email sent to: {Email}", user.Email);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send welcome email to: {Email}", user.Email);
                // Don't fail registration if email fails
            }

            // Create a personalized welcome message
            var welcomeMessage = CreateWelcomeMessage(user, welcomeBadges);

            _logger.LogInformation("User registration completed successfully for: {Email}", request.Email);

            return new RegisterUserResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Level = user.Level,
                ExperiencePoints = user.ExperiencePoints,
                IsEmailVerified = user.IsEmailVerified,
                Token = tokenResult.AccessToken,
                RefreshToken = tokenResult.RefreshToken,
                ExpiresAt = tokenResult.ExpiresAt,
                WelcomeBadges = welcomeBadges.Select(b => b.Name).ToList(),
                WelcomeMessage = welcomeMessage,
                NeedsOnboarding = true
            };
        }

        /// <summary>
        /// Creates a personalized welcome message based on the user's fitness goals.
        /// Like having a personal trainer give you an encouraging introduction speech.
        /// </summary>
        private string CreateWelcomeMessage(User user, List<Badge> welcomeBadges)
        {
            var goalMessages = new Dictionary<FitnessGoal, string>
            {
                [FitnessGoal.WeightLoss] = "Ready to crush your weight loss goals? Every step counts, and we're here to cheer you on!",
                [FitnessGoal.BuildMuscle] = "Time to build some serious strength! Your muscle-building journey starts now!",
                [FitnessGoal.ImproveCardio] = "Let's get that heart pumping! Your cardio improvement adventure begins today!",
                [FitnessGoal.Flexibility] = "Flexibility is the key to longevity! Ready to become more limber and agile?",
                [FitnessGoal.SportSpecific] = "Training for greatness! Let's make you unstoppable in your sport!",
                [FitnessGoal.GeneralHealth] = "Your health is your wealth! Let's build lifelong healthy habits together!",
                [FitnessGoal.StayActive] = "Movement is medicine! Let's keep you active and energized every day!"
            };

            var goalMessage = goalMessages.GetValueOrDefault(user.PrimaryGoal, 
                "Welcome to your fitness journey! Let's make every workout count!");

            var badgeCount = welcomeBadges.Count;
            var badgeMessage = badgeCount > 0 
                ? $" You've already earned {badgeCount} welcome badge{(badgeCount > 1 ? "s" : "")} - you're off to a great start!"
                : "";

            return $"Welcome to FitnessVibe, {user.FirstName}! {goalMessage}{badgeMessage}";
        }
    }
}