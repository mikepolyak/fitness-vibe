using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using FitnessVibe.Application.Services;
using FitnessVibe.Domain.Entities.Users;

namespace FitnessVibe.Infrastructure.Services
{
    /// <summary>
    /// Token Service Implementation - the digital ID card system for our fitness app.
    /// Think of this as the advanced security desk that issues temporary access badges
    /// and verifies them throughout the gym facility.
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TokenService> _logger;
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _accessTokenExpiryMinutes;
        private readonly int _refreshTokenExpiryDays;

        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Load JWT configuration
            _jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
            _issuer = _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
            _audience = _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");
            
            _accessTokenExpiryMinutes = int.Parse(_configuration["Jwt:ExpiryInMinutes"] ?? "60");
            _refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryInDays"] ?? "7");

            // Validate key length for security
            if (_jwtKey.Length < 32)
                throw new InvalidOperationException("JWT Key must be at least 32 characters long for security");
        }

        /// <summary>
        /// Generate an access token for the user.
        /// Like issuing a temporary VIP pass with the user's privileges and info.
        /// </summary>
        public string GenerateAccessToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.GetDisplayName()),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim("level", user.Level.ToString()),
                    new Claim("experience_points", user.ExperiencePoints.ToString()),
                    new Claim("fitness_level", user.FitnessLevel.ToString()),
                    new Claim("primary_goal", user.PrimaryGoal.ToString()),
                    new Claim("email_verified", user.IsEmailVerified.ToString().ToLower()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, 
                        new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), 
                        ClaimValueTypes.Integer64)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
                    Issuer = _issuer,
                    Audience = _audience,
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                _logger.LogDebug("Generated access token for user {UserId}", user.Id);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating access token for user {UserId}", user.Id);
                throw new InvalidOperationException("Failed to generate access token", ex);
            }
        }

        /// <summary>
        /// Generate a refresh token for the user.
        /// Like issuing a backup key that can be used to get new temporary passes.
        /// </summary>
        public string GenerateRefreshToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            try
            {
                // Generate a cryptographically secure random token
                var randomBytes = new byte[64];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomBytes);
                
                var refreshToken = Convert.ToBase64String(randomBytes);
                
                _logger.LogDebug("Generated refresh token for user {UserId}", user.Id);
                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token for user {UserId}", user.Id);
                throw new InvalidOperationException("Failed to generate refresh token", ex);
            }
        }

        /// <summary>
        /// Validate and extract user information from a token.
        /// Like scanning an ID badge to verify its authenticity and read the info.
        /// </summary>
        public bool ValidateToken(string token, out int userId)
        {
            userId = 0;

            if (string.IsNullOrWhiteSpace(token))
                return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5), // Allow 5 minutes clock skew
                    RequireExpirationTime = true
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                
                // Extract user ID from claims
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out userId))
                {
                    _logger.LogDebug("Successfully validated token for user {UserId}", userId);
                    return true;
                }

                _logger.LogWarning("Token validation failed: User ID claim not found or invalid");
                return false;
            }
            catch (SecurityTokenExpiredException)
            {
                _logger.LogDebug("Token validation failed: Token expired");
                return false;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning(ex, "Token validation failed: Security token exception");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during token validation");
                return false;
            }
        }

        /// <summary>
        /// Refresh an access token using a valid refresh token.
        /// Like using your backup key to get a new temporary pass.
        /// </summary>
        public string RefreshAccessToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token cannot be null or empty", nameof(refreshToken));

            // In a real implementation, you would:
            // 1. Look up the refresh token in the database
            // 2. Verify it's not expired or revoked
            // 3. Get the associated user
            // 4. Generate a new access token
            // 5. Optionally rotate the refresh token

            // For now, this is a placeholder implementation
            _logger.LogWarning("RefreshAccessToken called but not fully implemented");
            throw new NotImplementedException("Refresh token functionality requires database integration");
        }

        /// <summary>
        /// Get token expiry date.
        /// Like checking when an access badge expires.
        /// </summary>
        public DateTime? GetTokenExpiryDate(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                
                return jwtToken.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read token expiry date");
                return null;
            }
        }

        /// <summary>
        /// Extract claims from a token without validation.
        /// Like reading the information on an ID badge without verifying authenticity.
        /// </summary>
        public ClaimsPrincipal? GetPrincipalFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = securityKey,
                    ValidateIssuer = false, // Don't validate for this method
                    ValidateAudience = false, // Don't validate for this method
                    ValidateLifetime = false, // Don't validate expiry for this method
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract principal from token");
                return null;
            }
        }

        /// <summary>
        /// Check if a token is expired.
        /// Like checking if an access badge is past its expiration date.
        /// </summary>
        public bool IsTokenExpired(string token)
        {
            var expiry = GetTokenExpiryDate(token);
            return expiry.HasValue && expiry.Value <= DateTime.UtcNow;
        }

        /// <summary>
        /// Get user ID from token claims.
        /// Like reading the member ID from an access badge.
        /// </summary>
        public int? GetUserIdFromToken(string token)
        {
            var principal = GetPrincipalFromToken(token);
            var userIdClaim = principal?.FindFirst(ClaimTypes.NameIdentifier);
            
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                return userId;
                
            return null;
        }

        /// <summary>
        /// Revoke a refresh token (mark as invalid).
        /// Like cancelling a backup key so it can't be used anymore.
        /// </summary>
        public async Task RevokeRefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            // In a real implementation, you would mark the refresh token as revoked in the database
            _logger.LogDebug("Refresh token revoked: {RefreshToken}", refreshToken[..8] + "...");
            
            // TODO: Implement database logic to mark token as revoked
            await Task.CompletedTask;
        }
    }
}
