import { createAction, props } from '@ngrx/store';
import { 
  Badge, 
  UserBadge, 
  BadgeCategory, 
  BadgeRarity 
} from '../../shared/models/user.model';

/**
 * Gamification Actions - the commands that trigger game-like fitness engagement.
 * Think of these as different ways to earn rewards, track achievements, and celebrate
 * progress in your fitness journey - like a video game for your health!
 */

// Experience Points and Leveling
export const earnExperiencePoints = createAction(
  '[Gamification] Earn Experience Points',
  props<{ 
    points: number; 
    source: 'activity_completion' | 'goal_achievement' | 'streak_bonus' | 'challenge_completion' | 'badge_earned' | 'social_interaction';
    sourceId?: number; // ID of the activity, goal, etc. that triggered the XP
    description?: string;
  }>()
);

export const earnExperiencePointsSuccess = createAction(
  '[Gamification] Earn Experience Points Success',
  props<{ 
    pointsEarned: number;
    totalPoints: number;
    newLevel?: number;
    leveledUp: boolean;
    levelUpRewards?: Array<{
      type: 'badge' | 'feature_unlock' | 'virtual_currency';
      value: any;
      description: string;
    }>;
  }>()
);

export const earnExperiencePointsFailure = createAction(
  '[Gamification] Earn Experience Points Failure',
  props<{ error: string }>()
);

export const checkLevelUp = createAction(
  '[Gamification] Check Level Up',
  props<{ currentXP: number }>()
);

export const levelUpSuccess = createAction(
  '[Gamification] Level Up Success',
  props<{ 
    newLevel: number;
    previousLevel: number;
    rewards: Array<{
      type: 'badge' | 'feature_unlock' | 'virtual_currency';
      value: any;
      description: string;
    }>;
  }>()
);

// Badge System
export const loadAvailableBadges = createAction('[Gamification] Load Available Badges');

export const loadAvailableBadgesSuccess = createAction(
  '[Gamification] Load Available Badges Success',
  props<{ badges: Badge[] }>()
);

export const loadAvailableBadgesFailure = createAction(
  '[Gamification] Load Available Badges Failure',
  props<{ error: string }>()
);

export const loadUserBadges = createAction('[Gamification] Load User Badges');

export const loadUserBadgesSuccess = createAction(
  '[Gamification] Load User Badges Success',
  props<{ userBadges: UserBadge[] }>()
);

export const loadUserBadgesFailure = createAction(
  '[Gamification] Load User Badges Failure',
  props<{ error: string }>()
);

export const earnBadge = createAction(
  '[Gamification] Earn Badge',
  props<{ 
    badgeId: number;
    context?: string; // JSON context about how the badge was earned
    activityId?: number;
    goalId?: number;
  }>()
);

export const earnBadgeSuccess = createAction(
  '[Gamification] Earn Badge Success',
  props<{ 
    userBadge: UserBadge;
    experiencePointsBonus: number;
  }>()
);

export const earnBadgeFailure = createAction(
  '[Gamification] Earn Badge Failure',
  props<{ error: string }>()
);

export const checkBadgeEligibility = createAction(
  '[Gamification] Check Badge Eligibility',
  props<{ 
    triggerType: 'activity_completed' | 'goal_achieved' | 'streak_reached' | 'milestone_achieved';
    triggerData: any;
  }>()
);

export const checkBadgeEligibilitySuccess = createAction(
  '[Gamification] Check Badge Eligibility Success',
  props<{ eligibleBadges: Array<{ badgeId: number; badge: Badge; criteria: any }> }>()
);

// Streak System
export const loadActiveStreaks = createAction('[Gamification] Load Active Streaks');

export const loadActiveStreaksSuccess = createAction(
  '[Gamification] Load Active Streaks Success',
  props<{ streaks: Array<{
    id: number;
    type: 'daily_activity' | 'weekly_goal' | 'workout_consistency' | 'step_target' | 'custom';
    name: string;
    description: string;
    currentCount: number;
    targetCount: number;
    lastActivityDate: Date;
    isActive: boolean;
    streakLevel: 'bronze' | 'silver' | 'gold' | 'platinum' | 'diamond';
    xpMultiplier: number;
    nextMilestone: number;
  }> }>()
);

export const loadActiveStreaksFailure = createAction(
  '[Gamification] Load Active Streaks Failure',
  props<{ error: string }>()
);

export const updateStreak = createAction(
  '[Gamification] Update Streak',
  props<{ 
    streakType: 'daily_activity' | 'weekly_goal' | 'workout_consistency' | 'step_target' | 'custom';
    increment: boolean; // true to increment, false might break streak
    activityDate?: Date;
  }>()
);

export const updateStreakSuccess = createAction(
  '[Gamification] Update Streak Success',
  props<{ 
    streak: {
      id: number;
      type: string;
      currentCount: number;
      wasExtended: boolean;
      milestoneReached?: {
        milestone: number;
        reward: {
          xp: number;
          badge?: Badge;
        };
      };
    };
  }>()
);

export const updateStreakFailure = createAction(
  '[Gamification] Update Streak Failure',
  props<{ error: string }>()
);

export const breakStreak = createAction(
  '[Gamification] Break Streak',
  props<{ 
    streakId: number;
    reason: 'missed_day' | 'user_reset' | 'goal_failed';
  }>()
);

export const breakStreakSuccess = createAction(
  '[Gamification] Break Streak Success',
  props<{ 
    streakId: number;
    finalCount: number;
    encouragementMessage: string;
  }>()
);

// Achievements and Milestones
export const loadAchievements = createAction('[Gamification] Load Achievements');

export const loadAchievementsSuccess = createAction(
  '[Gamification] Load Achievements Success',
  props<{ achievements: Array<{
    id: number;
    title: string;
    description: string;
    category: 'activity' | 'goal' | 'social' | 'streak' | 'special';
    icon: string;
    difficulty: 'easy' | 'medium' | 'hard' | 'legendary';
    criteria: any; // JSON criteria
    reward: {
      xp: number;
      badge?: Badge;
      virtualCurrency?: number;
    };
    progress: {
      current: number;
      target: number;
      percentage: number;
    };
    isCompleted: boolean;
    completedAt?: Date;
  }> }>()
);

export const loadAchievementsFailure = createAction(
  '[Gamification] Load Achievements Failure',
  props<{ error: string }>()
);

export const unlockAchievement = createAction(
  '[Gamification] Unlock Achievement',
  props<{ 
    achievementId: number;
    context?: any;
  }>()
);

export const unlockAchievementSuccess = createAction(
  '[Gamification] Unlock Achievement Success',
  props<{ 
    achievement: {
      id: number;
      title: string;
      description: string;
      unlockedAt: Date;
      reward: any;
    };
  }>()
);

export const unlockAchievementFailure = createAction(
  '[Gamification] Unlock Achievement Failure',
  props<{ error: string }>()
);

// Leaderboards and Competition
export const loadLeaderboards = createAction(
  '[Gamification] Load Leaderboards',
  props<{ 
    type: 'xp' | 'activities' | 'goals_completed' | 'streak_count' | 'badges_earned';
    scope: 'global' | 'friends' | 'local' | 'age_group';
    period: 'daily' | 'weekly' | 'monthly' | 'all_time';
  }>()
);

export const loadLeaderboardsSuccess = createAction(
  '[Gamification] Load Leaderboards Success',
  props<{ 
    leaderboardType: string;
    scope: string;
    period: string;
    entries: Array<{
      rank: number;
      userId: number;
      userName: string;
      userAvatar?: string;
      value: number;
      isCurrentUser: boolean;
      trendDirection: 'up' | 'down' | 'same';
      trendValue: number;
    }>;
    userEntry?: {
      rank: number;
      value: number;
      trendDirection: 'up' | 'down' | 'same';
      trendValue: number;
    };
  }>()
);

export const loadLeaderboardsFailure = createAction(
  '[Gamification] Load Leaderboards Failure',
  props<{ error: string }>()
);

// Virtual Currency and Rewards Store
export const loadVirtualCurrency = createAction('[Gamification] Load Virtual Currency');

export const loadVirtualCurrencySuccess = createAction(
  '[Gamification] Load Virtual Currency Success',
  props<{ 
    currency: {
      balance: number;
      totalEarned: number;
      totalSpent: number;
      recentTransactions: Array<{
        id: number;
        type: 'earned' | 'spent';
        amount: number;
        source: string;
        description: string;
        timestamp: Date;
      }>;
    };
  }>()
);

export const loadVirtualCurrencyFailure = createAction(
  '[Gamification] Load Virtual Currency Failure',
  props<{ error: string }>()
);

export const earnVirtualCurrency = createAction(
  '[Gamification] Earn Virtual Currency',
  props<{ 
    amount: number;
    source: 'level_up' | 'achievement' | 'daily_bonus' | 'streak_bonus' | 'challenge_completion';
    description: string;
  }>()
);

export const earnVirtualCurrencySuccess = createAction(
  '[Gamification] Earn Virtual Currency Success',
  props<{ 
    amountEarned: number;
    newBalance: number;
    transactionId: number;
  }>()
);

export const spendVirtualCurrency = createAction(
  '[Gamification] Spend Virtual Currency',
  props<{ 
    amount: number;
    itemId: number;
    itemType: 'avatar_item' | 'theme' | 'power_up' | 'streak_freeze' | 'goal_boost';
    itemName: string;
  }>()
);

export const spendVirtualCurrencySuccess = createAction(
  '[Gamification] Spend Virtual Currency Success',
  props<{ 
    amountSpent: number;
    newBalance: number;
    purchasedItem: any;
    transactionId: number;
  }>()
);

export const spendVirtualCurrencyFailure = createAction(
  '[Gamification] Spend Virtual Currency Failure',
  props<{ error: string }>()
);

// Rewards Store
export const loadRewardsStore = createAction('[Gamification] Load Rewards Store');

export const loadRewardsStoreSuccess = createAction(
  '[Gamification] Load Rewards Store Success',
  props<{ 
    storeItems: Array<{
      id: number;
      name: string;
      description: string;
      category: 'avatar_items' | 'themes' | 'power_ups' | 'utilities';
      price: number;
      currency: 'virtual_coins' | 'real_money';
      imageUrl: string;
      isOwned: boolean;
      isLimitedTime?: boolean;
      expiresAt?: Date;
      popularity: number;
    }>;
    featuredItems: Array<number>; // item IDs
    userOwnedItems: Array<number>; // item IDs
  }>()
);

export const loadRewardsStoreFailure = createAction(
  '[Gamification] Load Rewards Store Failure',
  props<{ error: string }>()
);

export const purchaseStoreItem = createAction(
  '[Gamification] Purchase Store Item',
  props<{ 
    itemId: number;
    paymentMethod: 'virtual_currency' | 'real_money';
  }>()
);

export const purchaseStoreItemSuccess = createAction(
  '[Gamification] Purchase Store Item Success',
  props<{ 
    item: any;
    transactionId: number;
    newCurrencyBalance?: number;
  }>()
);

export const purchaseStoreItemFailure = createAction(
  '[Gamification] Purchase Store Item Failure',
  props<{ error: string }>()
);

// Daily/Weekly Challenges and Bonuses
export const loadDailyChallenges = createAction('[Gamification] Load Daily Challenges');

export const loadDailyChallengesSuccess = createAction(
  '[Gamification] Load Daily Challenges Success',
  props<{ 
    dailyChallenges: Array<{
      id: number;
      title: string;
      description: string;
      difficulty: 'easy' | 'medium' | 'hard';
      type: 'activity' | 'step_count' | 'duration' | 'social' | 'streak';
      target: number;
      progress: number;
      reward: {
        xp: number;
        virtualCurrency: number;
        badge?: Badge;
      };
      isCompleted: boolean;
      expiresAt: Date;
    }>;
    weeklyChallenge?: {
      id: number;
      title: string;
      description: string;
      type: 'multi_activity' | 'consistency' | 'goal_focused';
      targets: Array<{ type: string; value: number; progress: number }>;
      reward: {
        xp: number;
        virtualCurrency: number;
        badge?: Badge;
        specialReward?: any;
      };
      isCompleted: boolean;
      progress: number; // overall percentage
      expiresAt: Date;
    };
  }>()
);

export const loadDailyChallengesFailure = createAction(
  '[Gamification] Load Daily Challenges Failure',
  props<{ error: string }>()
);

export const completeDailyChallenge = createAction(
  '[Gamification] Complete Daily Challenge',
  props<{ challengeId: number }>()
);

export const completeDailyChallengeSuccess = createAction(
  '[Gamification] Complete Daily Challenge Success',
  props<{ 
    challengeId: number;
    reward: any;
    newChallenges?: any[]; // New challenges that might be unlocked
  }>()
);

export const completeDailyChallengeFailure = createAction(
  '[Gamification] Complete Daily Challenge Failure',
  props<{ error: string }>()
);

// Seasonal Events and Special Rewards
export const loadSeasonalEvents = createAction('[Gamification] Load Seasonal Events');

export const loadSeasonalEventsSuccess = createAction(
  '[Gamification] Load Seasonal Events Success',
  props<{ 
    activeEvents: Array<{
      id: number;
      name: string;
      description: string;
      theme: string;
      startDate: Date;
      endDate: Date;
      challenges: Array<{
        id: number;
        title: string;
        description: string;
        progress: number;
        target: number;
        reward: any;
        isCompleted: boolean;
      }>;
      leaderboard?: {
        userRank: number;
        totalParticipants: number;
        topEntries: Array<{
          rank: number;
          userName: string;
          score: number;
        }>;
      };
      exclusiveRewards: Array<{
        id: number;
        name: string;
        description: string;
        requiredPoints: number;
        userProgress: number;
        isUnlocked: boolean;
      }>;
    }>;
  }>()
);

export const loadSeasonalEventsFailure = createAction(
  '[Gamification] Load Seasonal Events Failure',
  props<{ error: string }>()
);

export const participateInSeasonalEvent = createAction(
  '[Gamification] Participate In Seasonal Event',
  props<{ eventId: number }>()
);

export const participateInSeasonalEventSuccess = createAction(
  '[Gamification] Participate In Seasonal Event Success',
  props<{ eventId: number; participationReward?: any }>()
);

// UI State and Notifications
export const setGamificationView = createAction(
  '[Gamification] Set Gamification View',
  props<{ view: 'overview' | 'badges' | 'achievements' | 'leaderboards' | 'store' | 'challenges' }>()
);

export const showRewardNotification = createAction(
  '[Gamification] Show Reward Notification',
  props<{ 
    type: 'xp_earned' | 'level_up' | 'badge_earned' | 'achievement_unlocked' | 'streak_milestone';
    title: string;
    message: string;
    reward?: any;
    autoHide?: boolean;
    duration?: number;
  }>()
);

export const hideRewardNotification = createAction('[Gamification] Hide Reward Notification');

export const clearGamificationError = createAction('[Gamification] Clear Error');

export const refreshGamificationData = createAction('[Gamification] Refresh Gamification Data');

// Analytics and Insights
export const loadGamificationAnalytics = createAction('[Gamification] Load Gamification Analytics');

export const loadGamificationAnalyticsSuccess = createAction(
  '[Gamification] Load Gamification Analytics Success',
  props<{ 
    analytics: {
      totalXPEarned: number;
      averageXPPerDay: number;
      badgesEarned: number;
      achievementsUnlocked: number;
      longestStreak: number;
      currentLevel: number;
      xpToNextLevel: number;
      leaderboardRanks: { [key: string]: number };
      engagementTrends: Array<{
        date: Date;
        xpEarned: number;
        activitiesCompleted: number;
        badgesEarned: number;
      }>;
      motivationalInsights: string[];
    };
  }>()
);

export const loadGamificationAnalyticsFailure = createAction(
  '[Gamification] Load Gamification Analytics Failure',
  props<{ error: string }>()
);
