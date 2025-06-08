using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FitnessVibe.Application.Behaviors
{
    /// <summary>
    /// Authorization Behavior - the digital gym security guard.
    /// This ensures users can only access features and data they're authorized for,
    /// just like how a gym has different access levels for members, trainers, and staff.
    /// </summary>
    public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, IRequest<TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthorizationBehavior<TRequest, TResponse>> _logger;

        public AuthorizationBehavior(
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthorizationBehavior<TRequest, TResponse>> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request, 
            RequestHandlerDelegate<TResponse> next, 
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var httpContext = _httpContextAccessor.HttpContext;
            
            // Skip authorization for requests that don't require it
            if (IsPublicRequest(requestName))
            {
                return await next();
            }

            // Check if user is authenticated
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("Authorization failed: User not authenticated for {RequestName}", requestName);
                throw new UnauthorizedAccessException("Authentication required");
            }

            var userId = GetCurrentUserId(httpContext.User);
            var userRole = GetCurrentUserRole(httpContext.User);

            _logger.LogDebug("Authorization check for {RequestName}, User: {UserId}, Role: {UserRole}", 
                requestName, userId, userRole);

            // Perform authorization checks based on request type
            await PerformAuthorizationChecks(request, requestName, userId, userRole);

            // Log successful authorization
            _logger.LogDebug("Authorization successful for {RequestName}, User: {UserId}", requestName, userId);

            return await next();
        }

        /// <summary>
        /// Determines if a request is public and doesn't require authentication.
        /// Like identifying which gym areas are open to visitors without membership.
        /// </summary>
        private bool IsPublicRequest(string requestName)
        {
            var publicRequests = new[]
            {
                "RegisterUserCommand",
                "LoginCommand",
                "ForgotPasswordCommand",
                "ResetPasswordCommand",
                "VerifyEmailCommand",
                "RefreshTokenCommand"
            };

            return publicRequests.Contains(requestName);
        }

        /// <summary>
        /// Performs specific authorization checks based on request type and content.
        /// Like having different security checks for different gym areas and equipment.
        /// </summary>
        private async Task PerformAuthorizationChecks(TRequest request, string requestName, int userId, string userRole)
        {
            // Resource ownership checks - users can only access their own data
            await CheckResourceOwnership(request, requestName, userId);

            // Role-based authorization checks
            CheckRoleAuthorization(requestName, userRole);

            // Feature-specific authorization checks
            await CheckFeatureAuthorization(request, requestName, userId);

            // Rate limiting and usage checks
            await CheckUsageLimits(requestName, userId);
        }

        /// <summary>
        /// Checks if the user owns or has access to the requested resource.
        /// Like ensuring gym members can only access their own lockers and training data.
        /// </summary>
        private async Task CheckResourceOwnership(TRequest request, string requestName, int userId)
        {
            // Use reflection to check for UserId property on the request
            var userIdProperty = typeof(TRequest).GetProperty("UserId");
            if (userIdProperty != null)
            {
                var requestUserId = (int?)userIdProperty.GetValue(request);
                if (requestUserId.HasValue && requestUserId.Value != userId)
                {
                    _logger.LogWarning("Authorization failed: User {UserId} attempted to access resource for user {RequestUserId} in {RequestName}",
                        userId, requestUserId.Value, requestName);
                    throw new UnauthorizedAccessException("Access denied: You can only access your own resources");
                }
            }

            // Additional ownership checks for specific request types
            await PerformSpecificOwnershipChecks(request, requestName, userId);
        }

        /// <summary>
        /// Performs specific ownership checks for certain request types.
        /// </summary>
        private async Task PerformSpecificOwnershipChecks(TRequest request, string requestName, int userId)
        {
            // Activity-related authorization
            if (requestName.Contains("Activity"))
            {
                await CheckActivityOwnership(request, userId);
            }

            // Social feature authorization
            if (requestName.Contains("Social") || requestName.Contains("Friend"))
            {
                await CheckSocialAuthorization(request, userId);
            }

            // Club and group authorization
            if (requestName.Contains("Club") || requestName.Contains("Group"))
            {
                await CheckClubAuthorization(request, userId);
            }
        }

        /// <summary>
        /// Checks role-based authorization for administrative and special features.
        /// Like ensuring only gym staff can access management functions.
        /// </summary>
        private void CheckRoleAuthorization(string requestName, string userRole)
        {
            // Admin-only requests
            var adminOnlyRequests = new[]
            {
                "CreateClubCommand", // Only admins can create clubs initially
                "BanUserCommand",
                "DeleteUserCommand",
                "AdminReportQuery"
            };

            if (adminOnlyRequests.Contains(requestName) && userRole != "Admin")
            {
                _logger.LogWarning("Authorization failed: Non-admin user attempted admin operation: {RequestName}", requestName);
                throw new UnauthorizedAccessException("Administrative privileges required");
            }

            // Trainer-specific requests
            var trainerRequests = new[]
            {
                "CreateWorkoutPlanCommand",
                "AssignTrainerCommand",
                "TrainerDashboardQuery"
            };

            if (trainerRequests.Contains(requestName) && userRole != "Trainer" && userRole != "Admin")
            {
                _logger.LogWarning("Authorization failed: Non-trainer user attempted trainer operation: {RequestName}", requestName);
                throw new UnauthorizedAccessException("Trainer privileges required");
            }
        }

        /// <summary>
        /// Checks feature-specific authorization based on user subscription or permissions.
        /// Like checking if a member has access to premium gym equipment.
        /// </summary>
        private async Task CheckFeatureAuthorization(TRequest request, string requestName, int userId)
        {
            // Premium feature checks
            var premiumFeatures = new[]
            {
                "AdvancedAnalyticsQuery",
                "PersonalTrainerRequestCommand",
                "CustomWorkoutPlanCommand",
                "ExportDataCommand"
            };

            if (premiumFeatures.Contains(requestName))
            {
                var hasPremiumAccess = await CheckPremiumAccess(userId);
                if (!hasPremiumAccess)
                {
                    _logger.LogWarning("Authorization failed: User {UserId} attempted premium feature: {RequestName}", userId, requestName);
                    throw new UnauthorizedAccessException("Premium subscription required for this feature");
                }
            }

            // Beta feature checks
            var betaFeatures = new[]
            {
                "AiCoachCommand",
                "VirtualRealityWorkoutCommand"
            };

            if (betaFeatures.Contains(requestName))
            {
                var hasBetaAccess = await CheckBetaAccess(userId);
                if (!hasBetaAccess)
                {
                    _logger.LogWarning("Authorization failed: User {UserId} attempted beta feature: {RequestName}", userId, requestName);
                    throw new UnauthorizedAccessException("Beta access required for this feature");
                }
            }
        }

        /// <summary>
        /// Checks usage limits and rate limiting for the user.
        /// Like ensuring members don't exceed their daily gym visit limits.
        /// </summary>
        private async Task CheckUsageLimits(string requestName, int userId)
        {
            // API rate limiting for certain operations
            var rateLimitedOperations = new[]
            {
                "StartActivityCommand",
                "CompleteActivityCommand",
                "ShareActivityCommand",
                "SendCheerCommand"
            };

            if (rateLimitedOperations.Contains(requestName))
            {
                var isWithinRateLimit = await CheckRateLimit(userId, requestName);
                if (!isWithinRateLimit)
                {
                    _logger.LogWarning("Rate limit exceeded for user {UserId} on {RequestName}", userId, requestName);
                    throw new UnauthorizedAccessException("Rate limit exceeded. Please try again later.");
                }
            }
        }

        /// <summary>
        /// Gets the current user ID from JWT claims.
        /// </summary>
        private int GetCurrentUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                throw new UnauthorizedAccessException("Invalid user ID in token");
            }
            return userId;
        }

        /// <summary>
        /// Gets the current user role from JWT claims.
        /// </summary>
        private string GetCurrentUserRole(ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value ?? "Member";
        }

        /// <summary>
        /// Placeholder methods for specific authorization checks.
        /// These would be implemented based on business logic.
        /// </summary>
        private async Task CheckActivityOwnership(TRequest request, int userId)
        {
            // Implementation would check if user owns the activity
            await Task.CompletedTask;
        }

        private async Task CheckSocialAuthorization(TRequest request, int userId)
        {
            // Implementation would check social permissions
            await Task.CompletedTask;
        }

        private async Task CheckClubAuthorization(TRequest request, int userId)
        {
            // Implementation would check club membership/permissions
            await Task.CompletedTask;
        }

        private async Task<bool> CheckPremiumAccess(int userId)
        {
            // Implementation would check user's subscription status
            await Task.CompletedTask;
            return true; // Placeholder
        }

        private async Task<bool> CheckBetaAccess(int userId)
        {
            // Implementation would check beta program enrollment
            await Task.CompletedTask;
            return true; // Placeholder
        }

        private async Task<bool> CheckRateLimit(int userId, string operation)
        {
            // Implementation would check rate limiting
            await Task.CompletedTask;
            return true; // Placeholder
        }
    }
}
