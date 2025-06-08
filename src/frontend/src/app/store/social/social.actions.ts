import { createAction, props } from '@ngrx/store';

/**
 * Social Actions - the commands that trigger social fitness community interactions.
 * Think of these as different ways to connect, share, and compete with your fitness
 * community - like a social network built specifically for fitness enthusiasts!
 */

// Activity Feed and Posts
export const loadActivityFeed = createAction(
  '[Social] Load Activity Feed',
  props<{ 
    feedType?: 'following' | 'friends' | 'public' | 'clubs';
    page?: number;
    limit?: number;
    filters?: {
      activityTypes?: string[];
      dateRange?: { start: Date; end: Date };
      location?: string;
    };
  }>()
);

export const loadActivityFeedSuccess = createAction(
  '[Social] Load Activity Feed Success',
  props<{ 
    posts: Array<{
      id: number;
      userId: number;
      user: {
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
        level: number;
        isFollowing: boolean;
      };
      activityId?: number;
      activity?: {
        id: number;
        name: string;
        type: string;
        duration: number;
        distance?: number;
        caloriesBurned?: number;
        route?: any;
      };
      goalId?: number;
      goal?: {
        id: number;
        title: string;
        type: string;
        progress: number;
        target: number;
      };
      postType: 'activity' | 'goal_achievement' | 'milestone' | 'badge_earned' | 'level_up' | 'general';
      content: string;
      images: string[];
      location?: string;
      visibility: 'public' | 'friends' | 'private';
      likes: number;
      comments: number;
      shares: number;
      isLiked: boolean;
      isSaved: boolean;
      createdAt: Date;
      updatedAt?: Date;
    }>;
    hasMore: boolean;
    totalCount: number;
  }>()
);

export const loadActivityFeedFailure = createAction(
  '[Social] Load Activity Feed Failure',
  props<{ error: string }>()
);

export const createPost = createAction(
  '[Social] Create Post',
  props<{ 
    postData: {
      content: string;
      postType: 'activity' | 'goal_achievement' | 'milestone' | 'badge_earned' | 'level_up' | 'general';
      activityId?: number;
      goalId?: number;
      images?: string[];
      location?: string;
      visibility: 'public' | 'friends' | 'private';
      tags?: string[];
    };
  }>()
);

export const createPostSuccess = createAction(
  '[Social] Create Post Success',
  props<{ post: any }>()
);

export const createPostFailure = createAction(
  '[Social] Create Post Failure',
  props<{ error: string }>()
);

export const updatePost = createAction(
  '[Social] Update Post',
  props<{ 
    postId: number;
    updates: {
      content?: string;
      visibility?: 'public' | 'friends' | 'private';
      tags?: string[];
    };
  }>()
);

export const updatePostSuccess = createAction(
  '[Social] Update Post Success',
  props<{ post: any }>()
);

export const deletePost = createAction(
  '[Social] Delete Post',
  props<{ postId: number }>()
);

export const deletePostSuccess = createAction(
  '[Social] Delete Post Success',
  props<{ postId: number }>()
);

// Post Interactions
export const likePost = createAction(
  '[Social] Like Post',
  props<{ postId: number }>()
);

export const likePostSuccess = createAction(
  '[Social] Like Post Success',
  props<{ postId: number; likesCount: number }>()
);

export const unlikePost = createAction(
  '[Social] Unlike Post',
  props<{ postId: number }>()
);

export const unlikePostSuccess = createAction(
  '[Social] Unlike Post Success',
  props<{ postId: number; likesCount: number }>()
);

export const savePost = createAction(
  '[Social] Save Post',
  props<{ postId: number }>()
);

export const savePostSuccess = createAction(
  '[Social] Save Post Success',
  props<{ postId: number }>()
);

export const unsavePost = createAction(
  '[Social] Unsave Post',
  props<{ postId: number }>()
);

export const unsavePostSuccess = createAction(
  '[Social] Unsave Post Success',
  props<{ postId: number }>()
);

export const sharePost = createAction(
  '[Social] Share Post',
  props<{ 
    postId: number;
    shareType: 'repost' | 'external' | 'message';
    message?: string;
    recipients?: number[];
  }>()
);

export const sharePostSuccess = createAction(
  '[Social] Share Post Success',
  props<{ postId: number; sharesCount: number }>()
);

// Comments
export const loadPostComments = createAction(
  '[Social] Load Post Comments',
  props<{ postId: number; page?: number; limit?: number }>()
);

export const loadPostCommentsSuccess = createAction(
  '[Social] Load Post Comments Success',
  props<{ 
    postId: number;
    comments: Array<{
      id: number;
      postId: number;
      userId: number;
      user: {
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
      };
      content: string;
      likes: number;
      isLiked: boolean;
      replies: number;
      createdAt: Date;
      updatedAt?: Date;
    }>;
    hasMore: boolean;
  }>()
);

export const addComment = createAction(
  '[Social] Add Comment',
  props<{ 
    postId: number;
    content: string;
    parentCommentId?: number;
  }>()
);

export const addCommentSuccess = createAction(
  '[Social] Add Comment Success',
  props<{ comment: any; postId: number }>()
);

export const updateComment = createAction(
  '[Social] Update Comment',
  props<{ commentId: number; content: string }>()
);

export const updateCommentSuccess = createAction(
  '[Social] Update Comment Success',
  props<{ comment: any }>()
);

export const deleteComment = createAction(
  '[Social] Delete Comment',
  props<{ commentId: number; postId: number }>()
);

export const deleteCommentSuccess = createAction(
  '[Social] Delete Comment Success',
  props<{ commentId: number; postId: number }>()
);

export const likeComment = createAction(
  '[Social] Like Comment',
  props<{ commentId: number }>()
);

export const likeCommentSuccess = createAction(
  '[Social] Like Comment Success',
  props<{ commentId: number; likesCount: number }>()
);

// Friends and Following
export const loadFriends = createAction('[Social] Load Friends');

export const loadFriendsSuccess = createAction(
  '[Social] Load Friends Success',
  props<{ 
    friends: Array<{
      id: number;
      firstName: string;
      lastName: string;
      avatarUrl?: string;
      level: number;
      experiencePoints: number;
      lastActiveDate: Date;
      mutualFriends: number;
      isOnline: boolean;
      relationship: 'friend' | 'following' | 'follower' | 'mutual';
      connectedSince: Date;
    }>;
  }>()
);

export const loadFriendsFailure = createAction(
  '[Social] Load Friends Failure',
  props<{ error: string }>()
);

export const loadFriendRequests = createAction('[Social] Load Friend Requests');

export const loadFriendRequestsSuccess = createAction(
  '[Social] Load Friend Requests Success',
  props<{ 
    incoming: Array<{
      id: number;
      fromUserId: number;
      user: {
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
        level: number;
        mutualFriends: number;
      };
      message?: string;
      createdAt: Date;
    }>;
    outgoing: Array<{
      id: number;
      toUserId: number;
      user: {
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
      };
      message?: string;
      createdAt: Date;
    }>;
  }>()
);

export const sendFriendRequest = createAction(
  '[Social] Send Friend Request',
  props<{ userId: number; message?: string }>()
);

export const sendFriendRequestSuccess = createAction(
  '[Social] Send Friend Request Success',
  props<{ userId: number; requestId: number }>()
);

export const acceptFriendRequest = createAction(
  '[Social] Accept Friend Request',
  props<{ requestId: number }>()
);

export const acceptFriendRequestSuccess = createAction(
  '[Social] Accept Friend Request Success',
  props<{ requestId: number; newFriend: any }>()
);

export const declineFriendRequest = createAction(
  '[Social] Decline Friend Request',
  props<{ requestId: number }>()
);

export const declineFriendRequestSuccess = createAction(
  '[Social] Decline Friend Request Success',
  props<{ requestId: number }>()
);

export const removeFriend = createAction(
  '[Social] Remove Friend',
  props<{ userId: number }>()
);

export const removeFriendSuccess = createAction(
  '[Social] Remove Friend Success',
  props<{ userId: number }>()
);

export const followUser = createAction(
  '[Social] Follow User',
  props<{ userId: number }>()
);

export const followUserSuccess = createAction(
  '[Social] Follow User Success',
  props<{ userId: number }>()
);

export const unfollowUser = createAction(
  '[Social] Unfollow User',
  props<{ userId: number }>()
);

export const unfollowUserSuccess = createAction(
  '[Social] Unfollow User Success',
  props<{ userId: number }>()
);

// Clubs and Groups
export const loadClubs = createAction(
  '[Social] Load Clubs',
  props<{ 
    type?: 'joined' | 'public' | 'recommended' | 'nearby';
    category?: string;
    location?: string;
    page?: number;
  }>()
);

export const loadClubsSuccess = createAction(
  '[Social] Load Clubs Success',
  props<{ 
    clubs: Array<{
      id: number;
      name: string;
      description: string;
      category: string;
      imageUrl?: string;
      isPublic: boolean;
      memberCount: number;
      activeMembers: number;
      location?: string;
      tags: string[];
      isJoined: boolean;
      role?: 'owner' | 'admin' | 'moderator' | 'member';
      joinedAt?: Date;
      recentActivity: {
        posts: number;
        challenges: number;
        events: number;
      };
      stats: {
        totalActivities: number;
        totalDistance: number;
        averageLevel: number;
      };
    }>;
    hasMore: boolean;
  }>()
);

export const loadClubsFailure = createAction(
  '[Social] Load Clubs Failure',
  props<{ error: string }>()
);

export const loadClubDetails = createAction(
  '[Social] Load Club Details',
  props<{ clubId: number }>()
);

export const loadClubDetailsSuccess = createAction(
  '[Social] Load Club Details Success',
  props<{ 
    club: {
      id: number;
      name: string;
      description: string;
      category: string;
      imageUrl?: string;
      coverImageUrl?: string;
      isPublic: boolean;
      memberCount: number;
      location?: string;
      tags: string[];
      rules?: string;
      isJoined: boolean;
      role?: 'owner' | 'admin' | 'moderator' | 'member';
      joinedAt?: Date;
      createdAt: Date;
      owner: {
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
      };
      stats: {
        totalActivities: number;
        totalDistance: number;
        totalMembers: number;
        averageLevel: number;
        monthlyGrowth: number;
      };
      recentMembers: Array<{
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
        joinedAt: Date;
      }>;
      upcomingEvents: Array<{
        id: number;
        title: string;
        startDate: Date;
        type: string;
      }>;
      activeChallenges: Array<{
        id: number;
        title: string;
        participants: number;
        endDate: Date;
      }>;
    };
  }>()
);

export const createClub = createAction(
  '[Social] Create Club',
  props<{ 
    clubData: {
      name: string;
      description: string;
      category: string;
      isPublic: boolean;
      location?: string;
      tags: string[];
      rules?: string;
      imageUrl?: string;
    };
  }>()
);

export const createClubSuccess = createAction(
  '[Social] Create Club Success',
  props<{ club: any }>()
);

export const joinClub = createAction(
  '[Social] Join Club',
  props<{ clubId: number; message?: string }>()
);

export const joinClubSuccess = createAction(
  '[Social] Join Club Success',
  props<{ clubId: number; membership: any }>()
);

export const leaveClub = createAction(
  '[Social] Leave Club',
  props<{ clubId: number }>()
);

export const leaveClubSuccess = createAction(
  '[Social] Leave Club Success',
  props<{ clubId: number }>()
);

export const updateClub = createAction(
  '[Social] Update Club',
  props<{ 
    clubId: number;
    updates: {
      name?: string;
      description?: string;
      rules?: string;
      tags?: string[];
      imageUrl?: string;
    };
  }>()
);

export const updateClubSuccess = createAction(
  '[Social] Update Club Success',
  props<{ club: any }>()
);

// Club Members Management
export const loadClubMembers = createAction(
  '[Social] Load Club Members',
  props<{ 
    clubId: number;
    role?: 'all' | 'owner' | 'admin' | 'moderator' | 'member';
    page?: number;
  }>()
);

export const loadClubMembersSuccess = createAction(
  '[Social] Load Club Members Success',
  props<{ 
    clubId: number;
    members: Array<{
      id: number;
      userId: number;
      user: {
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
        level: number;
        lastActiveDate: Date;
      };
      role: 'owner' | 'admin' | 'moderator' | 'member';
      joinedAt: Date;
      isActive: boolean;
      contributions: {
        posts: number;
        activities: number;
        challenges: number;
      };
    }>;
    hasMore: boolean;
  }>()
);

export const updateMemberRole = createAction(
  '[Social] Update Member Role',
  props<{ 
    clubId: number;
    userId: number;
    newRole: 'admin' | 'moderator' | 'member';
  }>()
);

export const updateMemberRoleSuccess = createAction(
  '[Social] Update Member Role Success',
  props<{ clubId: number; userId: number; newRole: string }>()
);

export const removeMember = createAction(
  '[Social] Remove Member',
  props<{ clubId: number; userId: number; reason?: string }>()
);

export const removeMemberSuccess = createAction(
  '[Social] Remove Member Success',
  props<{ clubId: number; userId: number }>()
);

// Social Challenges and Competitions
export const loadSocialChallenges = createAction(
  '[Social] Load Social Challenges',
  props<{ 
    type?: 'active' | 'completed' | 'available' | 'invited';
    scope?: 'friends' | 'clubs' | 'public';
    page?: number;
  }>()
);

export const loadSocialChallengesSuccess = createAction(
  '[Social] Load Social Challenges Success',
  props<{ 
    challenges: Array<{
      id: number;
      title: string;
      description: string;
      type: 'duel' | 'group' | 'club' | 'public';
      category: string;
      creatorId: number;
      creator: {
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
      };
      participants: Array<{
        userId: number;
        userName: string;
        userAvatar?: string;
        progress: number;
        rank: number;
        joinedAt: Date;
      }>;
      maxParticipants?: number;
      targetValue: number;
      unit: string;
      duration: number; // in days
      startDate: Date;
      endDate: Date;
      status: 'pending' | 'active' | 'completed' | 'cancelled';
      isParticipating: boolean;
      userProgress?: number;
      userRank?: number;
      rewards: {
        winner: {
          xp: number;
          badge?: any;
          coins?: number;
        };
        participants: {
          xp: number;
          coins?: number;
        };
      };
      clubId?: number;
      isInviteOnly: boolean;
    }>;
    hasMore: boolean;
  }>()
);

export const createSocialChallenge = createAction(
  '[Social] Create Social Challenge',
  props<{ 
    challengeData: {
      title: string;
      description: string;
      type: 'duel' | 'group' | 'club' | 'public';
      category: string;
      targetValue: number;
      unit: string;
      duration: number;
      maxParticipants?: number;
      isInviteOnly: boolean;
      clubId?: number;
      invitedUsers?: number[];
    };
  }>()
);

export const createSocialChallengeSuccess = createAction(
  '[Social] Create Social Challenge Success',
  props<{ challenge: any }>()
);

export const joinSocialChallenge = createAction(
  '[Social] Join Social Challenge',
  props<{ challengeId: number }>()
);

export const joinSocialChallengeSuccess = createAction(
  '[Social] Join Social Challenge Success',
  props<{ challengeId: number; participation: any }>()
);

export const leaveSocialChallenge = createAction(
  '[Social] Leave Social Challenge',
  props<{ challengeId: number }>()
);

export const leaveSocialChallengeSuccess = createAction(
  '[Social] Leave Social Challenge Success',
  props<{ challengeId: number }>()
);

export const inviteToChallenge = createAction(
  '[Social] Invite To Challenge',
  props<{ 
    challengeId: number;
    userIds: number[];
    message?: string;
  }>()
);

export const inviteToChallengeSuccess = createAction(
  '[Social] Invite To Challenge Success',
  props<{ challengeId: number; invitedUserIds: number[] }>()
);

// Social Discovery
export const loadSocialRecommendations = createAction('[Social] Load Social Recommendations');

export const loadSocialRecommendationsSuccess = createAction(
  '[Social] Load Social Recommendations Success',
  props<{ 
    recommendations: {
      users: Array<{
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
        level: number;
        mutualFriends: number;
        mutualClubs: number;
        similarActivities: string[];
        reason: 'mutual_friends' | 'similar_activities' | 'same_location' | 'similar_goals';
      }>;
      clubs: Array<{
        id: number;
        name: string;
        description: string;
        memberCount: number;
        category: string;
        imageUrl?: string;
        reason: 'friends_joined' | 'similar_interests' | 'location_based' | 'popular';
        friendsInClub: number;
      }>;
      activities: Array<{
        id: number;
        name: string;
        category: string;
        popularityScore: number;
        friendsParticipating: number;
        reason: 'trending' | 'friends_activity' | 'skill_level_match';
      }>;
    };
  }>()
);

export const loadSocialRecommendationsFailure = createAction(
  '[Social] Load Social Recommendations Failure',
  props<{ error: string }>()
);

export const searchUsers = createAction(
  '[Social] Search Users',
  props<{ 
    query: string;
    filters?: {
      location?: string;
      level?: { min: number; max: number };
      activities?: string[];
      clubs?: number[];
    };
  }>()
);

export const searchUsersSuccess = createAction(
  '[Social] Search Users Success',
  props<{ 
    users: Array<{
      id: number;
      firstName: string;
      lastName: string;
      avatarUrl?: string;
      level: number;
      location?: string;
      mutualFriends: number;
      isFollowing: boolean;
      isFriend: boolean;
    }>;
    hasMore: boolean;
  }>()
);

export const searchClubs = createAction(
  '[Social] Search Clubs',
  props<{ 
    query: string;
    filters?: {
      category?: string;
      location?: string;
      memberCount?: { min: number; max: number };
      isPublic?: boolean;
    };
  }>()
);

export const searchClubsSuccess = createAction(
  '[Social] Search Clubs Success',
  props<{ 
    clubs: Array<{
      id: number;
      name: string;
      description: string;
      memberCount: number;
      category: string;
      imageUrl?: string;
      isPublic: boolean;
      isJoined: boolean;
    }>;
    hasMore: boolean;
  }>()
);

// Messaging and Chat
export const loadConversations = createAction('[Social] Load Conversations');

export const loadConversationsSuccess = createAction(
  '[Social] Load Conversations Success',
  props<{ 
    conversations: Array<{
      id: number;
      type: 'direct' | 'group' | 'club';
      participants: Array<{
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
        isOnline: boolean;
      }>;
      lastMessage: {
        id: number;
        content: string;
        senderId: number;
        timestamp: Date;
        messageType: 'text' | 'image' | 'activity' | 'challenge_invite';
      };
      unreadCount: number;
      isArchived: boolean;
      isPinned: boolean;
      clubId?: number;
      clubName?: string;
    }>;
  }>()
);

export const loadMessages = createAction(
  '[Social] Load Messages',
  props<{ 
    conversationId: number;
    page?: number;
    limit?: number;
  }>()
);

export const loadMessagesSuccess = createAction(
  '[Social] Load Messages Success',
  props<{ 
    conversationId: number;
    messages: Array<{
      id: number;
      conversationId: number;
      senderId: number;
      sender: {
        id: number;
        firstName: string;
        lastName: string;
        avatarUrl?: string;
      };
      content: string;
      messageType: 'text' | 'image' | 'activity' | 'challenge_invite' | 'system';
      attachments?: Array<{
        type: 'image' | 'activity' | 'goal';
        url: string;
        metadata: any;
      }>;
      isRead: boolean;
      timestamp: Date;
      editedAt?: Date;
    }>;
    hasMore: boolean;
  }>()
);

export const sendMessage = createAction(
  '[Social] Send Message',
  props<{ 
    conversationId: number;
    content: string;
    messageType?: 'text' | 'image' | 'activity';
    attachments?: Array<{
      type: 'image' | 'activity' | 'goal';
      url: string;
      metadata: any;
    }>;
  }>()
);

export const sendMessageSuccess = createAction(
  '[Social] Send Message Success',
  props<{ message: any }>()
);

export const markMessagesAsRead = createAction(
  '[Social] Mark Messages As Read',
  props<{ conversationId: number; messageIds: number[] }>()
);

export const markMessagesAsReadSuccess = createAction(
  '[Social] Mark Messages As Read Success',
  props<{ conversationId: number; messageIds: number[] }>()
);

// UI State Management
export const setSelectedPost = createAction(
  '[Social] Set Selected Post',
  props<{ post: any | null }>()
);

export const setSelectedClub = createAction(
  '[Social] Set Selected Club',
  props<{ club: any | null }>()
);

export const setSelectedConversation = createAction(
  '[Social] Set Selected Conversation',
  props<{ conversationId: number | null }>()
);

export const setSocialFilters = createAction(
  '[Social] Set Social Filters',
  props<{ 
    filters: {
      feedType?: 'following' | 'friends' | 'public' | 'clubs';
      activityTypes?: string[];
      location?: string;
      dateRange?: { start: Date; end: Date };
    };
  }>()
);

export const clearSocialError = createAction('[Social] Clear Error');

export const refreshSocialData = createAction('[Social] Refresh Social Data');

// Real-time updates (would be triggered by WebSocket/SignalR)
export const receiveLivePost = createAction(
  '[Social] Receive Live Post',
  props<{ post: any }>()
);

export const receiveLiveMessage = createAction(
  '[Social] Receive Live Message',
  props<{ message: any }>()
);

export const receiveLiveLike = createAction(
  '[Social] Receive Live Like',
  props<{ postId: number; userId: number; likesCount: number }>()
);

export const receiveLiveComment = createAction(
  '[Social] Receive Live Comment',
  props<{ comment: any }>()
);

export const receiveUserOnlineStatus = createAction(
  '[Social] Receive User Online Status',
  props<{ userId: number; isOnline: boolean; lastActiveDate?: Date }>()
);
