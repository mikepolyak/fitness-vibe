import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  UserGoal, 
  GoalType, 
  GoalFrequency, 
  GoalStatus,
  CreateGoalRequest 
} from '../../shared/models/user.model';

/**
 * Goals Service - the API communication specialist for all fitness goal operations.
 * Think of this as the personal goal coordinator who manages target setting,
 * progress tracking, milestone achievements, and motivation systems.
 */
@Injectable({
  providedIn: 'root'
})
export class GoalsService {
  private readonly baseUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient) {}

  // ============================================================================
  // CORE GOAL MANAGEMENT
  // ============================================================================

  /**
   * Get user's goals with optional filtering - like reviewing your fitness target list
   */
  getGoals(
    userId?: number, 
    status?: GoalStatus, 
    type?: GoalType
  ): Observable<UserGoal[]> {
    let params = new HttpParams();
    
    if (userId) {
      params = params.set('userId', userId.toString());
    }
    if (status) {
      params = params.set('status', status);
    }
    if (type) {
      params = params.set('type', type);
    }

    return this.http.get<UserGoal[]>(`${this.baseUrl}/goals`, { params });
  }

  /**
   * Get a specific goal by ID - like examining details of one fitness target
   */
  getGoalById(goalId: number): Observable<UserGoal> {
    return this.http.get<UserGoal>(`${this.baseUrl}/goals/${goalId}`);
  }

  /**
   * Create a new fitness goal - like setting a new personal challenge
   */
  createGoal(goal: CreateGoalRequest): Observable<UserGoal> {
    return this.http.post<UserGoal>(`${this.baseUrl}/goals`, goal);
  }

  /**
   * Update an existing goal - like adjusting your fitness targets
   */
  updateGoal(goalId: number, updates: Partial<UserGoal>): Observable<UserGoal> {
    return this.http.put<UserGoal>(`${this.baseUrl}/goals/${goalId}`, updates);
  }

  /**
   * Delete a goal - like removing a fitness target that's no longer relevant
   */
  deleteGoal(goalId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/goals/${goalId}`);
  }

  /**
   * Update goal progress - like logging advancement after activities
   */
  updateGoalProgress(
    goalId: number, 
    progressValue: number, 
    activityId?: number
  ): Observable<UserGoal> {
    return this.http.post<UserGoal>(`${this.baseUrl}/goals/${goalId}/progress`, {
      progressValue,
      activityId,
      timestamp: new Date().toISOString()
    });
  }

  /**
   * Manually complete a goal - like officially celebrating achievement
   */
  completeGoal(goalId: number): Observable<{
    goal: UserGoal;
    experiencePointsEarned: number;
    badgesEarned?: any[];
  }> {
    return this.http.post<{
      goal: UserGoal;
      experiencePointsEarned: number;
      badgesEarned?: any[];
    }>(`${this.baseUrl}/goals/${goalId}/complete`, {});
  }

  /**
   * Abandon a goal - like officially stepping back from a target
   */
  abandonGoal(goalId: number, reason?: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/goals/${goalId}/abandon`, { 
      reason,
      abandonedAt: new Date().toISOString()
    });
  }

  // ============================================================================
  // GOAL SUGGESTIONS AND TEMPLATES
  // ============================================================================

  /**
   * Get AI-powered goal suggestions - like having a personal trainer recommend targets
   */
  getGoalSuggestions(
    basedOnActivity: boolean = true, 
    userLevel?: number
  ): Observable<Partial<CreateGoalRequest>[]> {
    let params = new HttpParams()
      .set('basedOnActivity', basedOnActivity.toString());
    
    if (userLevel) {
      params = params.set('userLevel', userLevel.toString());
    }

    return this.http.get<Partial<CreateGoalRequest>[]>(
      `${this.baseUrl}/goals/suggestions`, 
      { params }
    );
  }

  /**
   * Get goal templates - like browsing preset fitness challenge templates
   */
  getGoalTemplates(): Observable<Array<Omit<CreateGoalRequest, 'startDate' | 'endDate'> & { 
    id: number; 
    name: string; 
    description: string; 
    category: string;
    difficulty: 'beginner' | 'intermediate' | 'advanced';
    estimatedDuration: number;
    tags: string[];
    popularityScore: number;
  }>> {
    return this.http.get<Array<Omit<CreateGoalRequest, 'startDate' | 'endDate'> & { 
      id: number; 
      name: string; 
      description: string; 
      category: string;
      difficulty: 'beginner' | 'intermediate' | 'advanced';
      estimatedDuration: number;
      tags: string[];
      popularityScore: number;
    }>>(`${this.baseUrl}/goals/templates`);
  }

  /**
   * Create a goal from a template - like using a preset workout plan
   */
  createGoalFromTemplate(
    templateId: number, 
    customizations?: Partial<CreateGoalRequest>
  ): Observable<UserGoal> {
    return this.http.post<UserGoal>(`${this.baseUrl}/goals/templates/${templateId}/create`, {
      customizations,
      startDate: new Date().toISOString(),
      ...customizations
    });
  }

  /**
   * Save a custom goal as a template for future use
   */
  saveGoalAsTemplate(
    goalId: number, 
    templateName: string, 
    templateDescription?: string
  ): Observable<{ templateId: number }> {
    return this.http.post<{ templateId: number }>(
      `${this.baseUrl}/goals/${goalId}/save-as-template`, 
      {
        name: templateName,
        description: templateDescription,
        isPublic: false
      }
    );
  }

  // ============================================================================
  // GOAL ADJUSTMENTS AND AI COACHING
  // ============================================================================

  /**
   * Request AI-powered goal adjustment - like having a smart trainer adjust your targets
   */
  requestGoalAdjustment(
    goalId: number, 
    reason: 'too_easy' | 'too_hard' | 'auto_adaptive'
  ): Observable<{
    goal: UserGoal;
    adjustmentReason: string;
    previousTarget: number;
    newTarget: number;
  }> {
    return this.http.post<{
      goal: UserGoal;
      adjustmentReason: string;
      previousTarget: number;
      newTarget: number;
    }>(`${this.baseUrl}/goals/${goalId}/adjust`, {
      reason,
      requestedAt: new Date().toISOString()
    });
  }

  /**
   * Get goal adjustment recommendations based on performance patterns
   */
  getGoalAdjustmentRecommendations(): Observable<Array<{
    goalId: number;
    goalTitle: string;
    recommendationType: 'increase_target' | 'decrease_target' | 'extend_deadline' | 'break_down';
    reasoning: string;
    suggestedChanges: any;
    confidenceScore: number;
  }>> {
    return this.http.get<Array<{
      goalId: number;
      goalTitle: string;
      recommendationType: 'increase_target' | 'decrease_target' | 'extend_deadline' | 'break_down';
      reasoning: string;
      suggestedChanges: any;
      confidenceScore: number;
    }>>(`${this.baseUrl}/goals/adjustment-recommendations`);
  }

  // ============================================================================
  // GOAL ANALYTICS AND INSIGHTS
  // ============================================================================

  /**
   * Get comprehensive goal analytics - like generating a goal achievement report card
   */
  getGoalAnalytics(period: 'week' | 'month' | 'quarter' | 'year' = 'month'): Observable<{
    totalGoals: number;
    completedGoals: number;
    activeGoals: number;
    abandonedGoals: number;
    averageCompletionTime: number;
    completionRate: number;
    goalsByType: { [key in GoalType]?: number };
    goalsByFrequency: { [key in GoalFrequency]?: number };
    streakData: {
      currentStreak: number;
      longestStreak: number;
      streakGoalType?: GoalType;
    };
    progressTrends: Array<{
      date: Date;
      activeGoals: number;
      completedGoals: number;
      progressPercentage: number;
    }>;
    motivationalInsights: string[];
    improvementSuggestions: string[];
  }> {
    const params = new HttpParams().set('period', period);
    
    return this.http.get<{
      totalGoals: number;
      completedGoals: number;
      activeGoals: number;
      abandonedGoals: number;
      averageCompletionTime: number;
      completionRate: number;
      goalsByType: { [key in GoalType]?: number };
      goalsByFrequency: { [key in GoalFrequency]?: number };
      streakData: {
        currentStreak: number;
        longestStreak: number;
        streakGoalType?: GoalType;
      };
      progressTrends: Array<{
        date: Date;
        activeGoals: number;
        completedGoals: number;
        progressPercentage: number;
      }>;
      motivationalInsights: string[];
      improvementSuggestions: string[];
    }>(`${this.baseUrl}/goals/analytics`, { params });
  }

  /**
   * Get goal performance comparison with other users (anonymized)
   */
  getGoalPerformanceBenchmarks(): Observable<{
    userCompletionRate: number;
    averageCompletionRate: number;
    userAverageGoalDuration: number;
    averageGoalDuration: number;
    popularGoalTypes: Array<{ type: GoalType; percentage: number }>;
    successFactors: string[];
  }> {
    return this.http.get<{
      userCompletionRate: number;
      averageCompletionRate: number;
      userAverageGoalDuration: number;
      averageGoalDuration: number;
      popularGoalTypes: Array<{ type: GoalType; percentage: number }>;
      successFactors: string[];
    }>(`${this.baseUrl}/goals/benchmarks`);
  }

  // ============================================================================
  // GOAL MILESTONES AND ACHIEVEMENTS
  // ============================================================================

  /**
   * Get milestones for a specific goal - like checking checkpoint achievements
   */
  getGoalMilestones(goalId: number): Observable<Array<{
    id: number;
    goalId: number;
    milestone: number; // percentage (25, 50, 75, 100)
    achievedAt?: Date;
    message: string;
    reward?: {
      type: 'badge' | 'xp' | 'virtual_item';
      value: string | number;
    };
  }>> {
    return this.http.get<Array<{
      id: number;
      goalId: number;
      milestone: number;
      achievedAt?: Date;
      message: string;
      reward?: {
        type: 'badge' | 'xp' | 'virtual_item';
        value: string | number;
      };
    }>>(`${this.baseUrl}/goals/${goalId}/milestones`);
  }

  /**
   * Mark a milestone as achieved - like celebrating a checkpoint
   */
  achieveMilestone(goalId: number, milestoneId: number): Observable<{
    milestoneId: number;
    achievedAt: Date;
    reward?: {
      type: 'badge' | 'xp' | 'virtual_item';
      value: string | number;
    };
  }> {
    return this.http.post<{
      milestoneId: number;
      achievedAt: Date;
      reward?: {
        type: 'badge' | 'xp' | 'virtual_item';
        value: string | number;
      };
    }>(`${this.baseUrl}/goals/${goalId}/milestones/${milestoneId}/achieve`, {});
  }

  /**
   * Get all achieved milestones for the user
   */
  getAllAchievedMilestones(): Observable<Array<{
    id: number;
    goalId: number;
    goalTitle: string;
    milestone: number;
    achievedAt: Date;
    reward?: any;
  }>> {
    return this.http.get<Array<{
      id: number;
      goalId: number;
      goalTitle: string;
      milestone: number;
      achievedAt: Date;
      reward?: any;
    }>>(`${this.baseUrl}/goals/milestones/achieved`);
  }

  // ============================================================================
  // BULK GOAL PROGRESS TRACKING
  // ============================================================================

  /**
   * Track progress for multiple goals based on activities - like auto-updating all relevant targets
   */
  trackGoalProgress(activities: Array<{
    activityId: number;
    value: number;
    unit: string;
  }>): Observable<UserGoal[]> {
    return this.http.post<UserGoal[]>(`${this.baseUrl}/goals/track-progress`, {
      activities,
      timestamp: new Date().toISOString()
    });
  }

  /**
   * Bulk update multiple goals
   */
  bulkUpdateGoals(updates: Array<{
    goalId: number;
    updates: Partial<UserGoal>;
  }>): Observable<UserGoal[]> {
    return this.http.put<UserGoal[]>(`${this.baseUrl}/goals/bulk-update`, { updates });
  }

  // ============================================================================
  // SOCIAL GOAL FEATURES
  // ============================================================================

  /**
   * Share a goal with friends or community - like announcing your fitness commitment
   */
  shareGoal(
    goalId: number, 
    platform: 'feed' | 'friends' | 'external', 
    message?: string
  ): Observable<{ shareId: number; sharedAt: Date }> {
    return this.http.post<{ shareId: number; sharedAt: Date }>(
      `${this.baseUrl}/goals/${goalId}/share`, 
      {
        platform,
        message,
        sharedAt: new Date().toISOString()
      }
    );
  }

  /**
   * Get shared goals from friends and community - like browsing others' fitness commitments
   */
  getSharedGoals(
    friendsOnly: boolean = false, 
    page: number = 1,
    limit: number = 20
  ): Observable<Array<UserGoal & { 
    sharedBy: { id: number; firstName: string; lastName: string; avatarUrl?: string };
    sharedAt: Date;
    likes: number;
    comments: number;
    isLikedByCurrentUser: boolean;
  }>> {
    const params = new HttpParams()
      .set('friendsOnly', friendsOnly.toString())
      .set('page', page.toString())
      .set('limit', limit.toString());

    return this.http.get<Array<UserGoal & { 
      sharedBy: { id: number; firstName: string; lastName: string; avatarUrl?: string };
      sharedAt: Date;
      likes: number;
      comments: number;
      isLikedByCurrentUser: boolean;
    }>>(`${this.baseUrl}/goals/shared`, { params });
  }

  /**
   * Like a shared goal
   */
  likeSharedGoal(shareId: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/goals/shared/${shareId}/like`, {});
  }

  /**
   * Unlike a shared goal
   */
  unlikeSharedGoal(shareId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/goals/shared/${shareId}/like`);
  }

  /**
   * Comment on a shared goal
   */
  commentOnSharedGoal(shareId: number, comment: string): Observable<{
    commentId: number;
    comment: string;
    createdAt: Date;
  }> {
    return this.http.post<{
      commentId: number;
      comment: string;
      createdAt: Date;
    }>(`${this.baseUrl}/goals/shared/${shareId}/comments`, { comment });
  }

  // ============================================================================
  // GOAL REMINDERS AND NOTIFICATIONS
  // ============================================================================

  /**
   * Schedule a goal reminder - like setting fitness commitment alarms
   */
  scheduleGoalReminder(
    goalId: number, 
    reminderType: 'daily' | 'weekly' | 'custom',
    time?: string
  ): Observable<{ reminderId: number }> {
    return this.http.post<{ reminderId: number }>(
      `${this.baseUrl}/goals/${goalId}/reminders`, 
      {
        type: reminderType,
        time,
        isActive: true
      }
    );
  }

  /**
   * Cancel a goal reminder
   */
  cancelGoalReminder(goalId: number, reminderId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/goals/${goalId}/reminders/${reminderId}`);
  }

  /**
   * Get all active reminders for the user
   */
  getGoalReminders(): Observable<Array<{
    id: number;
    goalId: number;
    goalTitle: string;
    type: 'daily' | 'weekly' | 'custom';
    time?: string;
    isActive: boolean;
    nextReminderAt: Date;
  }>> {
    return this.http.get<Array<{
      id: number;
      goalId: number;
      goalTitle: string;
      type: 'daily' | 'weekly' | 'custom';
      time?: string;
      isActive: boolean;
      nextReminderAt: Date;
    }>>(`${this.baseUrl}/goals/reminders`);
  }

  // ============================================================================
  // GOAL CHALLENGES AND COMPETITIONS
  // ============================================================================

  /**
   * Create a goal-based challenge for friends
   */
  createGoalChallenge(
    goalId: number,
    challengeDetails: {
      title: string;
      description: string;
      duration: number; // in days
      invitedFriends: number[];
      isPublic: boolean;
    }
  ): Observable<{ challengeId: number }> {
    return this.http.post<{ challengeId: number }>(
      `${this.baseUrl}/goals/${goalId}/challenges`, 
      challengeDetails
    );
  }

  /**
   * Join a goal challenge
   */
  joinGoalChallenge(challengeId: number): Observable<{ joined: boolean }> {
    return this.http.post<{ joined: boolean }>(
      `${this.baseUrl}/goal-challenges/${challengeId}/join`, 
      {}
    );
  }

  /**
   * Get goal challenges leaderboard
   */
  getGoalChallengeLeaderboard(challengeId: number): Observable<Array<{
    userId: number;
    userName: string;
    userAvatar?: string;
    progress: number;
    rank: number;
    isCurrentUser: boolean;
  }>> {
    return this.http.get<Array<{
      userId: number;
      userName: string;
      userAvatar?: string;
      progress: number;
      rank: number;
      isCurrentUser: boolean;
    }>>(`${this.baseUrl}/goal-challenges/${challengeId}/leaderboard`);
  }

  // ============================================================================
  // GOAL DATA EXPORT AND INSIGHTS
  // ============================================================================

  /**
   * Export goal data to various formats
   */
  exportGoalData(
    format: 'csv' | 'json' | 'pdf' = 'csv',
    filters?: {
      startDate?: Date;
      endDate?: Date;
      status?: GoalStatus[];
      type?: GoalType[];
    }
  ): Observable<Blob> {
    let params = new HttpParams().set('format', format);
    
    if (filters) {
      if (filters.startDate) {
        params = params.set('startDate', filters.startDate.toISOString());
      }
      if (filters.endDate) {
        params = params.set('endDate', filters.endDate.toISOString());
      }
      if (filters.status) {
        filters.status.forEach(status => {
          params = params.append('status', status);
        });
      }
      if (filters.type) {
        filters.type.forEach(type => {
          params = params.append('type', type);
        });
      }
    }

    return this.http.get(`${this.baseUrl}/goals/export`, {
      params,
      responseType: 'blob'
    });
  }

  /**
   * Get goal achievement insights and patterns
   */
  getGoalInsights(): Observable<{
    bestPerformingGoalTypes: Array<{ type: GoalType; successRate: number }>;
    optimalGoalDuration: number; // in days
    mostProductiveDaysOfWeek: Array<{ day: string; successRate: number }>;
    streakPatterns: Array<{ pattern: string; impact: string }>;
    personalizedTips: string[];
  }> {
    return this.http.get<{
      bestPerformingGoalTypes: Array<{ type: GoalType; successRate: number }>;
      optimalGoalDuration: number;
      mostProductiveDaysOfWeek: Array<{ day: string; successRate: number }>;
      streakPatterns: Array<{ pattern: string; impact: string }>;
      personalizedTips: string[];
    }>(`${this.baseUrl}/goals/insights`);
  }
}
