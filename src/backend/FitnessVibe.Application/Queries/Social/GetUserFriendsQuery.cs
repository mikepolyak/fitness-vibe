using MediatR;

namespace FitnessVibe.Application.Queries.Social
{
    /// <summary>
    /// Query to get user's friends, friend requests, and social connections.
    /// Like checking your workout buddy network and pending invitations!
    /// </summary>
    public class GetUserFriendsQuery : IRequest<UserFriendsResponse>
    {
        public int UserId { get; set; }
        public string Section { get; set; } = "All"; // All, Friends, Pending, Sent, Suggestions
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SearchQuery { get; set; }
        public bool IncludeActivityStatus { get; set; } = true;
        public bool IncludeMutualFriends { get; set; } = true;
        public string SortBy { get; set; } = "Name"; // Name, RecentActivity, MutualFriends
    }

    /// <summary>
    /// Complete friends and social connections data.
    /// Like your fitness community address book with all your workout connections!
    /// </summary>
    public class UserFriendsResponse
    {
        public List<FriendDto> Friends { get; set; } = new();
        public List<FriendRequestDto> PendingRequests { get; set; } = new();
        public List<FriendRequestDto> SentRequests { get; set; } = new();
        public List<FriendSuggestionDto> Suggestions { get; set; } = new();
        public FriendsStatsDto Statistics { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }

    /// <summary>
    /// Friend information with activity status.
    /// Like a profile card for each of your workout buddies!
    /// </summary>
    public class FriendDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Level { get; set; }
        public string LevelTitle { get; set; } = string.Empty;
        public bool IsOnline { get; set; }
        public DateTime? LastSeenAt { get; set; }
        public FriendActivityStatusDto? ActivityStatus { get; set; }
        public FriendStatsDto Stats { get; set; } = new();
        public List<MutualFriendDto> MutualFriends { get; set; } = new();
        public DateTime FriendsSince { get; set; }
        public bool CanSendMessage { get; set; }
        public bool AllowsCheering { get; set; }
        public string? Location { get; set; }
        public List<string> FavoriteActivities { get; set; } = new();
        public List<string> RecentBadges { get; set; } = new();
    }

    /// <summary>
    /// Friend's current activity status.
    /// </summary>
    public class FriendActivityStatusDto
    {
        public string Status { get; set; } = string.Empty; // WorkingOut, JustFinished, Resting, Planning
        public string? CurrentActivity { get; set; }
        public DateTime? ActivityStartTime { get; set; }
        public string? ActivityLocation { get; set; }
        public bool CanCheer { get; set; }
        public string StatusMessage { get; set; } = string.Empty; // "Running 5km", "Just finished yoga"
    }

    /// <summary>
    /// Friend's fitness statistics.
    /// </summary>
    public class FriendStatsDto
    {
        public int CurrentStreak { get; set; }
        public int TotalWorkouts { get; set; }
        public int TotalBadges { get; set; }
        public string FavoriteActivity { get; set; } = string.Empty;
        public int WorkoutsThisWeek { get; set; }
        public bool IsOnFireThisWeek { get; set; } // More active than usual
        public int LeaderboardRank { get; set; }
    }

    /// <summary>
    /// Mutual friend information.
    /// </summary>
    public class MutualFriendDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// Friend request information.
    /// </summary>
    public class FriendRequestDto
    {
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Level { get; set; }
        public string LevelTitle { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTime SentAt { get; set; }
        public string Source { get; set; } = string.Empty; // Manual, Activity, Club
        public List<MutualFriendDto> MutualFriends { get; set; } = new();
        public bool IsIncoming { get; set; } // True if received, false if sent
        public FriendStatsDto Stats { get; set; } = new();
        public string? Location { get; set; }
    }

    /// <summary>
    /// Friend suggestion with reasoning.
    /// </summary>
    public class FriendSuggestionDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Level { get; set; }
        public string LevelTitle { get; set; } = string.Empty;
        public string SuggestionReason { get; set; } = string.Empty; // "3 mutual friends", "Similar fitness goals"
        public List<string> ReasonDetails { get; set; } = new();
        public List<MutualFriendDto> MutualFriends { get; set; } = new();
        public double CompatibilityScore { get; set; } // 0-100%
        public FriendStatsDto Stats { get; set; } = new();
        public List<string> SharedInterests { get; set; } = new();
        public string? Location { get; set; }
        public bool HasRequestBeenSent { get; set; }
    }

    /// <summary>
    /// Overall friends statistics.
    /// </summary>
    public class FriendsStatsDto
    {
        public int TotalFriends { get; set; }
        public int OnlineFriends { get; set; }
        public int FriendsWorkingOut { get; set; }
        public int PendingIncomingRequests { get; set; }
        public int PendingSentRequests { get; set; }
        public int AvailableSuggestions { get; set; }
        public int NewRequestsToday { get; set; }
        public List<PopularFriendActivityDto> PopularActivities { get; set; } = new();
        public string MostActiveFriend { get; set; } = string.Empty;
        public string NetworkGrowthTrend { get; set; } = string.Empty; // Growing, Stable, Declining
    }

    /// <summary>
    /// Popular activity among friends.
    /// </summary>
    public class PopularFriendActivityDto
    {
        public string ActivityType { get; set; } = string.Empty;
        public int FriendsCount { get; set; }
        public string Description { get; set; } = string.Empty; // "5 friends love running"
    }
}
