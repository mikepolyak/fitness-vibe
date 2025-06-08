import { createFeatureSelector, createSelector } from '@ngrx/store';
import { GamificationState } from './gamification.reducer';
import { BadgeCategory, BadgeRarity } from '../../shared/models/user.model';

/**
 * Gamification Selectors - the specialized game masters who can quickly analyze 
 * progress, achievements, and rewards. Think of them as expert arcade attendants
 * who know exactly how well you're performing in the fitness game!
 */

// Feature selector - gets the entire gamification section of the store
export const selectGamificationState = createFeatureSelector<GamificationState>('gamification');

// Experience Points and Leveling selectors
export const selectCurrentXP = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.currentXP
);

export const selectCurrentLevel = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.currentLevel
);

export const selectXPToNextLevel = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.xpToNextLevel
);

export const selectTotalXPEarned = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.totalXPEarned
);

export const selectRecentXPTransactions = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.recentXPTransactions
);

export const selectLevelProgress = createSelector(
  selectCurrentXP,
  selectCurrentLevel,
  (currentXP, currentLevel) => {
    // Calculate XP needed for current level and next level
    // Each level requires: level * 100 XP (100, 200, 300, etc.)
    const currentLevelRequiredXP = currentLevel === 1 ? 0 : 
      Array.from({length: currentLevel - 1}, (_, i) => (i + 1) * 100)
        .reduce((sum, xp) => sum + xp, 0);
    
    const nextLevelRequiredXP = currentLevelRequiredXP + (currentLevel * 100);
    const progressInCurrentLevel = currentXP - currentLevelRequiredXP;
    const progressNeeded = currentLevel * 100;
    const progressPercentage = (progressInCurrentLevel / progressNeeded) * 100;
    
    return {
      currentXP: progressInCurrentLevel,
      nextLevelXP: progressNeeded,
      progress: Math.min(100, Math.max(0, progressPercentage)),
      totalXP: currentXP,
      level: currentLevel,
      xpToNextLevel: Math.max(0, nextLevelRequiredXP - currentXP)
    };
  }
);

// Badge System selectors
export const selectAvailableBadges = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.availableBadges
);

export const selectAvailableBadgesLoaded = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.availableBadgesLoaded
);

export const selectUserBadges = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.userBadges
);

export const selectUserBadgesLoaded = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.userBadgesLoaded
);

export const selectRecentlyEarnedBadges = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.recentlyEarnedBadges
);

export const selectBadgesByCategory = createSelector(
  selectUserBadges,
  (userBadges) => {
    const badgesByCategory: { [key in BadgeCategory]?: typeof userBadges } = {};
    
    userBadges.forEach(userBadge => {
      const category = userBadge.badge.category;
      if (!badgesByCategory[category]) {
        badgesByCategory[category] = [];
      }
      badgesByCategory[category]!.push(userBadge);
    });
    
    return badgesByCategory;
  }
);

export const selectBadgesByRarity = createSelector(
  selectUserBadges,
  (userBadges) => {
    const badgesByRarity: { [key in BadgeRarity]?: typeof userBadges } = {};
    
    userBadges.forEach(userBadge => {
      const rarity = userBadge.badge.rarity;
      if (!badgesByRarity[rarity]) {
        badgesByRarity[rarity] = [];
      }
      badgesByRarity[rarity]!.push(userBadge);
    });
    
    return badgesByRarity;
  }
);

export const selectBadgeCompletionStats = createSelector(
  selectAvailableBadges,
  selectUserBadges,
  (availableBadges, userBadges) => {
    if (availableBadges.length === 0) return { earned: 0, total: 0, percentage: 0 };
    
    const userBadgeIds = new Set(userBadges.map(ub => ub.badgeId));
    const earnedCount = availableBadges.filter(badge => userBadgeIds.has(badge.id)).length;
    
    return {
      earned: earnedCount,
      total: availableBadges.length,
      percentage: (earnedCount / availableBadges.length) * 100
    };
  }
);

export const selectRareBadges = createSelector(
  selectUserBadges,
  (userBadges) => userBadges.filter(ub => 
    ub.badge.rarity === BadgeRarity.Rare || 
    ub.badge.rarity === BadgeRarity.Epic || 
    ub.badge.rarity === BadgeRarity.Legendary
  )
);

// Streaks selectors
export const selectActiveStreaks = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.activeStreaks
);

export const selectStreaksLoaded = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.streaksLoaded
);

export const selectLongestActiveStreak = createSelector(
  selectActiveStreaks,
  (streaks) => {
    if (streaks.length === 0) return null;
    
    return streaks.reduce((longest, current) => 
      current.currentCount > longest.currentCount ? current : longest
    );
  }
);

export const selectStreaksByLevel = createSelector(
  selectActiveStreaks,
  (streaks) => {
    const streaksByLevel = {
      bronze: streaks.filter(s => s.streakLevel === 'bronze'),
      silver: streaks.filter(s => s.streakLevel === 'silver'),
      gold: streaks.filter(s => s.streakLevel === 'gold'),
      platinum: streaks.filter(s => s.streakLevel === 'platinum'),
      diamond: streaks.filter(s => s.streakLevel === 'diamond')
    };
    
    return streaksByLevel;
  }
);

export const selectActiveStreakCount = createSelector(
  selectActiveStreaks,
  (streaks) => streaks.filter(s => s.isActive).length
);

export const selectTotalStreakDays = createSelector(
  selectActiveStreaks,
  (streaks) => streaks.reduce((total, streak) => total + streak.currentCount, 0)
);

// Achievements selectors
export const selectAchievements = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.achievements
);

export const selectAchievementsLoaded = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.achievementsLoaded
);

export const selectRecentlyUnlockedAchievements = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.recentlyUnlockedAchievements
);

export const selectCompletedAchievements = createSelector(
  selectAchievements,
  (achievements) => achievements.filter(a => a.isCompleted)
);

export const selectInProgressAchievements = createSelector(
  selectAchievements,
  (achievements) => achievements.filter(a => !a.isCompleted && a.progress.current > 0)
);

export const selectUpcomingAchievements = createSelector(
  selectAchievements,
  (achievements) => achievements.filter(a => !a.isCompleted && a.progress.current === 0)
);

export const selectAchievementsByCategory = createSelector(
  selectAchievements,
  (achievements) => {
    const achievementsByCategory: { [key: string]: typeof achievements } = {};
    
    achievements.forEach(achievement => {
      const category = achievement.category;
      if (!achievementsByCategory[category]) {
        achievementsByCategory[category] = [];
      }
      achievementsByCategory[category].push(achievement);
    });
    
    return achievementsByCategory;
  }
);

export const selectAchievementsByDifficulty = createSelector(
  selectAchievements,
  (achievements) => {
    const achievementsByDifficulty = {
      easy: achievements.filter(a => a.difficulty === 'easy'),
      medium: achievements.filter(a => a.difficulty === 'medium'),
      hard: achievements.filter(a => a.difficulty === 'hard'),
      legendary: achievements.filter(a => a.difficulty === 'legendary')
    };
    
    return achievementsByDifficulty;
  }
);

export const selectAchievementCompletionStats = createSelector(
  selectAchievements,
  (achievements) => {
    if (achievements.length === 0) return { completed: 0, total: 0, percentage: 0 };
    
    const completedCount = achievements.filter(a => a.isCompleted).length;
    
    return {
      completed: completedCount,
      total: achievements.length,
      percentage: (completedCount / achievements.length) * 100
    };
  }
);

export const selectNearCompletionAchievements = createSelector(
  selectInProgressAchievements,
  (inProgress) => inProgress.filter(a => a.progress.percentage >= 75)
);

// Leaderboards selectors
export const selectLeaderboards = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.leaderboards
);

export const selectLeaderboardsLoaded = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.leaderboardsLoaded
);

export const selectLeaderboardByType = (type: string, scope: string, period: string) => createSelector(
  selectLeaderboards,
  (leaderboards) => {
    const key = `${type}_${scope}_${period}`;
    return leaderboards[key] || null;
  }
);

export const selectUserLeaderboardRanks = createSelector(
  selectLeaderboards,
  (leaderboards) => {
    const ranks: { [key: string]: number } = {};
    
    Object.entries(leaderboards).forEach(([key, leaderboard]) => {
      if (leaderboard.userEntry) {
        ranks[key] = leaderboard.userEntry.rank;
      }
    });
    
    return ranks;
  }
);

export const selectBestLeaderboardRank = createSelector(
  selectUserLeaderboardRanks,
  (ranks) => {
    const rankValues = Object.values(ranks);
    return rankValues.length > 0 ? Math.min(...rankValues) : null;
  }
);

// Virtual Currency selectors
export const selectVirtualCurrency = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.virtualCurrency
);

export const selectVirtualCurrencyBalance = createSelector(
  selectVirtualCurrency,
  (currency) => currency?.balance || 0
);

export const selectVirtualCurrencyTransactions = createSelector(
  selectVirtualCurrency,
  (currency) => currency?.recentTransactions || []
);

// Store selectors
export const selectStoreItems = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.storeItems
);

export const selectFeaturedStoreItems = createSelector(
  selectStoreItems,
  selectGamificationState,
  (storeItems, state) => {
    const featuredIds = new Set(state.featuredStoreItems);
    return storeItems.filter(item => featuredIds.has(item.id));
  }
);

export const selectUserOwnedItems = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.userOwnedItems
);

export const selectStoreLoaded = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.storeLoaded
);

export const selectStoreItemsByCategory = createSelector(
  selectStoreItems,
  (storeItems) => {
    const itemsByCategory: { [key: string]: typeof storeItems } = {};
    
    storeItems.forEach(item => {
      const category = item.category;
      if (!itemsByCategory[category]) {
        itemsByCategory[category] = [];
      }
      itemsByCategory[category].push(item);
    });
    
    return itemsByCategory;
  }
);

export const selectAffordableStoreItems = createSelector(
  selectStoreItems,
  selectVirtualCurrencyBalance,
  (storeItems, balance) => storeItems.filter(item => 
    item.currency === 'virtual_coins' && item.price <= balance && !item.isOwned
  )
);

// Challenges selectors
export const selectDailyChallenges = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.dailyChallenges
);

export const selectWeeklyChallenge = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.weeklyChallenge
);

export const selectChallengesLoaded = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.challengesLoaded
);

export const selectCompletedDailyChallenges = createSelector(
  selectDailyChallenges,
  (challenges) => challenges.filter(c => c.isCompleted)
);

export const selectIncompleteDailyChallenges = createSelector(
  selectDailyChallenges,
  (challenges) => challenges.filter(c => !c.isCompleted)
);

export const selectDailyChallengeProgress = createSelector(
  selectDailyChallenges,
  (challenges) => {
    if (challenges.length === 0) return { completed: 0, total: 0, percentage: 0 };
    
    const completedCount = challenges.filter(c => c.isCompleted).length;
    
    return {
      completed: completedCount,
      total: challenges.length,
      percentage: (completedCount / challenges.length) * 100
    };
  }
);

export const selectChallengesByDifficulty = createSelector(
  selectDailyChallenges,
  (challenges) => {
    const challengesByDifficulty = {
      easy: challenges.filter(c => c.difficulty === 'easy'),
      medium: challenges.filter(c => c.difficulty === 'medium'),
      hard: challenges.filter(c => c.difficulty === 'hard')
    };
    
    return challengesByDifficulty;
  }
);

export const selectNearCompletionChallenges = createSelector(
  selectIncompleteDailyChallenges,
  (incompleteChecks) => incompleteChecks.filter(c => 
    (c.progress / c.target) >= 0.75
  )
);

// Seasonal Events selectors
export const selectSeasonalEvents = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.seasonalEvents
);

export const selectSeasonalEventsLoaded = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.seasonalEventsLoaded
);

export const selectActiveSeasonalEvents = createSelector(
  selectSeasonalEvents,
  (events) => {
    const now = new Date();
    return events.filter(event => 
      new Date(event.startDate) <= now && new Date(event.endDate) >= now
    );
  }
);

export const selectUpcomingSeasonalEvents = createSelector(
  selectSeasonalEvents,
  (events) => {
    const now = new Date();
    return events.filter(event => new Date(event.startDate) > now);
  }
);

// Analytics selectors
export const selectGamificationAnalytics = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.analytics
);

export const selectAnalyticsLoaded = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.analyticsLoaded
);

export const selectEngagementTrends = createSelector(
  selectGamificationAnalytics,
  (analytics) => analytics?.engagementTrends || []
);

export const selectMotivationalInsights = createSelector(
  selectGamificationAnalytics,
  (analytics) => analytics?.motivationalInsights || []
);

// UI State selectors
export const selectCurrentView = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.currentView
);

export const selectRewardNotification = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.rewardNotification
);

export const selectIsRewardNotificationVisible = createSelector(
  selectRewardNotification,
  (notification) => notification?.isVisible || false
);

// Loading state selectors
export const selectGamificationError = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.error
);

export const selectIsLoading = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.isLoading
);

export const selectIsEarningXP = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.isEarningXP
);

export const selectIsEarningBadge = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.isEarningBadge
);

export const selectIsUpdatingStreak = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.isUpdatingStreak
);

export const selectIsPurchasing = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.isPurchasing
);

export const selectIsLoadingAnalytics = createSelector(
  selectGamificationState,
  (state: GamificationState) => state.isLoadingAnalytics
);

export const selectIsAnyGamificationLoading = createSelector(
  selectIsLoading,
  selectIsEarningXP,
  selectIsEarningBadge,
  selectIsUpdatingStreak,
  selectIsPurchasing,
  selectIsLoadingAnalytics,
  (isLoading, isEarningXP, isEarningBadge, isUpdatingStreak, isPurchasing, isLoadingAnalytics) =>
    isLoading || isEarningXP || isEarningBadge || isUpdatingStreak || isPurchasing || isLoadingAnalytics
);

// Error handling selectors
export const selectHasGamificationError = createSelector(
  selectGamificationError,
  (error) => !!error
);

// Dashboard summary selectors
export const selectGamificationDashboardSummary = createSelector(
  selectCurrentLevel,
  selectLevelProgress,
  selectUserBadges,
  selectActiveStreakCount,
  selectCompletedAchievements,
  selectVirtualCurrencyBalance,
  selectCompletedDailyChallenges,
  (level, levelProgress, badges, activeStreaks, completedAchievements, currencyBalance, completedChallenges) => ({
    level,
    progressToNextLevel: levelProgress.progress,
    badgesEarned: badges.length,
    activeStreaks,
    achievementsCompleted: completedAchievements.length,
    virtualCurrency: currencyBalance,
    dailyChallengesCompleted: completedChallenges.length
  })
);

// Motivation and engagement selectors
export const selectMotivationalData = createSelector(
  selectLongestActiveStreak,
  selectRareBadges,
  selectNearCompletionAchievements,
  selectBestLeaderboardRank,
  selectNearCompletionChallenges,
  (longestStreak, rareBadges, nearAchievements, bestRank, nearChallenges) => ({
    longestActiveStreak: longestStreak,
    rareBadgesCount: rareBadges.length,
    achievementsNearCompletion: nearAchievements.length,
    bestLeaderboardRank: bestRank,
    challengesNearCompletion: nearChallenges.length,
    motivationalMessage: generateMotivationalMessage(longestStreak, rareBadges.length, bestRank)
  })
);

// Helper function for motivational messages
function generateMotivationalMessage(
  longestStreak: any, 
  rareBadgesCount: number, 
  bestRank: number | null
): string {
  if (longestStreak && longestStreak.currentCount >= 30) {
    return `Amazing ${longestStreak.currentCount} day streak! You're on fire! 🔥`;
  }
  
  if (rareBadgesCount >= 5) {
    return `Wow! ${rareBadgesCount} rare badges earned! You're a true achiever! 🏆`;
  }
  
  if (bestRank && bestRank <= 10) {
    return `Top ${bestRank} on the leaderboard! You're crushing it! ⭐`;
  }
  
  if (longestStreak && longestStreak.currentCount >= 7) {
    return `${longestStreak.currentCount} day streak! Keep the momentum going! 💪`;
  }
  
  return "Every workout counts! Keep pushing towards your goals! 🎯";
}

// Progress tracking selectors
export const selectOverallGameProgress = createSelector(
  selectCurrentLevel,
  selectBadgeCompletionStats,
  selectAchievementCompletionStats,
  selectActiveStreakCount,
  (level, badgeStats, achievementStats, activeStreaks) => {
    // Calculate overall progress score (0-100)
    const levelScore = Math.min(level * 5, 50); // Max 50 points for level (up to level 10)
    const badgeScore = (badgeStats.percentage / 100) * 25; // Max 25 points for badges
    const achievementScore = (achievementStats.percentage / 100) * 20; // Max 20 points for achievements
    const streakScore = Math.min(activeStreaks * 1, 5); // Max 5 points for active streaks
    
    const totalScore = levelScore + badgeScore + achievementScore + streakScore;
    
    return {
      overallScore: Math.round(totalScore),
      levelContribution: Math.round(levelScore),
      badgeContribution: Math.round(badgeScore),
      achievementContribution: Math.round(achievementScore),
      streakContribution: Math.round(streakScore),
      nextMilestone: getNextMilestone(totalScore)
    };
  }
);

// Helper function for next milestone
function getNextMilestone(currentScore: number): { score: number; description: string } {
  if (currentScore < 25) return { score: 25, description: "Fitness Rookie Complete" };
  if (currentScore < 50) return { score: 50, description: "Fitness Enthusiast" };
  if (currentScore < 75) return { score: 75, description: "Fitness Expert" };
  if (currentScore < 90) return { score: 90, description: "Fitness Master" };
  return { score: 100, description: "Fitness Legend" };
}

// Recent activity summary
export const selectRecentGamificationActivity = createSelector(
  selectRecentXPTransactions,
  selectRecentlyEarnedBadges,
  selectRecentlyUnlockedAchievements,
  (xpTransactions, badges, achievements) => {
    const activities: Array<{
      type: 'xp' | 'badge' | 'achievement';
      timestamp: Date;
      description: string;
      value?: number;
      icon: string;
    }> = [];
    
    // Add XP transactions
    xpTransactions.slice(0, 5).forEach(transaction => {
      activities.push({
        type: 'xp',
        timestamp: transaction.timestamp,
        description: transaction.description,
        value: transaction.points,
        icon: '⭐'
      });
    });
    
    // Add recent badges
    badges.slice(0, 3).forEach(badge => {
      activities.push({
        type: 'badge',
        timestamp: badge.earnedAt,
        description: `Earned "${badge.badge.name}" badge`,
        icon: '🏆'
      });
    });
    
    // Add recent achievements
    achievements.slice(0, 3).forEach(achievement => {
      activities.push({
        type: 'achievement',
        timestamp: achievement.unlockedAt,
        description: `Unlocked "${achievement.title}"`,
        icon: '🎯'
      });
    });
    
    // Sort by timestamp (most recent first)
    return activities
      .sort((a, b) => new Date(b.timestamp).getTime() - new Date(a.timestamp).getTime())
      .slice(0, 10);
  }
);
