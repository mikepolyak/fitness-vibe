using MediatR;

namespace FitnessVibe.Application.Commands.Social
{
    /// <summary>
    /// Command to send a friend request to another user.
    /// Like asking someone to be your workout buddy in the fitness community!
    /// </summary>
    public class SendFriendRequestCommand : IRequest<SendFriendRequestResponse>
    {
        public int UserId { get; set; } // Sender
        public int TargetUserId { get; set; } // Receiver
        public string? Message { get; set; } // Optional personal message
        public string Source { get; set; } = "Manual"; // Manual, Activity, Club, etc.
    }

    /// <summary>
    /// Response after sending a friend request.
    /// Like confirmation that your workout buddy invitation was sent!
    /// </summary>
    public class SendFriendRequestResponse
    {
        public int FriendRequestId { get; set; }
        public string TargetUserName { get; set; } = string.Empty;
        public string TargetUserAvatarUrl { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public string Status { get; set; } = string.Empty; // Sent, AlreadyFriends, RequestExists
        public string Message { get; set; } = string.Empty; // Success or info message
    }

    /// <summary>
    /// Command to respond to a friend request (accept or decline).
    /// Like deciding whether to accept a workout buddy invitation!
    /// </summary>
    public class RespondToFriendRequestCommand : IRequest<RespondToFriendRequestResponse>
    {
        public int UserId { get; set; } // Responder
        public int FriendRequestId { get; set; }
        public string Response { get; set; } = string.Empty; // Accept, Decline
        public string? ResponseMessage { get; set; } // Optional message
    }

    /// <summary>
    /// Response after responding to a friend request.
    /// </summary>
    public class RespondToFriendRequestResponse
    {
        public bool IsAccepted { get; set; }
        public string SenderUserName { get; set; } = string.Empty;
        public string SenderUserAvatarUrl { get; set; } = string.Empty;
        public DateTime RespondedAt { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool BothUsersNowFriends { get; set; }
        public int? NewFriendshipId { get; set; }
    }

    /// <summary>
    /// Command to send a cheer or encouragement to a friend during their workout.
    /// Like being a virtual cheerleader for your workout buddy!
    /// </summary>
    public class SendCheerCommand : IRequest<SendCheerResponse>
    {
        public int UserId { get; set; } // Sender
        public int TargetUserId { get; set; } // Receiver
        public int? ActivityId { get; set; } // Live activity being cheered
        public string CheerType { get; set; } = "Text"; // Text, Audio, Emoji, PowerUp
        public string Message { get; set; } = string.Empty;
        public string? AudioUrl { get; set; } // For audio cheers
        public string? EmojiCode { get; set; } // For emoji cheers
        public int PowerUpValue { get; set; } = 0; // XP bonus for receiver
    }

    /// <summary>
    /// Response after sending a cheer.
    /// </summary>
    public class SendCheerResponse
    {
        public int CheerId { get; set; }
        public string TargetUserName { get; set; } = string.Empty;
        public bool IsLiveActivity { get; set; }
        public bool CheerDelivered { get; set; }
        public int PowerUpAwarded { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    /// <summary>
    /// Command to share an activity to the social feed.
    /// Like posting your workout achievement on the community bulletin board!
    /// </summary>
    public class ShareActivityCommand : IRequest<ShareActivityResponse>
    {
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public string? Caption { get; set; }
        public List<string> TaggedFriends { get; set; } = new();
        public List<string> HashTags { get; set; } = new();
        public bool ShareToClubs { get; set; } = false;
        public List<int> ClubIds { get; set; } = new();
        public string Privacy { get; set; } = "Friends"; // Public, Friends, Private
    }

    /// <summary>
    /// Response after sharing an activity.
    /// </summary>
    public class ShareActivityResponse
    {
        public int PostId { get; set; }
        public string ShareableUrl { get; set; } = string.Empty;
        public string GeneratedCaption { get; set; } = string.Empty;
        public List<string> SuggestedHashtags { get; set; } = new();
        public int EstimatedReach { get; set; } // How many people might see it
        public bool SharedToClubs { get; set; }
        public List<string> ClubsSharedTo { get; set; } = new();
        public DateTime SharedAt { get; set; }
    }
}
