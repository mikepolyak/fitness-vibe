import { createReducer, on } from '@ngrx/store';
import { 
  Badge, 
  UserBadge, 
  BadgeCategory, 
  BadgeRarity 
} from '../../shared/models/user.model';
import * as GamificationActions from './gamification.actions';

/**
 * Gamification State Interface - the comprehensive reward and motivation system.
 * Think of this as a digital arcade that tracks achievements, rewards, and progress
 * in your fitness journey - making healthy habits feel like playing a rewarding game!
 */
export interface GamificationState {
  // Experience Points and Leveling
  currentXP: number;
  currentLevel: number;
  xpToNextLevel: number;
  totalXPEarned: number;
  recentXPTransactions: Array<{
    id: number;
    points: number;
    source: string;
    description: string;
    timestamp: Date;
  }>;
  
  // Badge System
  availableBadges: Badge[];
  availableBadgesLoaded: boolean;
  userBadges: UserBadge[];
  userBadgesLoaded: boolean;
  recentlyEarnedBadges: UserBadge[];
  
  // Streaks System
  activeStreaks: Array<{
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
  }>;
  streaksLoaded: boolean;
  
  // Achievements System
  achievements: Array<{
    id: number;
    title: string;
    description: string;
    category: 'activity' | 'goal' | 'social' | 'streak' | 'special';
    icon: string;
    difficulty: 'easy' | 'medium' | 'hard' | 'legendary';
    criteria: any;
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
  }>;
  achievementsLoaded: boolean;
  recentlyUnlockedAchievements: any[];
  
  // Leaderboards
  leaderboards: {
    [key: string]: {
      type: string;
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
      lastUpdated: Date;
    };
  };
  leaderboardsLoaded: boolean;
  
  // Virtual Currency and Store
  virtualCurrency: {
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
  } | null;
  
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
  featuredStoreItems: number[];
  userOwnedItems: number[];
  storeLoaded: boolean;
  
  // Daily and Weekly Challenges
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
  
  weeklyChallenge: {
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
    progress: number;
    expiresAt: Date;
  } | null;
  
  challengesLoaded: boolean;
  
  // Seasonal Events
  seasonalEvents: Array<{
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
  seasonalEventsLoaded: boolean;
  
  // Analytics and Insights
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
  } | null;
  analyticsLoaded: boolean;
  
  // UI State
  currentView: 'overview' | 'badges' | 'achievements' | 'leaderboards' | 'store' | 'challenges';
  rewardNotification: {
    isVisible: boolean;
    type: 'xp_earned' | 'level_up' | 'badge_earned' | 'achievement_unlocked' | 'streak_milestone';
    title: string;
    message: string;
    reward?: any;
    autoHide: boolean;
    duration: number;
  } | null;
  
  // Loading states
  isLoading: boolean;
  isEarningXP: boolean;
  isEarningBadge: boolean;
  isUpdatingStreak: boolean;
  isPurchasing: boolean;
  isLoadingAnalytics: boolean;
  
  // Error handling
  error: string | null;
}

/**
 * Initial state - a fresh gamification system ready to make fitness fun and rewarding.
 */
export const initialState: GamificationState = {
  currentXP: 0,
  currentLevel: 1,
  xpToNextLevel: 100,
  totalXPEarned: 0,
  recentXPTransactions: [],
  
  availableBadges: [],
  availableBadgesLoaded: false,
  userBadges: [],
  userBadgesLoaded: false,
  recentlyEarnedBadges: [],
  
  activeStreaks: [],
  streaksLoaded: false,
  
  achievements: [],
  achievementsLoaded: false,
  recentlyUnlockedAchievements: [],
  
  leaderboards: {},
  leaderboardsLoaded: false,
  
  virtualCurrency: null,
  storeItems: [],
  featuredStoreItems: [],
  userOwnedItems: [],
  storeLoaded: false,
  
  dailyChallenges: [],
  weeklyChallenge: null,
  challengesLoaded: false,
  
  seasonalEvents: [],
  seasonalEventsLoaded: false,
  
  analytics: null,
  analyticsLoaded: false,
  
  currentView: 'overview',
  rewardNotification: null,
  
  isLoading: false,
  isEarningXP: false,
  isEarningBadge: false,
  isUpdatingStreak: false,
  isPurchasing: false,
  isLoadingAnalytics: false,
  
  error: null
};

/**
 * Gamification Reducer - the game master who manages all rewards, achievements, and motivational systems.
 * Each action is like a different game event: "player earned points", "unlocked achievement", 
 * "reached new level", "completed challenge", etc.
 */
export const gamificationReducer = createReducer(
  initialState,

  // Experience Points and Leveling
  on(GamificationActions.earnExperiencePoints, (state) => ({
    ...state,
    isEarningXP: true,
    error: null
  })),

  on(GamificationActions.earnExperiencePointsSuccess, (state, { 
    pointsEarned, 
    totalPoints, 
    newLevel, 
    leveledUp, 
    levelUpRewards 
  }) => ({
    ...state,
    currentXP: totalPoints,
    totalXPEarned: state.totalXPEarned + pointsEarned,
    currentLevel: newLevel || state.currentLevel,
    recentXPTransactions: [
      {
        id: Date.now(),
        points: pointsEarned,
        source: 'Activity',
        description: `Earned ${pointsEarned} XP`,
        timestamp: new Date()
      },
      ...state.recentXPTransactions.slice(0, 9) // Keep last 10 transactions
    ],
    isEarningXP: false,
    error: null,
    // Show level up notification if leveled up
    rewardNotification: leveledUp ? {
      isVisible: true,
      type: 'level_up' as const,
      title: `Level Up!`,
      message: `You reached level ${newLevel}!`,
      reward: levelUpRewards,
      autoHide: true,
      duration: 5000
    } : state.rewardNotification
  })),

  on(GamificationActions.earnExperiencePointsFailure, (state, { error }) => ({
    ...state,
    isEarningXP: false,
    error
  })),

  on(GamificationActions.levelUpSuccess, (state, { newLevel, previousLevel, rewards }) => ({
    ...state,
    currentLevel: newLevel,
    rewardNotification: {
      isVisible: true,
      type: 'level_up',
      title: `🎉 Level ${newLevel} Achieved!`,
      message: `Congratulations! You've advanced from level ${previousLevel} to ${newLevel}!`,
      reward: rewards,
      autoHide: true,
      duration: 8000
    }
  })),

  // Badge System
  on(GamificationActions.loadAvailableBadges, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.loadAvailableBadgesSuccess, (state, { badges }) => ({
    ...state,
    availableBadges: badges,
    availableBadgesLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GamificationActions.loadAvailableBadgesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GamificationActions.loadUserBadges, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.loadUserBadgesSuccess, (state, { userBadges }) => ({
    ...state,
    userBadges,
    userBadgesLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GamificationActions.loadUserBadgesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GamificationActions.earnBadge, (state) => ({
    ...state,
    isEarningBadge: true,
    error: null
  })),

  on(GamificationActions.earnBadgeSuccess, (state, { userBadge, experiencePointsBonus }) => ({
    ...state,
    userBadges: [...state.userBadges, userBadge],
    recentlyEarnedBadges: [userBadge, ...state.recentlyEarnedBadges.slice(0, 4)], // Keep last 5
    totalXPEarned: state.totalXPEarned + experiencePointsBonus,
    currentXP: state.currentXP + experiencePointsBonus,
    isEarningBadge: false,
    rewardNotification: {
      isVisible: true,
      type: 'badge_earned',
      title: '🏆 Badge Earned!',
      message: `You've earned the "${userBadge.badge.name}" badge!`,
      reward: { badge: userBadge.badge, xp: experiencePointsBonus },
      autoHide: true,
      duration: 6000
    },
    error: null
  })),

  on(GamificationActions.earnBadgeFailure, (state, { error }) => ({
    ...state,
    isEarningBadge: false,
    error
  })),

  on(GamificationActions.checkBadgeEligibilitySuccess, (state, { eligibleBadges }) => ({
    ...state,
    // Could store eligible badges for UI display if needed
    error: null
  })),

  // Streaks System
  on(GamificationActions.loadActiveStreaks, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.loadActiveStreaksSuccess, (state, { streaks }) => ({
    ...state,
    activeStreaks: streaks,
    streaksLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GamificationActions.loadActiveStreaksFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GamificationActions.updateStreak, (state) => ({
    ...state,
    isUpdatingStreak: true,
    error: null
  })),

  on(GamificationActions.updateStreakSuccess, (state, { streak }) => ({
    ...state,
    activeStreaks: state.activeStreaks.map(s => 
      s.id === streak.id ? { ...s, currentCount: streak.currentCount } : s
    ),
    isUpdatingStreak: false,
    rewardNotification: streak.milestoneReached ? {
      isVisible: true,
      type: 'streak_milestone',
      title: '🔥 Streak Milestone!',
      message: `${streak.currentCount} day streak! Amazing consistency!`,
      reward: streak.milestoneReached.reward,
      autoHide: true,
      duration: 5000
    } : state.rewardNotification,
    error: null
  })),

  on(GamificationActions.updateStreakFailure, (state, { error }) => ({
    ...state,
    isUpdatingStreak: false,
    error
  })),

  on(GamificationActions.breakStreakSuccess, (state, { streakId, finalCount, encouragementMessage }) => ({
    ...state,
    activeStreaks: state.activeStreaks.map(s => 
      s.id === streakId ? { ...s, currentCount: 0, isActive: false } : s
    ),
    rewardNotification: {
      isVisible: true,
      type: 'streak_milestone',
      title: 'Streak Reset',
      message: encouragementMessage,
      autoHide: true,
      duration: 4000
    },
    error: null
  })),

  // Achievements System
  on(GamificationActions.loadAchievements, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.loadAchievementsSuccess, (state, { achievements }) => ({
    ...state,
    achievements,
    achievementsLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GamificationActions.loadAchievementsFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GamificationActions.unlockAchievement, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.unlockAchievementSuccess, (state, { achievement }) => ({
    ...state,
    achievements: state.achievements.map(a => 
      a.id === achievement.id 
        ? { ...a, isCompleted: true, completedAt: achievement.unlockedAt }
        : a
    ),
    recentlyUnlockedAchievements: [achievement, ...state.recentlyUnlockedAchievements.slice(0, 4)],
    isLoading: false,
    rewardNotification: {
      isVisible: true,
      type: 'achievement_unlocked',
      title: '🎯 Achievement Unlocked!',
      message: `"${achievement.title}" - ${achievement.description}`,
      reward: achievement.reward,
      autoHide: true,
      duration: 7000
    },
    error: null
  })),

  on(GamificationActions.unlockAchievementFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // Leaderboards
  on(GamificationActions.loadLeaderboards, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.loadLeaderboardsSuccess, (state, { 
    leaderboardType, 
    scope, 
    period, 
    entries, 
    userEntry 
  }) => {
    const leaderboardKey = `${leaderboardType}_${scope}_${period}`;
    
    return {
      ...state,
      leaderboards: {
        ...state.leaderboards,
        [leaderboardKey]: {
          type: leaderboardType,
          scope,
          period,
          entries,
          userEntry,
          lastUpdated: new Date()
        }
      },
      leaderboardsLoaded: true,
      isLoading: false,
      error: null
    };
  }),

  on(GamificationActions.loadLeaderboardsFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // Virtual Currency
  on(GamificationActions.loadVirtualCurrency, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.loadVirtualCurrencySuccess, (state, { currency }) => ({
    ...state,
    virtualCurrency: currency,
    isLoading: false,
    error: null
  })),

  on(GamificationActions.loadVirtualCurrencyFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GamificationActions.earnVirtualCurrency, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.earnVirtualCurrencySuccess, (state, { amountEarned, newBalance, transactionId }) => ({
    ...state,
    virtualCurrency: state.virtualCurrency ? {
      ...state.virtualCurrency,
      balance: newBalance,
      totalEarned: state.virtualCurrency.totalEarned + amountEarned,
      recentTransactions: [
        {
          id: transactionId,
          type: 'earned',
          amount: amountEarned,
          source: 'Activity',
          description: `Earned ${amountEarned} coins`,
          timestamp: new Date()
        },
        ...state.virtualCurrency.recentTransactions.slice(0, 9)
      ]
    } : null,
    isLoading: false,
    error: null
  })),

  on(GamificationActions.spendVirtualCurrency, (state) => ({
    ...state,
    isPurchasing: true,
    error: null
  })),

  on(GamificationActions.spendVirtualCurrencySuccess, (state, { 
    amountSpent, 
    newBalance, 
    purchasedItem, 
    transactionId 
  }) => ({
    ...state,
    virtualCurrency: state.virtualCurrency ? {
      ...state.virtualCurrency,
      balance: newBalance,
      totalSpent: state.virtualCurrency.totalSpent + amountSpent,
      recentTransactions: [
        {
          id: transactionId,
          type: 'spent',
          amount: amountSpent,
          source: 'Store Purchase',
          description: `Bought ${purchasedItem.name}`,
          timestamp: new Date()
        },
        ...state.virtualCurrency.recentTransactions.slice(0, 9)
      ]
    } : null,
    userOwnedItems: [...state.userOwnedItems, purchasedItem.id],
    storeItems: state.storeItems.map(item => 
      item.id === purchasedItem.id ? { ...item, isOwned: true } : item
    ),
    isPurchasing: false,
    error: null
  })),

  on(GamificationActions.spendVirtualCurrencyFailure, (state, { error }) => ({
    ...state,
    isPurchasing: false,
    error
  })),

  // Rewards Store
  on(GamificationActions.loadRewardsStore, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.loadRewardsStoreSuccess, (state, { storeItems, featuredItems, userOwnedItems }) => ({
    ...state,
    storeItems,
    featuredStoreItems: featuredItems,
    userOwnedItems,
    storeLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GamificationActions.loadRewardsStoreFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GamificationActions.purchaseStoreItem, (state) => ({
    ...state,
    isPurchasing: true,
    error: null
  })),

  on(GamificationActions.purchaseStoreItemSuccess, (state, { item, transactionId, newCurrencyBalance }) => ({
    ...state,
    userOwnedItems: [...state.userOwnedItems, item.id],
    storeItems: state.storeItems.map(storeItem => 
      storeItem.id === item.id ? { ...storeItem, isOwned: true } : storeItem
    ),
    virtualCurrency: newCurrencyBalance !== undefined && state.virtualCurrency ? {
      ...state.virtualCurrency,
      balance: newCurrencyBalance
    } : state.virtualCurrency,
    isPurchasing: false,
    error: null
  })),

  on(GamificationActions.purchaseStoreItemFailure, (state, { error }) => ({
    ...state,
    isPurchasing: false,
    error
  })),

  // Daily and Weekly Challenges
  on(GamificationActions.loadDailyChallenges, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.loadDailyChallengesSuccess, (state, { dailyChallenges, weeklyChallenge }) => ({
    ...state,
    dailyChallenges,
    weeklyChallenge: weeklyChallenge || null,
    challengesLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GamificationActions.loadDailyChallengesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GamificationActions.completeDailyChallenge, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.completeDailyChallengeSuccess, (state, { challengeId, reward, newChallenges }) => ({
    ...state,
    dailyChallenges: state.dailyChallenges.map(challenge => 
      challenge.id === challengeId 
        ? { ...challenge, isCompleted: true, progress: challenge.target }
        : challenge
    ),
    // Add new challenges if any were unlocked
    ...(newChallenges && newChallenges.length > 0 ? {
      dailyChallenges: [...state.dailyChallenges.filter(c => c.id !== challengeId), ...newChallenges]
    } : {}),
    isLoading: false,
    rewardNotification: {
      isVisible: true,
      type: 'xp_earned',
      title: '💫 Challenge Complete!',
      message: `Daily challenge completed! Earned ${reward.xp} XP`,
      reward,
      autoHide: true,
      duration: 4000
    },
    error: null
  })),

  on(GamificationActions.completeDailyChallengeFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // Seasonal Events
  on(GamificationActions.loadSeasonalEvents, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.loadSeasonalEventsSuccess, (state, { activeEvents }) => ({
    ...state,
    seasonalEvents: activeEvents,
    seasonalEventsLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GamificationActions.loadSeasonalEventsFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GamificationActions.participateInSeasonalEvent, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GamificationActions.participateInSeasonalEventSuccess, (state, { eventId, participationReward }) => ({
    ...state,
    seasonalEvents: state.seasonalEvents.map(event => 
      event.id === eventId 
        ? { ...event, /* mark as participated */ }
        : event
    ),
    isLoading: false,
    error: null
  })),

  // Analytics
  on(GamificationActions.loadGamificationAnalytics, (state) => ({
    ...state,
    isLoadingAnalytics: true,
    error: null
  })),

  on(GamificationActions.loadGamificationAnalyticsSuccess, (state, { analytics }) => ({
    ...state,
    analytics,
    analyticsLoaded: true,
    isLoadingAnalytics: false,
    error: null
  })),

  on(GamificationActions.loadGamificationAnalyticsFailure, (state, { error }) => ({
    ...state,
    isLoadingAnalytics: false,
    error
  })),

  // UI State Management
  on(GamificationActions.setGamificationView, (state, { view }) => ({
    ...state,
    currentView: view
  })),

  on(GamificationActions.showRewardNotification, (state, { type, title, message, reward, autoHide = true, duration = 5000 }) => ({
    ...state,
    rewardNotification: {
      isVisible: true,
      type,
      title,
      message,
      reward,
      autoHide,
      duration
    }
  })),

  on(GamificationActions.hideRewardNotification, (state) => ({
    ...state,
    rewardNotification: null
  })),

  on(GamificationActions.clearGamificationError, (state) => ({
    ...state,
    error: null
  })),

  // Refresh all gamification data
  on(GamificationActions.refreshGamificationData, (state) => ({
    ...state,
    availableBadgesLoaded: false,
    userBadgesLoaded: false,
    streaksLoaded: false,
    achievementsLoaded: false,
    leaderboardsLoaded: false,
    storeLoaded: false,
    challengesLoaded: false,
    seasonalEventsLoaded: false,
    analyticsLoaded: false,
    isLoading: true
  }))
);
