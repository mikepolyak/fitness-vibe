using MediatR;
using Microsoft.Extensions.Logging;
using FitnessVibe.Application.Commands.Social;
using FitnessVibe.Domain.Repositories;
using FitnessVibe.Application.Services;

namespace FitnessVibe.Application.Handlers.Social
{
    /// <summary>
    /// Handler for sending cheers and encouragement - the motivation delivery service.
    /// This is like having a personal cheerleader system that lets you send real-time
    /// encouragement and support to your workout buddies during their fitness sessions!
    /// </summary>
    public class SendCheerCommandHandler : IRequestHandler<SendCheerCommand, SendCheerResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IActivityRepository _activityRepository;
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly ICheerRepository _cheerRepository;
        private readonly INotificationService _notificationService;
        private readonly IGamificationService _gamificationService;
        private readonly ILiveActivityService _liveActivityService;
        private readonly ILogger<SendCheerCommandHandler> _logger;

        public SendCheerCommandHandler(
            IUserRepository userRepository,
            IActivityRepository activityRepository,
            IFriendshipRepository friendshipRepository,
            ICheerRepository cheerRepository,
            INotificationService notificationService,
            IGamificationService gamificationService,
            ILiveActivityService liveActivityService,
            ILogger<SendCheerCommandHandler> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _activityRepository = activityRepository ?? throw new ArgumentNullException(nameof(activityRepository));
            _friendshipRepository = friendshipRepository ?? throw new ArgumentNullException(nameof(friendshipRepository));
            _cheerRepository = cheerRepository ?? throw new ArgumentNullException(nameof(cheerRepository));
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
            _gamificationService = gamificationService ?? throw new ArgumentNullException(nameof(gamificationService));
            _liveActivityService = liveActivityService ?? throw new ArgumentNullException(nameof(liveActivityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SendCheerResponse> Handle(SendCheerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing cheer from user {UserId} to user {TargetUserId}, type: {CheerType}", 
                request.UserId, request.TargetUserId, request.CheerType);

            // Validate users exist
            var sender = await _userRepository.GetByIdAsync(request.UserId);
            var target = await _userRepository.GetByIdAsync(request.TargetUserId);

            if (sender == null)
                throw new ArgumentException($"Sender user {request.UserId} not found");
            
            if (target == null)
                throw new ArgumentException($"Target user {request.TargetUserId} not found");

            // Can't cheer yourself (though that would be amusing!)
            if (request.UserId == request.TargetUserId)
            {
                throw new InvalidOperationException("Cannot send cheers to yourself");
            }

            // Verify friendship or public profile
            var canCheer = await CanSendCheer(request.UserId, request.TargetUserId);
            if (!canCheer)
            {
                throw new UnauthorizedAccessException("You don't have permission to send cheers to this user");
            }

            // Validate activity if specified
            Domain.Entities.Activities.Activity? activity = null;
            bool isLiveActivity = false;
            
            if (request.ActivityId.HasValue)
            {
                activity = await _activityRepository.GetByIdAsync(request.ActivityId.Value);
                if (activity == null)
                {
                    throw new ArgumentException($"Activity {request.ActivityId} not found");
                }

                if (activity.UserId != request.TargetUserId)
                {
                    throw new ArgumentException("Activity does not belong to the target user");
                }

                // Check if it's a live activity
                isLiveActivity = activity.Status == Domain.Enums.ActivityStatus.Active;
            }

            // Check rate limiting for cheers (prevent spam)
            var recentCheers = await _cheerRepository.GetRecentCheersFromUserAsync(request.UserId, TimeSpan.FromMinutes(1));
            if (recentCheers.Count >= 5) // Max 5 cheers per minute
            {
                throw new InvalidOperationException("Too many cheers sent recently. Please slow down!");
            }

            // Validate cheer content
            ValidateCheerContent(request);

            // Create the cheer
            var cheer = new Domain.Entities.Social.Cheer(
                senderId: request.UserId,
                receiverId: request.TargetUserId,
                activityId: request.ActivityId,
                cheerType: Enum.Parse<Domain.Enums.CheerType>(request.CheerType),
                message: request.Message,
                audioUrl: request.AudioUrl,
                emojiCode: request.EmojiCode,
                powerUpValue: request.PowerUpValue
            );

            await _cheerRepository.AddAsync(cheer);
            await _cheerRepository.SaveChangesAsync();

            _logger.LogInformation("Cheer {CheerId} created from user {UserId} to user {TargetUserId}", 
                cheer.Id, request.UserId, request.TargetUserId);

            // Award power-up XP if specified
            int powerUpAwarded = 0;
            if (request.PowerUpValue > 0)
            {
                try
                {
                    await _gamificationService.AwardXpAsync(
                        request.TargetUserId, 
                        request.PowerUpValue, 
                        $"Power-up cheer from {sender.FirstName}"
                    );
                    powerUpAwarded = request.PowerUpValue;
                    
                    _logger.LogDebug("Awarded {PowerUp} power-up XP to user {TargetUserId}", 
                        request.PowerUpValue, request.TargetUserId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to award power-up XP for cheer");
                    // Don't fail the cheer if XP award fails
                }
            }

            // Send real-time notification if it's a live activity
            bool cheerDelivered = false;
            if (isLiveActivity)
            {
                try
                {
                    cheerDelivered = await DeliverLiveCheer(cheer, sender, target, activity!);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to deliver live cheer");
                    // Continue with regular notification
                }
            }

            // Send regular notification if not delivered live
            if (!cheerDelivered)
            {
                try
                {
                    await SendCheerNotification(cheer, sender, target, activity);
                    cheerDelivered = true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send cheer notification");
                }
            }

            return new SendCheerResponse
            {
                CheerId = cheer.Id,
                TargetUserName = target.FirstName,
                IsLiveActivity = isLiveActivity,
                CheerDelivered = cheerDelivered,
                PowerUpAwarded = powerUpAwarded,
                Message = CreateCheerSuccessMessage(sender.FirstName, target.FirstName, request.CheerType, isLiveActivity),
                SentAt = cheer.CreatedAt
            };
        }

        /// <summary>
        /// Checks if the sender can send cheers to the target user.
        /// Like verifying you have permission to cheer someone on!
        /// </summary>
        private async Task<bool> CanSendCheer(int senderId, int targetId)
        {
            // Check if they're friends
            var friendship = await _friendshipRepository.GetFriendshipAsync(senderId, targetId);
            if (friendship?.Status == Domain.Enums.FriendshipStatus.Accepted)
                return true;

            // Check if target allows public cheering
            var targetPreferences = await _userRepository.GetUserPreferencesAsync(targetId);
            return targetPreferences.AllowCheering;
        }

        /// <summary>
        /// Validates the cheer content based on type.
        /// Like ensuring your cheer message meets community standards!
        /// </summary>
        private void ValidateCheerContent(SendCheerCommand request)
        {
            switch (request.CheerType.ToLower())
            {
                case "text":
                    if (string.IsNullOrWhiteSpace(request.Message))
                        throw new ArgumentException("Text cheers must include a message");
                    if (request.Message.Length > 280)
                        throw new ArgumentException("Cheer message too long (max 280 characters)");
                    break;

                case "audio":
                    if (string.IsNullOrWhiteSpace(request.AudioUrl))
                        throw new ArgumentException("Audio cheers must include an audio URL");
                    break;

                case "emoji":
                    if (string.IsNullOrWhiteSpace(request.EmojiCode))
                        throw new ArgumentException("Emoji cheers must include an emoji code");
                    break;

                case "powerup":
                    if (request.PowerUpValue <= 0 || request.PowerUpValue > 100)
                        throw new ArgumentException("Power-up value must be between 1 and 100");
                    break;

                default:
                    throw new ArgumentException($"Invalid cheer type: {request.CheerType}");
            }
        }

        /// <summary>
        /// Delivers live cheer to an active workout session.
        /// Like having a real-time cheerleader appear during someone's workout!
        /// </summary>
        private async Task<bool> DeliverLiveCheer(
            Domain.Entities.Social.Cheer cheer, 
            Domain.Entities.Users.User sender, 
            Domain.Entities.Users.User target, 
            Domain.Entities.Activities.Activity activity)
        {
            try
            {
                var liveCheerData = new
                {
                    CheerId = cheer.Id,
                    CheerType = cheer.CheerType.ToString(),
                    Message = cheer.Message,
                    AudioUrl = cheer.AudioUrl,
                    EmojiCode = cheer.EmojiCode,
                    PowerUpValue = cheer.PowerUpValue,
                    SenderName = sender.FirstName,
                    SenderAvatar = sender.AvatarUrl,
                    SentAt = cheer.CreatedAt
                };

                await _liveActivityService.DeliverLiveCheerAsync(activity.Id, liveCheerData);
                
                _logger.LogDebug("Live cheer delivered to activity {ActivityId}", activity.Id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deliver live cheer to activity {ActivityId}", activity.Id);
                return false;
            }
        }

        /// <summary>
        /// Sends a notification about the cheer.
        /// Like making sure the recipient knows they got some encouragement!
        /// </summary>
        private async Task SendCheerNotification(
            Domain.Entities.Social.Cheer cheer,
            Domain.Entities.Users.User sender,
            Domain.Entities.Users.User target,
            Domain.Entities.Activities.Activity? activity)
        {
            var notificationData = new
            {
                Type = "Cheer",
                CheerId = cheer.Id,
                CheerType = cheer.CheerType.ToString(),
                SenderId = sender.Id,
                SenderName = sender.FirstName,
                SenderAvatar = sender.AvatarUrl,
                Message = cheer.Message,
                PowerUpValue = cheer.PowerUpValue,
                ActivityId = activity?.Id,
                ActivityName = activity?.Name
            };

            var title = cheer.CheerType switch
            {
                Domain.Enums.CheerType.PowerUp => "Power-Up Cheer!",
                Domain.Enums.CheerType.Audio => "Audio Cheer!",
                Domain.Enums.CheerType.Emoji => "Emoji Cheer!",
                _ => "Cheer!"
            };

            var message = activity != null 
                ? $"{sender.FirstName} is cheering you on during your {activity.ActivityType.ToLower()}!"
                : $"{sender.FirstName} sent you a cheer!";

            await _notificationService.SendNotificationAsync(
                userId: target.Id,
                title: title,
                message: message,
                type: "Social",
                data: notificationData
            );
        }

        /// <summary>
        /// Creates a success message for the cheer sender.
        /// </summary>
        private string CreateCheerSuccessMessage(string senderName, string targetName, string cheerType, bool isLive)
        {
            var liveText = isLive ? " during their live workout" : "";
            
            return cheerType.ToLower() switch
            {
                "powerup" => $"Power-up sent to {targetName}{liveText}! 🚀",
                "audio" => $"Audio cheer delivered to {targetName}{liveText}! 🎵",
                "emoji" => $"Emoji cheer sent to {targetName}{liveText}! 😊",
                _ => $"Cheer sent to {targetName}{liveText}! 👏"
            };
        }
    }
}
