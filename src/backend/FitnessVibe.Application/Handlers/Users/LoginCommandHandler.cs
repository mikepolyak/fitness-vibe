using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Users;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Users
{
    /// <summary>
    /// Handler for user login - the digital security guard that checks your credentials.
    /// This is like having a friendly gym receptionist verify your membership,
    /// welcome you back, and hand you your access card for today's workout.
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ITokenService _tokenService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IChallengeRepository _challengeRepository;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            ITokenService tokenService,
            INotificationRepository notificationRepository,
            IChallengeRepository challengeRepository,
            ILogger<LoginCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHashingService = passwordHashingService ?? throw new ArgumentNullException(nameof(passwordHashingService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _notificationRepository = notificationRepository ?? throw new ArgumentNullException(nameof(notificationRepository));
            _challengeRepository = challengeRepository ?? throw new ArgumentNullException(nameof(challengeRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing login attempt for email: {Email}", request.Email);

            // Find the user - like looking up membership records
            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Login failed - user not found: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Check if account is locked or disabled
            if (user.IsLocked)
            {
                _logger.LogWarning("Login failed - account is locked: {Email}", request.Email);
                throw new UnauthorizedAccessException("Account is temporarily locked. Please try again later.");
            }

            // Verify password - like checking if the access code matches
            var isPasswordValid = await _passwordHashingService.VerifyPasswordAsync(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed - invalid password: {Email}", request.Email);
                
                // Record failed login attempt
                await RecordFailedLoginAttempt(user, request.IpAddress);
                
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            // Update last login information - like signing the guest book
            var lastLoginDate = user.LastLoginDate;
            user.UpdateLastLogin(request.IpAddress, request.DeviceInfo);
            await _userRepository.UpdateAsync(user);

            _logger.LogInformation("User logged in successfully: {UserId}", user.Id);

            // Calculate days since last login - like tracking how long it's been since their last gym visit
            var daysSinceLastLogin = lastLoginDate.HasValue 
                ? (DateTime.UtcNow - lastLoginDate.Value).Days 
                : 0;

            // Generate authentication tokens - like issuing today's access pass
            var tokenResult = await _tokenService.GenerateTokensAsync(user, request.RememberMe);

            // Get additional information for the response
            var unreadNotificationsCount = await _notificationRepository.GetUnreadCountAsync(user.Id);
            var activeChallenges = await _challengeRepository.GetActiveChallengesCountAsync(user.Id);
            var currentStreak = await CalculateCurrentStreak(user.Id);

            // Create personalized welcome back message
            var welcomeMessage = CreateWelcomeBackMessage(user, daysSinceLastLogin);

            return new LoginResponse
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
                WelcomeBackMessage = welcomeMessage,
                DaysSinceLastLogin = daysSinceLastLogin,
                HasUnreadNotifications = unreadNotificationsCount > 0,
                ActiveChallenges = activeChallenges,
                CurrentStreak = currentStreak
            };
        }

        /// <summary>
        /// Records a failed login attempt for security monitoring.
        /// Like keeping track of unsuccessful access attempts to the gym.
        /// </summary>
        private async Task RecordFailedLoginAttempt(User user, string? ipAddress)
        {
            try
            {
                user.RecordFailedLoginAttempt();
                await _userRepository.UpdateAsync(user);
                
                _logger.LogInformation("Recorded failed login attempt for user: {UserId}, IP: {IpAddress}", 
                    user.Id, ipAddress);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record login attempt for user: {UserId}", user.Id);
                // Don't fail the login process for this
            }
        }

        /// <summary>
        /// Calculates the user's current activity streak.
        /// Like tracking how many consecutive days they've visited the gym.
        /// </summary>
        private async Task<string> CalculateCurrentStreak(int userId)
        {
            try
            {
                // This would typically query the user's activity history
                // For now, return a placeholder - this would be implemented based on activity data
                var streakDays = await _userRepository.GetCurrentStreakAsync(userId);
                
                return streakDays switch
                {
                    0 => "Start your streak today!",
                    1 => "1 day streak - Keep going!",
                    var days when days < 7 => $"{days} day streak - Building momentum!",
                    var days when days < 30 => $"{days} day streak - You're on fire!",
                    var days => $"{days} day streak - Legendary dedication!"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate streak for user: {UserId}", userId);
                return "Welcome back!";
            }
        }

        /// <summary>
        /// Creates a personalized welcome back message based on user behavior.
        /// Like having a friendly gym staff member greet you by name and mention your progress.
        /// </summary>
        private string CreateWelcomeBackMessage(User user, int daysSinceLastLogin)
        {
            var timeOfDay = DateTime.Now.Hour switch
            {
                >= 5 and < 12 => "morning",
                >= 12 and < 17 => "afternoon",
                >= 17 and < 22 => "evening",
                _ => "night"
            };

            var greeting = $"Good {timeOfDay}, {user.FirstName}!";

            return daysSinceLastLogin switch
            {
                0 => $"{greeting} Ready for another great workout today?",
                1 => $"{greeting} Welcome back! Yesterday was productive - let's keep the momentum going!",
                var days when days <= 3 => $"{greeting} Great to see you again! Your {days}-day break is over - time to get back to crushing those goals!",
                var days when days <= 7 => $"{greeting} We missed you this week! Ready to jump back into your fitness journey?",
                var days when days <= 30 => $"{greeting} Welcome back after {days} days! Your fitness journey continues - every comeback is a new beginning!",
                var days => $"{greeting} What a comeback! After {days} days away, you're back and ready to restart your fitness adventure!"
            };
        }
    }
}
