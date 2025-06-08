using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FitnessVibe.Application.Commands.Users;
using System.Security.Claims;

namespace FitnessVibe.API.Controllers
{
    /// <summary>
    /// Authentication Controller - the digital front desk for our fitness app.
    /// Think of this as the reception area where members sign up, check in,
    /// and manage their access credentials to the fitness center.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IMediator mediator, ILogger<AuthController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Register a new user account.
        /// Like signing up for a new gym membership with all the onboarding.
        /// </summary>
        /// <param name="command">User registration information</param>
        /// <returns>User profile and authentication tokens</returns>
        /// <response code="201">User registered successfully</response>
        /// <response code="400">Invalid registration data</response>
        /// <response code="409">Email already exists</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(RegisterUserResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RegisterUserResponse>> Register([FromBody] RegisterUserCommand command)
        {
            try
            {
                _logger.LogInformation("User registration attempt for email: {Email}", command.Email);

                var result = await _mediator.Send(command);

                _logger.LogInformation("User registered successfully: {UserId}", result.UserId);

                return CreatedAtAction(
                    nameof(GetProfile),
                    new { },
                    result
                );
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", command.Email);
                return Conflict(new ProblemDetails
                {
                    Title = "Email Already Exists",
                    Detail = "A user with this email address already exists",
                    Status = StatusCodes.Status409Conflict
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failed for email: {Email}", command.Email);
                return BadRequest(new ProblemDetails
                {
                    Title = "Registration Failed",
                    Detail = "Unable to register user. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Authenticate user and get access tokens.
        /// Like checking in at the gym with your membership credentials.
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>User profile and authentication tokens</returns>
        /// <response code="200">Login successful</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="423">Account locked</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status423Locked)]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                _logger.LogInformation("Login attempt for email: {Email}", request.Email);

                // Create login command (to be implemented)
                var command = new LoginCommand
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var result = await _mediator.Send(command);

                _logger.LogInformation("User logged in successfully: {UserId}", result.UserId);

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Login failed - invalid credentials for email: {Email}", request.Email);
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid Credentials",
                    Detail = "The email or password provided is incorrect",
                    Status = StatusCodes.Status401Unauthorized
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {Email}", request.Email);
                return BadRequest(new ProblemDetails
                {
                    Title = "Login Failed",
                    Detail = "Unable to authenticate user. Please try again.",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get current user profile.
        /// Like checking your membership details and current status.
        /// </summary>
        /// <returns>Current user profile information</returns>
        /// <response code="200">Profile retrieved successfully</response>
        /// <response code="401">Not authenticated</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserProfileResponse>> GetProfile()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogDebug("Getting profile for user: {UserId}", userId);

                // Create get user profile query (to be implemented)
                var query = new GetUserProfileQuery { UserId = userId };
                var result = await _mediator.Send(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user profile");
                return BadRequest(new ProblemDetails
                {
                    Title = "Profile Retrieval Failed",
                    Detail = "Unable to retrieve user profile",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Refresh authentication tokens.
        /// Like renewing your gym access card before it expires.
        /// </summary>
        /// <param name="request">Refresh token</param>
        /// <returns>New access and refresh tokens</returns>
        /// <response code="200">Tokens refreshed successfully</response>
        /// <response code="401">Invalid refresh token</response>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenRefreshResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokenRefreshResponse>> RefreshToken([FromBody] TokenRefreshRequest request)
        {
            try
            {
                _logger.LogDebug("Token refresh attempt");

                // Create refresh token command (to be implemented)
                var command = new RefreshTokenCommand { RefreshToken = request.RefreshToken };
                var result = await _mediator.Send(command);

                return Ok(result);
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Token refresh failed - invalid refresh token");
                return Unauthorized(new ProblemDetails
                {
                    Title = "Invalid Refresh Token",
                    Detail = "The refresh token is invalid or expired",
                    Status = StatusCodes.Status401Unauthorized
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token refresh failed");
                return BadRequest(new ProblemDetails
                {
                    Title = "Token Refresh Failed",
                    Detail = "Unable to refresh tokens",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Logout user and invalidate tokens.
        /// Like checking out of the gym and deactivating your access card.
        /// </summary>
        /// <returns>Logout confirmation</returns>
        /// <response code="200">Logout successful</response>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Logout()
        {
            try
            {
                var userId = GetCurrentUserId();
                _logger.LogInformation("User logout: {UserId}", userId);

                // Create logout command (to be implemented)
                var command = new LogoutCommand { UserId = userId };
                await _mediator.Send(command);

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return BadRequest(new ProblemDetails
                {
                    Title = "Logout Failed",
                    Detail = "Unable to logout user",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Request password reset email.
        /// Like requesting a new access code when you've forgotten your credentials.
        /// </summary>
        /// <param name="request">Email address for password reset</param>
        /// <returns>Password reset confirmation</returns>
        /// <response code="200">Reset email sent</response>
        /// <response code="404">Email not found</response>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                _logger.LogInformation("Password reset requested for email: {Email}", request.Email);

                // Create forgot password command (to be implemented)
                var command = new ForgotPasswordCommand { Email = request.Email };
                await _mediator.Send(command);

                // Always return success to prevent email enumeration attacks
                return Ok(new { message = "If the email exists, a password reset link has been sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password reset request failed for email: {Email}", request.Email);
                return Ok(new { message = "If the email exists, a password reset link has been sent" });
            }
        }

        /// <summary>
        /// Reset password using reset token.
        /// Like using your temporary access code to set a new permanent password.
        /// </summary>
        /// <param name="request">Reset token and new password</param>
        /// <returns>Password reset confirmation</returns>
        /// <response code="200">Password reset successful</response>
        /// <response code="400">Invalid or expired token</response>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                _logger.LogInformation("Password reset attempt with token");

                // Create reset password command (to be implemented)
                var command = new ResetPasswordCommand 
                { 
                    Token = request.Token,
                    NewPassword = request.NewPassword
                };
                await _mediator.Send(command);

                return Ok(new { message = "Password reset successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Password reset failed: {Error}", ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Password Reset Failed",
                    Detail = "The reset token is invalid or expired",
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Password reset failed");
                return BadRequest(new ProblemDetails
                {
                    Title = "Password Reset Failed",
                    Detail = "Unable to reset password",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Verify email address.
        /// Like confirming your contact information to complete membership setup.
        /// </summary>
        /// <param name="request">Email verification token</param>
        /// <returns>Email verification confirmation</returns>
        /// <response code="200">Email verified successfully</response>
        /// <response code="400">Invalid verification token</response>
        [HttpPost("verify-email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
            try
            {
                _logger.LogInformation("Email verification attempt");

                // Create verify email command (to be implemented)
                var command = new VerifyEmailCommand { Token = request.Token };
                await _mediator.Send(command);

                return Ok(new { message = "Email verified successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Email verification failed: {Error}", ex.Message);
                return BadRequest(new ProblemDetails
                {
                    Title = "Email Verification Failed",
                    Detail = "The verification token is invalid or expired",
                    Status = StatusCodes.Status400BadRequest
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email verification failed");
                return BadRequest(new ProblemDetails
                {
                    Title = "Email Verification Failed",
                    Detail = "Unable to verify email",
                    Status = StatusCodes.Status400BadRequest
                });
            }
        }

        /// <summary>
        /// Get current user ID from JWT claims.
        /// Like reading the member ID from the access card.
        /// </summary>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }
    }

    // Request/Response DTOs

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public bool IsEmailVerified { get; set; }
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class TokenRefreshRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class TokenRefreshResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class VerifyEmailRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class UserProfileResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? AvatarUrl { get; set; }
        public string FitnessLevel { get; set; } = string.Empty;
        public string PrimaryGoal { get; set; } = string.Empty;
        public int Level { get; set; }
        public int ExperiencePoints { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime LastActiveDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Placeholder command classes (to be implemented in Application layer)
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class GetUserProfileQuery : IRequest<UserProfileResponse>
    {
        public int UserId { get; set; }
    }

    public class RefreshTokenCommand : IRequest<TokenRefreshResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class LogoutCommand : IRequest
    {
        public int UserId { get; set; }
    }

    public class ForgotPasswordCommand : IRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordCommand : IRequest
    {
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class VerifyEmailCommand : IRequest
    {
        public string Token { get; set; } = string.Empty;
    }
}
