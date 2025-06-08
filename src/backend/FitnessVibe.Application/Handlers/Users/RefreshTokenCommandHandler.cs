using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Users;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Users
{
    /// <summary>
    /// Handler for refreshing authentication tokens - the automated pass renewal service.
    /// This is like having an automatic gym pass renewal system that updates your access
    /// credentials before they expire, ensuring uninterrupted access to your fitness journey.
    /// </summary>
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenRefreshResponse>
    {
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RefreshTokenCommandHandler> _logger;

        public RefreshTokenCommandHandler(
            ITokenService tokenService,
            IUserRepository userRepository,
            ILogger<RefreshTokenCommandHandler> logger)
        {
            _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TokenRefreshResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Processing token refresh request");

            try
            {
                // Validate the refresh token - like verifying the renewal code
                var tokenValidationResult = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);
                
                if (!tokenValidationResult.IsValid)
                {
                    _logger.LogWarning("Token refresh failed - invalid refresh token");
                    throw new UnauthorizedAccessException("Invalid or expired refresh token");
                }

                // Get the user associated with the token
                var user = await _userRepository.GetByIdAsync(tokenValidationResult.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Token refresh failed - user not found: {UserId}", tokenValidationResult.UserId);
                    throw new UnauthorizedAccessException("User not found");
                }

                // Check if user account is still active
                if (user.IsLocked || !user.IsActive)
                {
                    _logger.LogWarning("Token refresh failed - user account is inactive: {UserId}", user.Id);
                    throw new UnauthorizedAccessException("User account is not active");
                }

                // Generate new tokens - like issuing a fresh access pass
                var newTokenResult = await _tokenService.RefreshTokensAsync(request.RefreshToken, request.DeviceInfo);

                // Update user's last activity
                user.UpdateLastActivity();
                await _userRepository.UpdateAsync(user);

                _logger.LogDebug("Token refresh successful for user: {UserId}", user.Id);

                return new TokenRefreshResponse
                {
                    Token = newTokenResult.AccessToken,
                    RefreshToken = newTokenResult.RefreshToken,
                    ExpiresAt = newTokenResult.ExpiresAt,
                    RefreshedAt = DateTime.UtcNow
                };
            }
            catch (UnauthorizedAccessException)
            {
                // Re-throw authorization exceptions as-is
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh failed with unexpected error");
                throw new UnauthorizedAccessException("Token refresh failed");
            }
        }
    }
}
