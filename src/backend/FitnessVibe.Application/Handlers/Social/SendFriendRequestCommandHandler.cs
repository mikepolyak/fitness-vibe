using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Social;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Social
{
    /// <summary>
    /// Handler for sending friend requests - the social connection facilitator.
    /// This is like having a friendly gym staff member help you connect with potential
    /// workout buddies and build your fitness community network.
    /// </summary>
    public class SendFriendRequestCommandHandler : IRequestHandler<SendFriendRequestCommand, SendFriendRequestResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly INotificationService _notificationService;
        private readonly ISocialAnalyticsService _socialAnalyticsService;
        private readonly ILogger<SendFriendRequestCommandHandler> _logger;

        public SendFriendRequestCommandHandler(
            IUserRepository userRepository,
            IFriendshipRepository friendshipRepository,
            INotificationService notificationService,
            ISocialAnalyticsService socialAnalyticsService,
            ILogger<SendFriendRequestCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _friendshipRepository = friendshipRepository ?? throw new ArgumentNullException(nameof(friendshipRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _socialAnalyticsService = socialAnalyticsService ?? throw new ArgumentNullException(nameof(socialAnalyticsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SendFriendRequestResponse> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing friend request from user {UserId} to user {TargetUserId}", 
                request.UserId, request.TargetUserId);

            // Validate users exist
            var sender = await _userRepository.GetByIdAsync(request.UserId);
            var target = await _userRepository.GetByIdAsync(request.TargetUserId);

            if (sender == null)
                throw new ArgumentException($"Sender user {request.UserId} not found");
            
            if (target == null)
                throw new ArgumentException($"Target user {request.TargetUserId} not found");

            // Can't send friend request to yourself
            if (request.UserId == request.TargetUserId)
            {
                throw new InvalidOperationException("Cannot send friend request to yourself");
            }

            // Check if they're already friends
            var existingFriendship = await _friendshipRepository.GetFriendshipAsync(request.UserId, request.TargetUserId);
            if (existingFriendship != null && existingFriendship.Status == Domain.Enums.FriendshipStatus.Accepted)
            {
                return new SendFriendRequestResponse
                {
                    TargetUserName = target.FirstName,
                    TargetUserAvatarUrl = target.AvatarUrl ?? "",
                    SentAt = DateTime.UtcNow,
                    Status = "AlreadyFriends",
                    Message = $"You and {target.FirstName} are already friends!"
                };
            }

            // Check if there's already a pending request
            var existingRequest = await _friendshipRepository.GetPendingRequestAsync(request.UserId, request.TargetUserId);
            if (existingRequest != null)
            {
                return new SendFriendRequestResponse
                {
                    FriendRequestId = existingRequest.Id,
                    TargetUserName = target.FirstName,
                    TargetUserAvatarUrl = target.AvatarUrl ?? "",
                    SentAt = existingRequest.CreatedAt,
                    Status = "RequestExists",
                    Message = $"Friend request to {target.FirstName} already sent!"
                };
            }

            // Check if target allows friend requests
            var targetPreferences = await _userRepository.GetUserPreferencesAsync(request.TargetUserId);
            if (!targetPreferences.AllowFriendRequests)
            {
                _logger.LogWarning("Friend request blocked - target user {TargetUserId} doesn't allow friend requests", 
                    request.TargetUserId);
                throw new InvalidOperationException("This user is not accepting friend requests");
            }

            // Check rate limiting (prevent spam)
            var recentRequests = await _friendshipRepository.GetRecentRequestsFromUserAsync(request.UserId, TimeSpan.FromHours(1));
            if (recentRequests.Count >= 10) // Max 10 requests per hour
            {
                throw new InvalidOperationException("Too many friend requests sent recently. Please try again later.");
            }

            // Create the friend request
            var friendRequest = new Domain.Entities.Social.FriendRequest(
                senderId: request.UserId,
                receiverId: request.TargetUserId,
                message: request.Message,
                source: request.Source
            );

            await _friendshipRepository.AddFriendRequestAsync(friendRequest);
            await _friendshipRepository.SaveChangesAsync();

            _logger.LogInformation("Friend request {RequestId} created from user {UserId} to user {TargetUserId}", 
                friendRequest.Id, request.UserId, request.TargetUserId);

            // Send notification to target user
            try
            {
                await SendFriendRequestNotification(sender, target, friendRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send friend request notification");
                // Don't fail the request if notification fails
            }

            // Track analytics
            try
            {
                await _socialAnalyticsService.TrackFriendRequestSentAsync(request.UserId, request.TargetUserId, request.Source);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to track friend request analytics");
                // Don't fail the request if analytics fail
            }

            return new SendFriendRequestResponse
            {
                FriendRequestId = friendRequest.Id,
                TargetUserName = target.FirstName,
                TargetUserAvatarUrl = target.AvatarUrl ?? "",
                SentAt = friendRequest.CreatedAt,
                Status = "Sent",
                Message = $"Friend request sent to {target.FirstName}!"
            };
        }

        /// <summary>
        /// Sends notification about the friend request to the target user.
        /// Like delivering a friendly workout buddy invitation!
        /// </summary>
        private async Task SendFriendRequestNotification(
            Domain.Entities.Users.User sender, 
            Domain.Entities.Users.User target, 
            Domain.Entities.Social.FriendRequest friendRequest)
        {
            var notificationData = new
            {
                Type = "FriendRequest",
                SenderId = sender.Id,
                SenderName = sender.FirstName,
                SenderAvatar = sender.AvatarUrl,
                RequestId = friendRequest.Id,
                Message = friendRequest.Message
            };

            await _notificationService.SendNotificationAsync(
                userId: target.Id,
                title: "New Friend Request",
                message: $"{sender.FirstName} wants to be your workout buddy!",
                type: "Social",
                data: notificationData
            );
        }
    }
}
