using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Users;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Users
{
    /// <summary>
    /// Handler for user logout - the digital security guard that safely ends your session.
    /// This is like having a gym receptionist process your checkout, secure your locker,
    /// and ensure all your session data is properly saved before you leave.
    /// </summary>
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            ITokenService tokenService,
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            ILogger<LogoutCommandHandler> logger)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing logout for user {UserId}", request.UserId);

            try
            {
                // Get the user to verify they exist
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Logout attempted for non-existent user: {UserId}", request.UserId);
                    return; // Silent fail for security
                }

                // Invalidate refresh tokens
                if (!string.IsNullOrEmpty(request.RefreshToken))
                {
                    await _tokenService.RevokeRefreshTokenAsync(request.RefreshToken);
                    _logger.LogDebug("Revoked specific refresh token for user {UserId}", request.UserId);
                }

                // If logging out all devices, revoke all refresh tokens
                if (request.LogoutAllDevices)
                {
                    await _tokenService.RevokeAllUserRefreshTokensAsync(request.UserId);
                    _logger.LogInformation("Revoked all refresh tokens for user {UserId}", request.UserId);
                }

                // End any active sessions
                await _sessionRepository.EndActiveSessionsAsync(request.UserId, request.LogoutAllDevices);

                // Update user's last logout time
                user.UpdateLastLogout();
                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("User {UserId} logged out successfully", request.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to logout user {UserId}", request.UserId);
                throw;
            }
        }
    }
}
