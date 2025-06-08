/**
 * Social Models - representing the community and social aspects of fitness tracking.
 * Think of these as the social media components of a fitness community where
 * users connect, share achievements, and motivate each other.
 */

import { User, UserActivity, UserGoal } from './user.model';

/**
 * Social Post - a user's shared fitness content
 * Like a social media post but focused on fitness achievements and activities
 */
export interface SocialPost {
  id: number;
  userId: number;
  user: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl' | 'level'>;
  type: PostType;
  content: string;
  mediaUrls: string[]; // Photos or videos
  
  // Related fitness data
  activityId?: number;
  activity?: UserActivity;
  goalId?: number;
  goal?: UserGoal;
  achievementId?: number;
  badgeId?: number;
  
  // Engagement metrics
  likes: number;
  comments: number;
  shares: number;
  
  // User interactions
  isLikedByCurrentUser: boolean;
  isSharedByCurrentUser: boolean;
  
  // Visibility and privacy
  visibility: PostVisibility;
  allowComments: boolean;
  
  // Location data
  location?: {
    name: string;
    latitude?: number;
    longitude?: number;
  };
  
  // Timestamps
  createdAt: Date;
  updatedAt?: Date;
}

/**
 * Post Comment - user comments on social posts
 * Like replies and discussions on fitness posts
 */
export interface PostComment {
  id: number;
  postId: number;
  userId: number;
  user: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl'>;
  content: string;
  parentCommentId?: number; // For threaded comments
  replies?: PostComment[];
  likes: number;
  isLikedByCurrentUser: boolean;
  createdAt: Date;
  updatedAt?: Date;
}

/**
 * Post Like - tracking who liked what posts
 */
export interface PostLike {
  id: number;
  postId: number;
  userId: number;
  user: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl'>;
  createdAt: Date;
}

/**
 * Friend Connection - relationship between users
 * Like having workout buddies or fitness connections
 */
export interface Friendship {
  id: number;
  requesterId: number;
  addresseeId: number;
  requester: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl' | 'level'>;
  addressee: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl' | 'level'>;
  status: FriendshipStatus;
  createdAt: Date;
  acceptedAt?: Date;
}

/**
 * Friend Request - pending friendship invitations
 */
export interface FriendRequest {
  id: number;
  fromUserId: number;
  toUserId: number;
  fromUser: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl' | 'level'>;
  toUser: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl' | 'level'>;
  message?: string;
  status: 'pending' | 'accepted' | 'declined' | 'cancelled';
  createdAt: Date;
  respondedAt?: Date;
}

/**
 * Group/Club - fitness communities and clubs
 * Like fitness clubs, running groups, or workout communities
 */
export interface FitnessGroup {
  id: number;
  name: string;
  description: string;
  imageUrl?: string;
  coverImageUrl?: string;
  
  // Group settings
  type: GroupType;
  privacy: GroupPrivacy;
  category: GroupCategory;
  location?: string;
  
  // Membership
  memberCount: number;
  maxMembers?: number;
  isJoined: boolean;
  membershipStatus?: 'member' | 'admin' | 'moderator' | 'pending' | 'banned';
  
  // Activity
  lastActivityAt: Date;
  recentPosts: number; // Posts in last 7 days
  
  // Rules and guidelines
  rules?: string;
  tags: string[];
  
  // Creation info
  createdBy: number;
  creator: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl'>;
  createdAt: Date;
  updatedAt?: Date;
}

/**
 * Group Membership - user's relationship with a group
 */
export interface GroupMembership {
  id: number;
  groupId: number;
  userId: number;
  user: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl' | 'level'>;
  role: GroupRole;
  joinedAt: Date;
  lastActiveAt?: Date;
  
  // Contributions
  postsCount: number;
  commentsCount: number;
  
  // Status
  isActive: boolean;
  isMuted: boolean;
}

/**
 * Group Event - organized fitness events within groups
 * Like group workouts, challenges, or meet-ups
 */
export interface GroupEvent {
  id: number;
  groupId: number;
  group: Pick<FitnessGroup, 'id' | 'name' | 'imageUrl'>;
  organizerId: number;
  organizer: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl'>;
  
  title: string;
  description: string;
  imageUrl?: string;
  
  // Event details
  type: EventType;
  category: 'workout' | 'challenge' | 'meetup' | 'competition' | 'social';
  difficulty?: 'beginner' | 'intermediate' | 'advanced';
  
  // Timing
  startTime: Date;
  endTime: Date;
  timeZone: string;
  
  // Location
  location?: {
    type: 'physical' | 'virtual';
    name: string;
    address?: string;
    coordinates?: { latitude: number; longitude: number };
    virtualLink?: string;
  };
  
  // Participation
  maxParticipants?: number;
  currentParticipants: number;
  isParticipating: boolean;
  participantStatus?: 'going' | 'maybe' | 'not_going';
  
  // Event data
  requirements?: string[];
  whatToBring?: string[];
  
  createdAt: Date;
  updatedAt?: Date;
}

/**
 * Challenge - social fitness challenges between users
 * Like competitions or friendly contests between friends or groups
 */
export interface SocialChallenge {
  id: number;
  title: string;
  description: string;
  imageUrl?: string;
  
  // Challenge configuration
  type: ChallengeType;
  category: ChallengeCategory;
  difficulty: 'easy' | 'medium' | 'hard' | 'extreme';
  
  // Metrics and goals
  metric: string; // 'steps', 'distance', 'duration', 'calories', etc.
  targetValue: number;
  unit: string;
  
  // Timing
  startDate: Date;
  endDate: Date;
  duration: number; // in days
  
  // Participation
  createdBy: number;
  creator: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl'>;
  maxParticipants?: number;
  currentParticipants: number;
  
  // Privacy and access
  privacy: 'public' | 'friends' | 'group' | 'private';
  groupId?: number;
  inviteOnly: boolean;
  
  // Rewards
  rewards?: {
    winner: { xp: number; coins: number; badge?: any };
    participation: { xp: number; coins: number };
  };
  
  // Status
  status: 'upcoming' | 'active' | 'completed' | 'cancelled';
  isParticipating: boolean;
  
  createdAt: Date;
  updatedAt?: Date;
}

/**
 * Challenge Participation - user's involvement in a challenge
 */
export interface ChallengeParticipation {
  id: number;
  challengeId: number;
  challenge: Pick<SocialChallenge, 'id' | 'title' | 'metric' | 'targetValue' | 'unit' | 'endDate'>;
  userId: number;
  user: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl' | 'level'>;
  
  // Progress tracking
  currentProgress: number;
  progressPercentage: number;
  rank?: number;
  
  // Participation details
  joinedAt: Date;
  lastUpdatedAt?: Date;
  completedAt?: Date;
  
  // Social features
  isPublic: boolean;
  allowCheering: boolean;
  
  // Status
  status: 'active' | 'completed' | 'withdrawn';
}

/**
 * Activity Feed Item - items that appear in social feeds
 * Like a unified social media feed for fitness activities
 */
export interface FeedItem {
  id: string;
  type: FeedItemType;
  timestamp: Date;
  
  // User who performed the action
  actor: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl' | 'level'>;
  
  // The content of the feed item
  content: {
    text?: string;
    action?: string;
    subject?: any; // Activity, Goal, Achievement, etc.
  };
  
  // Related objects
  post?: SocialPost;
  activity?: UserActivity;
  goal?: UserGoal;
  achievement?: any;
  badge?: any;
  friendship?: Friendship;
  group?: FitnessGroup;
  challenge?: SocialChallenge;
  
  // Interaction data
  likes: number;
  comments: number;
  isLikedByCurrentUser: boolean;
  
  // Visibility
  visibility: PostVisibility;
}

/**
 * User Follow - following relationships (like social media following)
 */
export interface UserFollow {
  id: number;
  followerId: number;
  followingId: number;
  follower: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl'>;
  following: Pick<User, 'id' | 'firstName' | 'lastName' | 'avatarUrl' | 'level'>;
  createdAt: Date;
  
  // Follow settings
  isNotificationsEnabled: boolean;
  isMuted: boolean;
}

// Enums and Types

export enum PostType {
  Activity = 'Activity',
  Goal = 'Goal',
  Achievement = 'Achievement',
  Badge = 'Badge',
  Photo = 'Photo',
  Text = 'Text',
  Challenge = 'Challenge',
  Milestone = 'Milestone'
}

export enum PostVisibility {
  Public = 'Public',
  Friends = 'Friends',
  Group = 'Group',
  Private = 'Private'
}

export enum FriendshipStatus {
  Pending = 'Pending',
  Accepted = 'Accepted',
  Blocked = 'Blocked',
  Declined = 'Declined'
}

export enum GroupType {
  Open = 'Open',           // Anyone can join
  Closed = 'Closed',       // Request to join
  Secret = 'Secret'        // Invitation only
}

export enum GroupPrivacy {
  Public = 'Public',       // Visible to everyone
  Private = 'Private'      // Only visible to members
}

export enum GroupCategory {
  Running = 'Running',
  Cycling = 'Cycling',
  Weightlifting = 'Weightlifting',
  Yoga = 'Yoga',
  Swimming = 'Swimming',
  Dance = 'Dance',
  MartialArts = 'MartialArts',
  Team = 'Team',
  Outdoor = 'Outdoor',
  Nutrition = 'Nutrition',
  Motivation = 'Motivation',
  General = 'General'
}

export enum GroupRole {
  Member = 'Member',
  Moderator = 'Moderator',
  Admin = 'Admin',
  Owner = 'Owner'
}

export enum EventType {
  Workout = 'Workout',
  Challenge = 'Challenge',
  Meetup = 'Meetup',
  Competition = 'Competition',
  Workshop = 'Workshop',
  Social = 'Social'
}

export enum ChallengeType {
  Individual = 'Individual',  // Solo challenge with leaderboard
  Team = 'Team',             // Team vs team
  Group = 'Group',           // Group working together
  OneVsOne = 'OneVsOne'      // Direct competition
}

export enum ChallengeCategory {
  Steps = 'Steps',
  Distance = 'Distance',
  Duration = 'Duration',
  Calories = 'Calories',
  Activities = 'Activities',
  Consistency = 'Consistency',
  Strength = 'Strength',
  Custom = 'Custom'
}

export enum FeedItemType {
  ActivityCompleted = 'ActivityCompleted',
  GoalAchieved = 'GoalAchieved',
  BadgeEarned = 'BadgeEarned',
  AchievementUnlocked = 'AchievementUnlocked',
  FriendshipAccepted = 'FriendshipAccepted',
  GroupJoined = 'GroupJoined',
  ChallengeCompleted = 'ChallengeCompleted',
  PostShared = 'PostShared',
  LevelUp = 'LevelUp',
  StreakMilestone = 'StreakMilestone'
}

// Request/Response types for API

export interface CreatePostRequest {
  type: PostType;
  content: string;
  mediaUrls?: string[];
  activityId?: number;
  goalId?: number;
  achievementId?: number;
  badgeId?: number;
  visibility: PostVisibility;
  allowComments?: boolean;
  location?: {
    name: string;
    latitude?: number;
    longitude?: number;
  };
}

export interface UpdatePostRequest {
  content?: string;
  visibility?: PostVisibility;
  allowComments?: boolean;
}

export interface CreateCommentRequest {
  content: string;
  parentCommentId?: number;
}

export interface SendFriendRequestRequest {
  toUserId: number;
  message?: string;
}

export interface CreateGroupRequest {
  name: string;
  description: string;
  type: GroupType;
  privacy: GroupPrivacy;
  category: GroupCategory;
  location?: string;
  maxMembers?: number;
  rules?: string;
  tags: string[];
}

export interface UpdateGroupRequest {
  name?: string;
  description?: string;
  type?: GroupType;
  privacy?: GroupPrivacy;
  location?: string;
  maxMembers?: number;
  rules?: string;
  tags?: string[];
}

export interface CreateEventRequest {
  groupId: number;
  title: string;
  description: string;
  type: EventType;
  category: string;
  difficulty?: string;
  startTime: Date;
  endTime: Date;
  timeZone: string;
  location?: {
    type: 'physical' | 'virtual';
    name: string;
    address?: string;
    coordinates?: { latitude: number; longitude: number };
    virtualLink?: string;
  };
  maxParticipants?: number;
  requirements?: string[];
  whatToBring?: string[];
}

export interface CreateChallengeRequest {
  title: string;
  description: string;
  type: ChallengeType;
  category: ChallengeCategory;
  difficulty: string;
  metric: string;
  targetValue: number;
  unit: string;
  startDate: Date;
  endDate: Date;
  privacy: string;
  groupId?: number;
  inviteOnly?: boolean;
  maxParticipants?: number;
  rewards?: any;
}

// Response types

export interface SocialFeedResponse {
  items: FeedItem[];
  hasMore: boolean;
  nextCursor?: string;
}

export interface PostsResponse {
  posts: SocialPost[];
  hasMore: boolean;
  totalCount: number;
}

export interface FriendsResponse {
  friends: Friendship[];
  totalCount: number;
}

export interface GroupsResponse {
  groups: FitnessGroup[];
  hasMore: boolean;
  totalCount: number;
}

export interface ChallengesResponse {
  challenges: SocialChallenge[];
  hasMore: boolean;
  totalCount: number;
}

export interface SocialStatsResponse {
  friendsCount: number;
  followersCount: number;
  followingCount: number;
  postsCount: number;
  groupsCount: number;
  challengesCompleted: number;
  totalLikes: number;
  totalComments: number;
  socialScore: number; // Engagement score
  recentActivity: {
    postsThisWeek: number;
    likesThisWeek: number;
    commentsThisWeek: number;
    newFriendsThisWeek: number;
  };
}

// Utility classes

export class SocialUtils {
  static formatEngagementCount(count: number): string {
    if (count < 1000) return count.toString();
    if (count < 1000000) return `${(count / 1000).toFixed(1)}K`;
    return `${(count / 1000000).toFixed(1)}M`;
  }

  static getPostTypeIcon(type: PostType): string {
    const iconMap: { [key in PostType]: string } = {
      [PostType.Activity]: 'directions_run',
      [PostType.Goal]: 'flag',
      [PostType.Achievement]: 'emoji_events',
      [PostType.Badge]: 'military_tech',
      [PostType.Photo]: 'photo',
      [PostType.Text]: 'edit',
      [PostType.Challenge]: 'sports_kabaddi',
      [PostType.Milestone]: 'stars'
    };
    
    return iconMap[type] || 'chat';
  }

  static getFeedItemTypeMessage(type: FeedItemType, actorName: string): string {
    const messageMap: { [key in FeedItemType]: string } = {
      [FeedItemType.ActivityCompleted]: `${actorName} completed a workout`,
      [FeedItemType.GoalAchieved]: `${actorName} achieved a goal`,
      [FeedItemType.BadgeEarned]: `${actorName} earned a badge`,
      [FeedItemType.AchievementUnlocked]: `${actorName} unlocked an achievement`,
      [FeedItemType.FriendshipAccepted]: `${actorName} made a new friend`,
      [FeedItemType.GroupJoined]: `${actorName} joined a group`,
      [FeedItemType.ChallengeCompleted]: `${actorName} completed a challenge`,
      [FeedItemType.PostShared]: `${actorName} shared a post`,
      [FeedItemType.LevelUp]: `${actorName} leveled up`,
      [FeedItemType.StreakMilestone]: `${actorName} reached a streak milestone`
    };
    
    return messageMap[type] || `${actorName} did something awesome`;
  }

  static getGroupCategoryIcon(category: GroupCategory): string {
    const iconMap: { [key in GroupCategory]: string } = {
      [GroupCategory.Running]: 'directions_run',
      [GroupCategory.Cycling]: 'directions_bike',
      [GroupCategory.Weightlifting]: 'fitness_center',
      [GroupCategory.Yoga]: 'self_improvement',
      [GroupCategory.Swimming]: 'pool',
      [GroupCategory.Dance]: 'music_note',
      [GroupCategory.MartialArts]: 'sports_kabaddi',
      [GroupCategory.Team]: 'groups',
      [GroupCategory.Outdoor]: 'terrain',
      [GroupCategory.Nutrition]: 'restaurant',
      [GroupCategory.Motivation]: 'psychology',
      [GroupCategory.General]: 'fitness_center'
    };
    
    return iconMap[category] || 'group';
  }

  static getChallengeTypeDescription(type: ChallengeType): string {
    const descriptionMap: { [key in ChallengeType]: string } = {
      [ChallengeType.Individual]: 'Compete individually on the leaderboard',
      [ChallengeType.Team]: 'Join a team and compete against other teams',
      [ChallengeType.Group]: 'Work together as a group to reach the goal',
      [ChallengeType.OneVsOne]: 'Direct head-to-head competition'
    };
    
    return descriptionMap[type] || 'Challenge format';
  }

  static getTimeUntilEvent(eventTime: Date): string {
    const now = new Date();
    const diffMs = eventTime.getTime() - now.getTime();
    
    if (diffMs <= 0) return 'Event has started';
    
    const days = Math.floor(diffMs / (1000 * 60 * 60 * 24));
    const hours = Math.floor((diffMs % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    const minutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
    
    if (days > 0) return `${days}d ${hours}h`;
    if (hours > 0) return `${hours}h ${minutes}m`;
    return `${minutes}m`;
  }

  static isEventJoinable(event: GroupEvent): boolean {
    const now = new Date();
    const eventStart = new Date(event.startTime);
    const hasSpace = !event.maxParticipants || event.currentParticipants < event.maxParticipants;
    
    return eventStart > now && hasSpace;
  }

  static isChallengeActive(challenge: SocialChallenge): boolean {
    const now = new Date();
    const startDate = new Date(challenge.startDate);
    const endDate = new Date(challenge.endDate);
    
    return now >= startDate && now <= endDate && challenge.status === 'active';
  }
}
