using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Domain.Entities.Users;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Commands.Users;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Users
{
    /// <summary>
    /// Handles user registration - like a digital receptionist who welcomes new members
    /// to our fitness community and sets them up for success.
    /// </summary>
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, RegisterUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHashingService passwordHashingService,
            ITokenService tokenService,
            ILogger<RegisterUserCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHashingService = passwordHashingService ?? throw new ArgumentNullException(nameof(passwordHashingService));
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RegisterUserResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing user registration for email: {Email}", request.Email);

            // Check if user already exists
            if (await _userRepository.EmailExistsAsync(request.Email, cancellationToken))
            {
                _logger.LogWarning("Registration attempt with existing email: {Email}", request.Email);
                throw new InvalidOperationException("A user with this email already exists");
            }

            // Hash the password securely
            var passwordHash = _passwordHashingService.HashPassword(request.Password);

            // Parse enums with fallback to defaults
            var fitnessLevel = Enum.TryParse<FitnessLevel>(request.FitnessLevel, out var level) 
                ? level 
                : FitnessLevel.Beginner;
            
            var primaryGoal = Enum.TryParse<FitnessGoal>(request.PrimaryGoal, out var goal) 
                ? goal 
                : FitnessGoal.StayActive;

            // Create the new user - their fitness journey begins!
            var user = new User(
                email: request.Email,
                firstName: request.FirstName,
                lastName: request.LastName,
                passwordHash: passwordHash,
                fitnessLevel: fitnessLevel,
                primaryGoal: primaryGoal);

            // Save to database
            await _userRepository.AddAsync(user, cancellationToken);

            _logger.LogInformation("Successfully registered user {UserId} with email {Email}", user.Id, user.Email);

            // Generate authentication tokens
            var token = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken(user);

            // Return the successful registration response
            return new RegisterUserResponse
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Level = user.Level,
                ExperiencePoints = user.ExperiencePoints,
                IsEmailVerified = user.IsEmailVerified,
                Token = token,
                RefreshToken = refreshToken
            };
        }
    }
}
