import { createReducer, on } from '@ngrx/store';
import * as SocialActions from './social.actions';

/**
 * Social State Interface - the comprehensive social fitness community system.
 * Think of this as a digital fitness community center that manages all social
 * interactions, friendships, groups, challenges, and conversations!
 */
export interface SocialState {
  // Activity Feed
  activityFeed: {
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
      activity?: any;
      goalId?: number;
      goal?: any;
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
    feedLoaded: boolean;
  };
  
  // Selected post for detailed view
  selectedPost: any | null;
  
  // Post comments
  postComments: {
    [postId: number]: {
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
      loaded: boolean;
    };
  };
  
  // Friends and Following
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
  friendsLoaded: boolean;
  
  friendRequests: {
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
    loaded: boolean;
  };
  
  // Clubs and Groups
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
  clubsLoaded: boolean;
  clubsHasMore: boolean;
  
  selectedClub: any | null;
  
  clubMembers: {
    [clubId: number]: {
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
      loaded: boolean;
    };
  };
  
  // Social Challenges
  socialChallenges: Array<{
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
    duration: number;
    startDate: Date;
    endDate: Date;
    status: 'pending' | 'active' | 'completed' | 'cancelled';
    isParticipating: boolean;
    userProgress?: number;
    userRank?: number;
    rewards: any;
    clubId?: number;
    isInviteOnly: boolean;
  }>;
  socialChallengesLoaded: boolean;
  socialChallengesHasMore: boolean;
  
  // Social Discovery and Recommendations
  socialRecommendations: {
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
    loaded: boolean;
  };
  
  // Search Results
  searchResults: {
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
    hasMoreUsers: boolean;
    hasMoreClubs: boolean;
  };
  
  // Messaging and Conversations
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
  conversationsLoaded: boolean;
  
  selectedConversationId: number | null;
  
  messages: {
    [conversationId: number]: {
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
      loaded: boolean;
    };
  };
  
  // UI State and Filters
  filters: {
    feedType: 'following' | 'friends' | 'public' | 'clubs';
    activityTypes: string[];
    location?: string;
    dateRange?: { start: Date; end: Date };
  };
  
  // Loading states
  isLoading: boolean;
  isCreatingPost: boolean;
  isJoiningClub: boolean;
  isCreatingChallenge: boolean;
  isSendingMessage: boolean;
  isLoadingRecommendations: boolean;
  
  // Error handling
  error: string | null;
}

/**
 * Initial state - a fresh social fitness community ready to connect people through fitness.
 */
export const initialState: SocialState = {
  activityFeed: {
    posts: [],
    hasMore: false,
    totalCount: 0,
    feedLoaded: false
  },
  
  selectedPost: null,
  
  postComments: {},
  
  friends: [],
  friendsLoaded: false,
  
  friendRequests: {
    incoming: [],
    outgoing: [],
    loaded: false
  },
  
  clubs: [],
  clubsLoaded: false,
  clubsHasMore: false,
  
  selectedClub: null,
  
  clubMembers: {},
  
  socialChallenges: [],
  socialChallengesLoaded: false,
  socialChallengesHasMore: false,
  
  socialRecommendations: {
    users: [],
    clubs: [],
    activities: [],
    loaded: false
  },
  
  searchResults: {
    users: [],
    clubs: [],
    hasMoreUsers: false,
    hasMoreClubs: false
  },
  
  conversations: [],
  conversationsLoaded: false,
  
  selectedConversationId: null,
  
  messages: {},
  
  filters: {
    feedType: 'following',
    activityTypes: []
  },
  
  isLoading: false,
  isCreatingPost: false,
  isJoiningClub: false,
  isCreatingChallenge: false,
  isSendingMessage: false,
  isLoadingRecommendations: false,
  
  error: null
};

/**
 * Social Reducer - the community manager who handles all social interactions and connections.
 * Each action is like a different social activity: "posted update", "made friend", 
 * "joined club", "sent message", etc.
 */
export const socialReducer = createReducer(
  initialState,

  // Activity Feed Management
  on(SocialActions.loadActivityFeed, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(SocialActions.loadActivityFeedSuccess, (state, { posts, hasMore, totalCount }) => ({
    ...state,
    activityFeed: {
      posts,
      hasMore,
      totalCount,
      feedLoaded: true
    },
    isLoading: false,
    error: null
  })),

  on(SocialActions.loadActivityFeedFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(SocialActions.createPost, (state) => ({
    ...state,
    isCreatingPost: true,
    error: null
  })),

  on(SocialActions.createPostSuccess, (state, { post }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: [post, ...state.activityFeed.posts],
      totalCount: state.activityFeed.totalCount + 1
    },
    isCreatingPost: false,
    error: null
  })),

  on(SocialActions.createPostFailure, (state, { error }) => ({
    ...state,
    isCreatingPost: false,
    error
  })),

  on(SocialActions.updatePostSuccess, (state, { post }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.map(p => p.id === post.id ? post : p)
    },
    selectedPost: state.selectedPost?.id === post.id ? post : state.selectedPost
  })),

  on(SocialActions.deletePostSuccess, (state, { postId }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.filter(p => p.id !== postId),
      totalCount: state.activityFeed.totalCount - 1
    },
    selectedPost: state.selectedPost?.id === postId ? null : state.selectedPost
  })),

  // Post Interactions
  on(SocialActions.likePostSuccess, (state, { postId, likesCount }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.map(p => 
        p.id === postId 
          ? { ...p, likes: likesCount, isLiked: true }
          : p
      )
    },
    selectedPost: state.selectedPost?.id === postId 
      ? { ...state.selectedPost, likes: likesCount, isLiked: true }
      : state.selectedPost
  })),

  on(SocialActions.unlikePostSuccess, (state, { postId, likesCount }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.map(p => 
        p.id === postId 
          ? { ...p, likes: likesCount, isLiked: false }
          : p
      )
    },
    selectedPost: state.selectedPost?.id === postId 
      ? { ...state.selectedPost, likes: likesCount, isLiked: false }
      : state.selectedPost
  })),

  on(SocialActions.savePostSuccess, (state, { postId }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.map(p => 
        p.id === postId ? { ...p, isSaved: true } : p
      )
    }
  })),

  on(SocialActions.unsavePostSuccess, (state, { postId }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.map(p => 
        p.id === postId ? { ...p, isSaved: false } : p
      )
    }
  })),

  on(SocialActions.sharePostSuccess, (state, { postId, sharesCount }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.map(p => 
        p.id === postId ? { ...p, shares: sharesCount } : p
      )
    }
  })),

  // Comments Management
  on(SocialActions.loadPostCommentsSuccess, (state, { postId, comments, hasMore }) => ({
    ...state,
    postComments: {
      ...state.postComments,
      [postId]: {
        comments,
        hasMore,
        loaded: true
      }
    }
  })),

  on(SocialActions.addCommentSuccess, (state, { comment, postId }) => {
    const existingComments = state.postComments[postId]?.comments || [];
    
    return {
      ...state,
      postComments: {
        ...state.postComments,
        [postId]: {
          comments: [...existingComments, comment],
          hasMore: state.postComments[postId]?.hasMore || false,
          loaded: true
        }
      },
      activityFeed: {
        ...state.activityFeed,
        posts: state.activityFeed.posts.map(p => 
          p.id === postId ? { ...p, comments: p.comments + 1 } : p
        )
      }
    };
  }),

  on(SocialActions.updateCommentSuccess, (state, { comment }) => {
    const postId = comment.postId;
    const existingComments = state.postComments[postId]?.comments || [];
    
    return {
      ...state,
      postComments: {
        ...state.postComments,
        [postId]: {
          ...state.postComments[postId],
          comments: existingComments.map(c => c.id === comment.id ? comment : c)
        }
      }
    };
  }),

  on(SocialActions.deleteCommentSuccess, (state, { commentId, postId }) => {
    const existingComments = state.postComments[postId]?.comments || [];
    
    return {
      ...state,
      postComments: {
        ...state.postComments,
        [postId]: {
          ...state.postComments[postId],
          comments: existingComments.filter(c => c.id !== commentId)
        }
      },
      activityFeed: {
        ...state.activityFeed,
        posts: state.activityFeed.posts.map(p => 
          p.id === postId ? { ...p, comments: p.comments - 1 } : p
        )
      }
    };
  }),

  on(SocialActions.likeCommentSuccess, (state, { commentId, likesCount }) => {
    // Update comment likes across all posts
    const updatedComments = { ...state.postComments };
    
    Object.keys(updatedComments).forEach(postId => {
      const postComments = updatedComments[+postId];
      if (postComments) {
        updatedComments[+postId] = {
          ...postComments,
          comments: postComments.comments.map(c => 
            c.id === commentId 
              ? { ...c, likes: likesCount, isLiked: true }
              : c
          )
        };
      }
    });
    
    return {
      ...state,
      postComments: updatedComments
    };
  }),

  // Friends and Following Management
  on(SocialActions.loadFriends, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(SocialActions.loadFriendsSuccess, (state, { friends }) => ({
    ...state,
    friends,
    friendsLoaded: true,
    isLoading: false,
    error: null
  })),

  on(SocialActions.loadFriendsFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(SocialActions.loadFriendRequestsSuccess, (state, { incoming, outgoing }) => ({
    ...state,
    friendRequests: {
      incoming,
      outgoing,
      loaded: true
    }
  })),

  on(SocialActions.sendFriendRequestSuccess, (state, { userId, requestId }) => ({
    ...state,
    friendRequests: {
      ...state.friendRequests,
      outgoing: [
        ...state.friendRequests.outgoing,
        {
          id: requestId,
          toUserId: userId,
          user: { id: userId, firstName: '', lastName: '', avatarUrl: undefined },
          createdAt: new Date()
        }
      ]
    }
  })),

  on(SocialActions.acceptFriendRequestSuccess, (state, { requestId, newFriend }) => ({
    ...state,
    friendRequests: {
      ...state.friendRequests,
      incoming: state.friendRequests.incoming.filter(req => req.id !== requestId)
    },
    friends: [...state.friends, newFriend]
  })),

  on(SocialActions.declineFriendRequestSuccess, (state, { requestId }) => ({
    ...state,
    friendRequests: {
      ...state.friendRequests,
      incoming: state.friendRequests.incoming.filter(req => req.id !== requestId)
    }
  })),

  on(SocialActions.removeFriendSuccess, (state, { userId }) => ({
    ...state,
    friends: state.friends.filter(friend => friend.id !== userId)
  })),

  on(SocialActions.followUserSuccess, (state, { userId }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.map(p => 
        p.userId === userId 
          ? { ...p, user: { ...p.user, isFollowing: true } }
          : p
      )
    }
  })),

  on(SocialActions.unfollowUserSuccess, (state, { userId }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.map(p => 
        p.userId === userId 
          ? { ...p, user: { ...p.user, isFollowing: false } }
          : p
      )
    }
  })),

  // Clubs Management
  on(SocialActions.loadClubs, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(SocialActions.loadClubsSuccess, (state, { clubs, hasMore }) => ({
    ...state,
    clubs,
    clubsLoaded: true,
    clubsHasMore: hasMore,
    isLoading: false,
    error: null
  })),

  on(SocialActions.loadClubsFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(SocialActions.loadClubDetailsSuccess, (state, { club }) => ({
    ...state,
    selectedClub: club,
    // Also update in clubs list if present
    clubs: state.clubs.map(c => c.id === club.id ? { ...c, ...club } : c)
  })),

  on(SocialActions.createClubSuccess, (state, { club }) => ({
    ...state,
    clubs: [club, ...state.clubs],
    selectedClub: club
  })),

  on(SocialActions.joinClub, (state) => ({
    ...state,
    isJoiningClub: true,
    error: null
  })),

  on(SocialActions.joinClubSuccess, (state, { clubId, membership }) => ({
    ...state,
    clubs: state.clubs.map(club => 
      club.id === clubId 
        ? { 
            ...club, 
            isJoined: true, 
            memberCount: club.memberCount + 1,
            role: membership.role,
            joinedAt: membership.joinedAt
          }
        : club
    ),
    selectedClub: state.selectedClub?.id === clubId 
      ? { 
          ...state.selectedClub, 
          isJoined: true, 
          memberCount: state.selectedClub.memberCount + 1,
          role: membership.role,
          joinedAt: membership.joinedAt
        }
      : state.selectedClub,
    isJoiningClub: false,
    error: null
  })),

  on(SocialActions.leaveClubSuccess, (state, { clubId }) => ({
    ...state,
    clubs: state.clubs.map(club => 
      club.id === clubId 
        ? { 
            ...club, 
            isJoined: false, 
            memberCount: club.memberCount - 1,
            role: undefined,
            joinedAt: undefined
          }
        : club
    ),
    selectedClub: state.selectedClub?.id === clubId 
      ? { 
          ...state.selectedClub, 
          isJoined: false, 
          memberCount: state.selectedClub.memberCount - 1,
          role: undefined,
          joinedAt: undefined
        }
      : state.selectedClub
  })),

  on(SocialActions.updateClubSuccess, (state, { club }) => ({
    ...state,
    clubs: state.clubs.map(c => c.id === club.id ? club : c),
    selectedClub: state.selectedClub?.id === club.id ? club : state.selectedClub
  })),

  // Club Members Management
  on(SocialActions.loadClubMembersSuccess, (state, { clubId, members, hasMore }) => ({
    ...state,
    clubMembers: {
      ...state.clubMembers,
      [clubId]: {
        members,
        hasMore,
        loaded: true
      }
    }
  })),

  on(SocialActions.updateMemberRoleSuccess, (state, { clubId, userId, newRole }) => {
    const clubMemberData = state.clubMembers[clubId];
    if (!clubMemberData) return state;
    
    return {
      ...state,
      clubMembers: {
        ...state.clubMembers,
        [clubId]: {
          ...clubMemberData,
          members: clubMemberData.members.map(member => 
            member.userId === userId 
              ? { ...member, role: newRole as any }
              : member
          )
        }
      }
    };
  }),

  on(SocialActions.removeMemberSuccess, (state, { clubId, userId }) => {
    const clubMemberData = state.clubMembers[clubId];
    if (!clubMemberData) return state;
    
    return {
      ...state,
      clubMembers: {
        ...state.clubMembers,
        [clubId]: {
          ...clubMemberData,
          members: clubMemberData.members.filter(member => member.userId !== userId)
        }
      },
      // Update club member count
      clubs: state.clubs.map(club => 
        club.id === clubId 
          ? { ...club, memberCount: club.memberCount - 1 }
          : club
      ),
      selectedClub: state.selectedClub?.id === clubId 
        ? { ...state.selectedClub, memberCount: state.selectedClub.memberCount - 1 }
        : state.selectedClub
    };
  }),

  // Social Challenges Management
  on(SocialActions.loadSocialChallenges, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(SocialActions.loadSocialChallengesSuccess, (state, { challenges, hasMore }) => ({
    ...state,
    socialChallenges: challenges,
    socialChallengesLoaded: true,
    socialChallengesHasMore: hasMore,
    isLoading: false,
    error: null
  })),

  on(SocialActions.createSocialChallenge, (state) => ({
    ...state,
    isCreatingChallenge: true,
    error: null
  })),

  on(SocialActions.createSocialChallengeSuccess, (state, { challenge }) => ({
    ...state,
    socialChallenges: [challenge, ...state.socialChallenges],
    isCreatingChallenge: false,
    error: null
  })),

  on(SocialActions.joinSocialChallengeSuccess, (state, { challengeId, participation }) => ({
    ...state,
    socialChallenges: state.socialChallenges.map(challenge => 
      challenge.id === challengeId 
        ? { 
            ...challenge, 
            isParticipating: true,
            participants: [...challenge.participants, participation]
          }
        : challenge
    )
  })),

  on(SocialActions.leaveSocialChallengeSuccess, (state, { challengeId }) => ({
    ...state,
    socialChallenges: state.socialChallenges.map(challenge => 
      challenge.id === challengeId 
        ? { 
            ...challenge, 
            isParticipating: false,
            participants: challenge.participants.filter(p => p.userId !== /* current user id would come from auth state */)
          }
        : challenge
    )
  })),

  // Social Recommendations
  on(SocialActions.loadSocialRecommendations, (state) => ({
    ...state,
    isLoadingRecommendations: true,
    error: null
  })),

  on(SocialActions.loadSocialRecommendationsSuccess, (state, { recommendations }) => ({
    ...state,
    socialRecommendations: {
      ...recommendations,
      loaded: true
    },
    isLoadingRecommendations: false,
    error: null
  })),

  on(SocialActions.loadSocialRecommendationsFailure, (state, { error }) => ({
    ...state,
    isLoadingRecommendations: false,
    error
  })),

  // Search Results
  on(SocialActions.searchUsersSuccess, (state, { users, hasMore }) => ({
    ...state,
    searchResults: {
      ...state.searchResults,
      users,
      hasMoreUsers: hasMore
    }
  })),

  on(SocialActions.searchClubsSuccess, (state, { clubs, hasMore }) => ({
    ...state,
    searchResults: {
      ...state.searchResults,
      clubs,
      hasMoreClubs: hasMore
    }
  })),

  // Messaging and Conversations
  on(SocialActions.loadConversationsSuccess, (state, { conversations }) => ({
    ...state,
    conversations,
    conversationsLoaded: true
  })),

  on(SocialActions.loadMessagesSuccess, (state, { conversationId, messages, hasMore }) => ({
    ...state,
    messages: {
      ...state.messages,
      [conversationId]: {
        messages,
        hasMore,
        loaded: true
      }
    }
  })),

  on(SocialActions.sendMessage, (state) => ({
    ...state,
    isSendingMessage: true,
    error: null
  })),

  on(SocialActions.sendMessageSuccess, (state, { message }) => {
    const conversationId = message.conversationId;
    const existingMessages = state.messages[conversationId]?.messages || [];
    
    return {
      ...state,
      messages: {
        ...state.messages,
        [conversationId]: {
          messages: [...existingMessages, message],
          hasMore: state.messages[conversationId]?.hasMore || false,
          loaded: true
        }
      },
      conversations: state.conversations.map(conv => 
        conv.id === conversationId 
          ? { 
              ...conv, 
              lastMessage: {
                id: message.id,
                content: message.content,
                senderId: message.senderId,
                timestamp: message.timestamp,
                messageType: message.messageType
              }
            }
          : conv
      ),
      isSendingMessage: false,
      error: null
    };
  }),

  on(SocialActions.markMessagesAsReadSuccess, (state, { conversationId, messageIds }) => {
    const conversationMessages = state.messages[conversationId];
    if (!conversationMessages) return state;
    
    return {
      ...state,
      messages: {
        ...state.messages,
        [conversationId]: {
          ...conversationMessages,
          messages: conversationMessages.messages.map(msg => 
            messageIds.includes(msg.id) ? { ...msg, isRead: true } : msg
          )
        }
      },
      conversations: state.conversations.map(conv => 
        conv.id === conversationId 
          ? { ...conv, unreadCount: Math.max(0, conv.unreadCount - messageIds.length) }
          : conv
      )
    };
  }),

  // UI State Management
  on(SocialActions.setSelectedPost, (state, { post }) => ({
    ...state,
    selectedPost: post
  })),

  on(SocialActions.setSelectedClub, (state, { club }) => ({
    ...state,
    selectedClub: club
  })),

  on(SocialActions.setSelectedConversation, (state, { conversationId }) => ({
    ...state,
    selectedConversationId: conversationId
  })),

  on(SocialActions.setSocialFilters, (state, { filters }) => ({
    ...state,
    filters: { ...state.filters, ...filters }
  })),

  on(SocialActions.clearSocialError, (state) => ({
    ...state,
    error: null
  })),

  // Refresh all social data
  on(SocialActions.refreshSocialData, (state) => ({
    ...state,
    activityFeed: { ...state.activityFeed, feedLoaded: false },
    friendsLoaded: false,
    clubsLoaded: false,
    socialChallengesLoaded: false,
    conversationsLoaded: false,
    isLoading: true
  })),

  // Real-time updates
  on(SocialActions.receiveLivePost, (state, { post }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: [post, ...state.activityFeed.posts],
      totalCount: state.activityFeed.totalCount + 1
    }
  })),

  on(SocialActions.receiveLiveMessage, (state, { message }) => {
    const conversationId = message.conversationId;
    const existingMessages = state.messages[conversationId]?.messages || [];
    
    return {
      ...state,
      messages: {
        ...state.messages,
        [conversationId]: {
          messages: [...existingMessages, message],
          hasMore: state.messages[conversationId]?.hasMore || false,
          loaded: true
        }
      },
      conversations: state.conversations.map(conv => 
        conv.id === conversationId 
          ? { 
              ...conv, 
              lastMessage: {
                id: message.id,
                content: message.content,
                senderId: message.senderId,
                timestamp: message.timestamp,
                messageType: message.messageType
              },
              unreadCount: conv.unreadCount + 1
            }
          : conv
      )
    };
  }),

  on(SocialActions.receiveLiveLike, (state, { postId, userId, likesCount }) => ({
    ...state,
    activityFeed: {
      ...state.activityFeed,
      posts: state.activityFeed.posts.map(p => 
        p.id === postId ? { ...p, likes: likesCount } : p
      )
    }
  })),

  on(SocialActions.receiveLiveComment, (state, { comment }) => {
    const postId = comment.postId;
    const existingComments = state.postComments[postId]?.comments || [];
    
    return {
      ...state,
      postComments: {
        ...state.postComments,
        [postId]: {
          comments: [...existingComments, comment],
          hasMore: state.postComments[postId]?.hasMore || false,
          loaded: true
        }
      },
      activityFeed: {
        ...state.activityFeed,
        posts: state.activityFeed.posts.map(p => 
          p.id === postId ? { ...p, comments: p.comments + 1 } : p
        )
      }
    };
  }),

  on(SocialActions.receiveUserOnlineStatus, (state, { userId, isOnline, lastActiveDate }) => ({
    ...state,
    friends: state.friends.map(friend => 
      friend.id === userId 
        ? { 
            ...friend, 
            isOnline, 
            lastActiveDate: lastActiveDate || friend.lastActiveDate 
          }
        : friend
    ),
    conversations: state.conversations.map(conv => ({
      ...conv,
      participants: conv.participants.map(participant => 
        participant.id === userId 
          ? { ...participant, isOnline }
          : participant
      )
    }))
  }))
);
