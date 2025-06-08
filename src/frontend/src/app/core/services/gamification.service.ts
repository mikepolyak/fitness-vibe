import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  Badge, 
  UserBadge, 
  BadgeCategory, 
  BadgeRarity 
} from '../../shared/models/user.model';

/**
 * Gamification Service - the API communication specialist for all reward and motivation operations.
 * Think of this as the arcade operator who manages points, prizes, achievements, and competitions
 * to keep your fitness journey exciting and rewarding!
 */
@Injectable({
  providedIn: 'root'
})
export class GamificationService {
  private readonly baseUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient) {}

  // ============================================================================
  // EXPERIENCE POINTS AND LEVELING SYSTEM
  // ============================================================================

  /**
   * Award experience points for fitness activities - like getting points for playing arcade games
   */
  earnExperiencePoints(
    points: number, 
    source: string, 
    sourceId?: number, 
    description?: string
  ): Observable<{
    pointsEarned: number;
    totalPoints: number;
    newLevel?: number;
    leveledUp: boolean;
    levelUpRewards?: Array<{
      type: 'badge' | 'feature_unlock' | 'virtual_currency';
      value: any;
      description: string;
    }>;
  }> {
    return this.http.post<{
      pointsEarned: number;
      totalPoints: number;
      newLevel?: number;
      leveledUp: boolean;
      levelUpRewards?: Array<{
        type: 'badge' | 'feature_unlock' | 'virtual_currency';
        value: any;
        description: string;
      }>;
    }>(`${this.baseUrl}/gamification/xp/earn`, {
      points,
      source,
      sourceId,
      description,
      timestamp: new Date().toISOString()
    });
  }

  /**
   * Check if user should level up based on current XP
   */
  checkLevelUp(currentXP: number): Observable<{
    leveledUp: boolean;
    newLevel: number;
    previousLevel: number;
    rewards: Array<{
      type: 'badge' | 'feature_unlock' | 'virtual_currency';
      value: any;
      description: string;
    }>;
  }> {
    return this.http.post<{
      leveledUp: boolean;
      newLevel: number;
      previousLevel: number;
      rewards: Array<{
        type: 'badge' | 'feature_unlock' | 'virtual_currency';
        value: any;
        description: string;
      }>;
    }>(`${this.baseUrl}/gamification/level/check`, { currentXP });
  }

  /**
   * Get XP transaction history
   */
  getXPHistory(
    limit: number = 50, 
    offset: number = 0
  ): Observable<Array<{
    id: number;
    points: number;
    source: string;
    description: string;
    timestamp: Date;
  }>> {
    const params = new HttpParams()
      .set('limit', limit.toString())
      .set('offset', offset.toString());

    return this.http.get<Array<{
      id: number;
      points: number;
      source: string;
      description: string;
      timestamp: Date;
    }>>(`${this.baseUrl}/gamification/xp/history`, { params });
  }

  // ============================================================================
  // BADGE SYSTEM
  // ============================================================================

  /**
   * Get all available badges that can be earned
   */
  getAvailableBadges(): Observable<Badge[]> {
    return this.http.get<Badge[]>(`${this.baseUrl}/gamification/badges/available`);
  }

  /**
   * Get badges the user has already earned
   */
  getUserBadges(): Observable<UserBadge[]> {
    return this.http.get<UserBadge[]>(`${this.baseUrl}/gamification/badges/user`);
  }

  /**
   * Award a badge to the user
   */
  earnBadge(
    badgeId: number, 
    context?: string, 
    activityId?: number, 
    goalId?: number
  ): Observable<{
    userBadge: UserBadge;
    experiencePointsBonus: number;
  }> {
    return this.http.post<{
      userBadge: UserBadge;
      experiencePointsBonus: number;
    }>(`${this.baseUrl}/gamification/badges/earn`, {
      badgeId,
      context,
      activityId,
      goalId,
      earnedAt: new Date().toISOString()
    });
  }

  /**
   * Check if user is eligible for any badges based on recent activity
   */
  checkBadgeEligibility(
    triggerType: 'activity_completed' | 'goal_achieved' | 'streak_reached' | 'milestone_achieved',
    triggerData: any
  ): Observable<Array<{ 
    badgeId: number; 
    badge: Badge; 
    criteria: any 
  }>> {
    return this.http.post<Array<{ 
      badgeId: number; 
      badge: Badge; 
      criteria: any 
    }>>(`${this.baseUrl}/gamification/badges/check-eligibility`, {
      triggerType,
      triggerData
    });
  }

  /**
   * Get badge categories and their completion stats
   */
  getBadgeStats(): Observable<{
    totalBadges: number;
    earnedBadges: number;
    byCategory: { [key in BadgeCategory]?: { available: number; earned: number } };
    byRarity: { [key in BadgeRarity]?: { available: number; earned: number } };
    recentlyEarned: UserBadge[];
  }> {
    return this.http.get<{
      totalBadges: number;
      earnedBadges: number;
      byCategory: { [key in BadgeCategory]?: { available: number; earned: number } };
      byRarity: { [key in BadgeRarity]?: { available: number; earned: number } };
      recentlyEarned: UserBadge[];
    }>(`${this.baseUrl}/gamification/badges/stats`);
  }

  // ============================================================================
  // STREAKS SYSTEM
  // ============================================================================

  /**
   * Get user's active streaks (consistency tracking)
   */
  getActiveStreaks(): Observable<Array<{
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
  }>> {
    return this.http.get<Array<{
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
    }>>(`${this.baseUrl}/gamification/streaks`);
  }

  /**
   * Update a streak (extend or break)
   */
  updateStreak(
    streakType: string, 
    increment: boolean, 
    activityDate?: Date
  ): Observable<{
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
  }> {
    return this.http.post<{
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
    }>(`${this.baseUrl}/gamification/streaks/update`, {
      streakType,
      increment,
      activityDate: activityDate?.toISOString() || new Date().toISOString()
    });
  }

  /**
   * Break a streak (with encouraging message)
   */
  breakStreak(
    streakId: number, 
    reason: 'missed_day' | 'user_reset' | 'goal_failed'
  ): Observable<{
    finalCount: number;
    encouragementMessage: string;
  }> {
    return this.http.post<{
      finalCount: number;
      encouragementMessage: string;
    }>(`${this.baseUrl}/gamification/streaks/${streakId}/break`, {
      reason,
      breakDate: new Date().toISOString()
    });
  }

  /**
   * Check daily streaks automatically (called by scheduler)
   */
  checkDailyStreaks(): Observable<Array<{
    id: number;
    type: string;
    currentCount: number;
    wasExtended: boolean;
    milestoneReached?: any;
  }>> {
    return this.http.post<Array<{
      id: number;
      type: string;
      currentCount: number;
      wasExtended: boolean;
      milestoneReached?: any;
    }>>(`${this.baseUrl}/gamification/streaks/daily-check`, {});
  }

  // ============================================================================
  // ACHIEVEMENTS SYSTEM
  // ============================================================================

  /**
   * Get all achievements and their progress
   */
  getAchievements(): Observable<Array<{
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
  }>> {
    return this.http.get<Array<{
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
    }>>(`${this.baseUrl}/gamification/achievements`);
  }

  /**
   * Unlock an achievement
   */
  unlockAchievement(
    achievementId: number, 
    context?: any
  ): Observable<{
    achievement: {
      id: number;
      title: string;
      description: string;
      unlockedAt: Date;
      reward: any;
    };
  }> {
    return this.http.post<{
      achievement: {
        id: number;
        title: string;
        description: string;
        unlockedAt: Date;
        reward: any;
      };
    }>(`${this.baseUrl}/gamification/achievements/${achievementId}/unlock`, {
      context,
      unlockedAt: new Date().toISOString()
    });
  }

  /**
   * Update achievement progress
   */
  updateAchievementProgress(
    achievementId: number, 
    progressValue: number
  ): Observable<{
    progress: {
      current: number;
      target: number;
      percentage: number;
    };
    isCompleted: boolean;
  }> {
    return this.http.post<{
      progress: {
        current: number;
        target: number;
        percentage: number;
      };
      isCompleted: boolean;
    }>(`${this.baseUrl}/gamification/achievements/${achievementId}/progress`, {
      progressValue
    });
  }

  // ============================================================================
  // LEADERBOARDS AND COMPETITION
  // ============================================================================

  /**
   * Get leaderboard rankings
   */
  getLeaderboards(
    type: 'xp' | 'activities' | 'goals_completed' | 'streak_count' | 'badges_earned',
    scope: 'global' | 'friends' | 'local' | 'age_group',
    period: 'daily' | 'weekly' | 'monthly' | 'all_time'
  ): Observable<{
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
  }> {
    const params = new HttpParams()
      .set('type', type)
      .set('scope', scope)
      .set('period', period);

    return this.http.get<{
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
    }>(`${this.baseUrl}/gamification/leaderboards`, { params });
  }

  /**
   * Get user's ranking across all leaderboards
   */
  getUserRankings(): Observable<{
    rankings: { [key: string]: { rank: number; total: number; percentile: number } };
    bestRanks: Array<{
      type: string;
      scope: string;
      period: string;
      rank: number;
      total: number;
    }>;
  }> {
    return this.http.get<{
      rankings: { [key: string]: { rank: number; total: number; percentile: number } };
      bestRanks: Array<{
        type: string;
        scope: string;
        period: string;
        rank: number;
        total: number;
      }>;
    }>(`${this.baseUrl}/gamification/leaderboards/user-rankings`);
  }

  // ============================================================================
  // VIRTUAL CURRENCY SYSTEM
  // ============================================================================

  /**
   * Get user's virtual currency balance and transaction history
   */
  getVirtualCurrency(): Observable<{
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
  }> {
    return this.http.get<{
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
    }>(`${this.baseUrl}/gamification/currency`);
  }

  /**
   * Earn virtual currency
   */
  earnVirtualCurrency(
    amount: number, 
    source: string, 
    description: string
  ): Observable<{
    amountEarned: number;
    newBalance: number;
    transactionId: number;
  }> {
    return this.http.post<{
      amountEarned: number;
      newBalance: number;
      transactionId: number;
    }>(`${this.baseUrl}/gamification/currency/earn`, {
      amount,
      source,
      description,
      timestamp: new Date().toISOString()
    });
  }

  /**
   * Spend virtual currency
   */
  spendVirtualCurrency(
    amount: number, 
    itemId: number, 
    itemType: string, 
    itemName: string
  ): Observable<{
    amountSpent: number;
    newBalance: number;
    purchasedItem: any;
    transactionId: number;
  }> {
    return this.http.post<{
      amountSpent: number;
      newBalance: number;
      purchasedItem: any;
      transactionId: number;
    }>(`${this.baseUrl}/gamification/currency/spend`, {
      amount,
      itemId,
      itemType,
      itemName,
      timestamp: new Date().toISOString()
    });
  }

  // ============================================================================
  // REWARDS STORE
  // ============================================================================

  /**
   * Get rewards store items
   */
  getRewardsStore(): Observable<{
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
    featuredItems: number[];
    userOwnedItems: number[];
  }> {
    return this.http.get<{
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
      featuredItems: number[];
      userOwnedItems: number[];
    }>(`${this.baseUrl}/gamification/store`);
  }

  /**
   * Purchase an item from the store
   */
  purchaseStoreItem(
    itemId: number, 
    paymentMethod: 'virtual_currency' | 'real_money'
  ): Observable<{
    item: any;
    transactionId: number;
    newCurrencyBalance?: number;
  }> {
    return this.http.post<{
      item: any;
      transactionId: number;
      newCurrencyBalance?: number;
    }>(`${this.baseUrl}/gamification/store/purchase`, {
      itemId,
      paymentMethod,
      timestamp: new Date().toISOString()
    });
  }

  /**
   * Get user's owned items
   */
  getUserOwnedItems(): Observable<Array<{
    id: number;
    itemId: number;
    itemName: string;
    itemType: string;
    purchasedAt: Date;
    isActive: boolean;
  }>> {
    return this.http.get<Array<{
      id: number;
      itemId: number;
      itemName: string;
      itemType: string;
      purchasedAt: Date;
      isActive: boolean;
    }>>(`${this.baseUrl}/gamification/store/owned`);
  }

  // ============================================================================
  // DAILY AND WEEKLY CHALLENGES
  // ============================================================================

  /**
   * Get daily challenges and weekly challenge
   */
  getDailyChallenges(): Observable<{
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
      progress: number;
      expiresAt: Date;
    };
  }> {
    return this.http.get<{
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
        progress: number;
        expiresAt: Date;
      };
    }>(`${this.baseUrl}/gamification/challenges`);
  }

  /**
   * Complete a daily challenge
   */
  completeDailyChallenge(challengeId: number): Observable<{
    reward: {
      xp: number;
      virtualCurrency: number;
      badge?: Badge;
    };
    newChallenges?: any[];
  }> {
    return this.http.post<{
      reward: {
        xp: number;
        virtualCurrency: number;
        badge?: Badge;
      };
      newChallenges?: any[];
    }>(`${this.baseUrl}/gamification/challenges/${challengeId}/complete`, {
      completedAt: new Date().toISOString()
    });
  }

  /**
   * Update challenge progress
   */
  updateChallengeProgress(
    challengeId: number, 
    progressValue: number
  ): Observable<{
    challenge: {
      id: number;
      progress: number;
      isCompleted: boolean;
    };
  }> {
    return this.http.post<{
      challenge: {
        id: number;
        progress: number;
        isCompleted: boolean;
      };
    }>(`${this.baseUrl}/gamification/challenges/${challengeId}/progress`, {
      progressValue
    });
  }

  // ============================================================================
  // SEASONAL EVENTS
  // ============================================================================

  /**
   * Get active seasonal events
   */
  getSeasonalEvents(): Observable<Array<{
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
  }>> {
    return this.http.get<Array<{
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
    }>>(`${this.baseUrl}/gamification/events/seasonal`);
  }

  /**
   * Participate in a seasonal event
   */
  participateInSeasonalEvent(eventId: number): Observable<{
    participationReward?: any;
  }> {
    return this.http.post<{
      participationReward?: any;
    }>(`${this.baseUrl}/gamification/events/seasonal/${eventId}/participate`, {
      participatedAt: new Date().toISOString()
    });
  }

  // ============================================================================
  // ANALYTICS AND INSIGHTS
  // ============================================================================

  /**
   * Get comprehensive gamification analytics
   */
  getGamificationAnalytics(): Observable<{
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
  }> {
    return this.http.get<{
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
    }>(`${this.baseUrl}/gamification/analytics`);
  }

  /**
   * Get personalized motivation recommendations
   */
  getMotivationRecommendations(): Observable<{
    recommendations: Array<{
      type: 'badge_opportunity' | 'streak_suggestion' | 'challenge_recommendation' | 'social_engagement';
      title: string;
      description: string;
      actionText: string;
      priority: 'low' | 'medium' | 'high';
      estimatedReward: {
        xp?: number;
        coins?: number;
        badge?: string;
      };
    }>;
    personalizedMessage: string;
  }> {
    return this.http.get<{
      recommendations: Array<{
        type: 'badge_opportunity' | 'streak_suggestion' | 'challenge_recommendation' | 'social_engagement';
        title: string;
        description: string;
        actionText: string;
        priority: 'low' | 'medium' | 'high';
        estimatedReward: {
          xp?: number;
          coins?: number;
          badge?: string;
        };
      }>;
      personalizedMessage: string;
    }>(`${this.baseUrl}/gamification/recommendations`);
  }

  /**
   * Get gamification engagement summary
   */
  getEngagementSummary(period: 'week' | 'month' = 'week'): Observable<{
    period: string;
    totalXPEarned: number;
    badgesEarned: number;
    streaksActive: number;
    challengesCompleted: number;
    leaderboardMovement: {
      improved: number;
      declined: number;
      stable: number;
    };
    motivationScore: number; // 0-100
    engagementTrend: 'increasing' | 'stable' | 'decreasing';
    personalizedTip: string;
  }> {
    const params = new HttpParams().set('period', period);
    
    return this.http.get<{
      period: string;
      totalXPEarned: number;
      badgesEarned: number;
      streaksActive: number;
      challengesCompleted: number;
      leaderboardMovement: {
        improved: number;
        declined: number;
        stable: number;
      };
      motivationScore: number;
      engagementTrend: 'increasing' | 'stable' | 'decreasing';
      personalizedTip: string;
    }>(`${this.baseUrl}/gamification/engagement-summary`, { params });
  }
}
