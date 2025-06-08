using MediatR;

namespace FitnessVibe.Application.Queries.Social
{
    /// <summary>
    /// Query to get the user's social activity feed.
    /// Like browsing your fitness community's news feed to see what everyone's been up to!
    /// </summary>
    public class GetSocialFeedQuery : IRequest<SocialFeedResponse>
    {
        public int UserId { get; set; }
        public string FeedType { get; set; } = "Friends"; // Friends, Following, Public, Clubs
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? ActivityTypeFilter { get; set; }
        public DateTime? Since { get; set; } // Only posts since this date
        public bool IncludeLiveActivities { get; set; } = true;
    }

    /// <summary>
    /// Social feed with fitness posts and activity updates.
    /// Like a personalized fitness news feed showing your community's achievements!
    /// </summary>
    public class SocialFeedResponse
    {
        public List<FeedPostDto> Posts { get; set; } = new();
        public FeedMetadataDto Metadata { get; set; } = new();
        public List<LiveActivityDto> LiveActivities { get; set; } = new();
        public List<SuggestedContentDto> SuggestedContent { get; set; } = new();
        public PaginationDto Pagination { get; set; } = new();
    }

    /// <summary>
    /// A post in the social feed.
    /// Like a workout story or achievement share!
    /// </summary>
    public class FeedPostDto
    {
        public int PostId { get; set; }
        public string PostType { get; set; } = string.Empty; // Activity, Achievement, Milestone, Status
        public PostAuthorDto Author { get; set; } = new();
        public string Content { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public DateTime CreatedAt { get; set; }
        public ActivitySummaryDto? Activity { get; set; } // If it's an activity post
        public AchievementDto? Achievement { get; set; } // If it's an achievement post
        public List<string> Photos { get; set; } = new();
        public List<string> HashTags { get; set; } = new();
        public List<TaggedUserDto> TaggedUsers { get; set; } = new();
        public PostEngagementDto Engagement { get; set; } = new();
        public bool CanInteract { get; set; } // Can current user like/comment
        public bool IsLive { get; set; } // Live activity
    }

    /// <summary>
    /// Post author information.
    /// </summary>
    public class PostAuthorDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public int Level { get; set; }
        public string LevelTitle { get; set; } = string.Empty;
        public bool IsFriend { get; set; }
        public bool IsVerified { get; set; }
        public string? Location { get; set; }
    }

    /// <summary>
    /// Achievement information in a post.
    /// </summary>
    public class AchievementDto
    {
        public string Type { get; set; } = string.Empty; // Badge, LevelUp, Milestone, PersonalRecord
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
        public int XpEarned { get; set; }
        public bool IsRare { get; set; }
    }

    /// <summary>
    /// Tagged user in a post.
    /// </summary>
    public class TaggedUserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
    }

    /// <summary>
    /// Post engagement metrics.
    /// </summary>
    public class PostEngagementDto
    {
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int CheersCount { get; set; }
        public int SharesCount { get; set; }
        public bool HasUserLiked { get; set; }
        public bool HasUserCheered { get; set; }
        public List<EngagementUserDto> RecentLikes { get; set; } = new(); // Top 3-5 recent likes
        public List<CommentDto> TopComments { get; set; } = new(); // Top 2-3 comments
    }

    /// <summary>
    /// User who engaged with a post.
    /// </summary>
    public class EngagementUserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public DateTime EngagedAt { get; set; }
    }

    /// <summary>
    /// Comment on a post.
    /// </summary>
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserAvatarUrl { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public bool HasUserLiked { get; set; }
    }

    /// <summary>
    /// Live activity in the feed.
    /// </summary>
    public class LiveActivityDto
    {
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserAvatarUrl { get; set; } = string.Empty;
        public string ActivityType { get; set; } = string.Empty;
        public string ActivityName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public double Distance { get; set; }
        public bool CanCheer { get; set; }
        public int CheerCount { get; set; }
        public List<EngagementUserDto> RecentCheers { get; set; } = new();
    }

    /// <summary>
    /// Suggested content for the feed.
    /// </summary>
    public class SuggestedContentDto
    {
        public string Type { get; set; } = string.Empty; // FriendSuggestion, ClubSuggestion, Challenge
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string ActionText { get; set; } = string.Empty; // "Add Friend", "Join Club"
        public string ActionUrl { get; set; } = string.Empty;
        public int Priority { get; set; }
    }

    /// <summary>
    /// Feed metadata and insights.
    /// </summary>
    public class FeedMetadataDto
    {
        public int TotalPosts { get; set; }
        public int NewPostsSinceLastVisit { get; set; }
        public int FriendsOnline { get; set; }
        public int LiveActivities { get; set; }
        public DateTime LastRefreshed { get; set; }
        public string PersonalizedMessage { get; set; } = string.Empty;
    }
}
