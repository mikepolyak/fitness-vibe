import { createAction, props } from '@ngrx/store';
import { 
  UserGoal, 
  GoalType, 
  GoalFrequency, 
  GoalStatus,
  CreateGoalRequest 
} from '../../shared/models/user.model';

/**
 * Goals Actions - the commands that trigger goal-related state changes.
 * Think of these as different goal management activities: setting new targets,
 * tracking progress, celebrating achievements, adjusting ambitions, etc.
 */

// Load user goals
export const loadGoals = createAction(
  '[Goals] Load Goals',
  props<{ userId?: number; status?: GoalStatus; type?: GoalType }>()
);

export const loadGoalsSuccess = createAction(
  '[Goals] Load Goals Success',
  props<{ goals: UserGoal[] }>()
);

export const loadGoalsFailure = createAction(
  '[Goals] Load Goals Failure',
  props<{ error: string }>()
);

// Load specific goal by ID
export const loadGoalById = createAction(
  '[Goals] Load Goal By ID',
  props<{ goalId: number }>()
);

export const loadGoalByIdSuccess = createAction(
  '[Goals] Load Goal By ID Success',
  props<{ goal: UserGoal }>()
);

export const loadGoalByIdFailure = createAction(
  '[Goals] Load Goal By ID Failure',
  props<{ error: string }>()
);

// Create new goal
export const createGoal = createAction(
  '[Goals] Create Goal',
  props<{ goal: CreateGoalRequest }>()
);

export const createGoalSuccess = createAction(
  '[Goals] Create Goal Success',
  props<{ goal: UserGoal }>()
);

export const createGoalFailure = createAction(
  '[Goals] Create Goal Failure',
  props<{ error: string }>()
);

// Update existing goal
export const updateGoal = createAction(
  '[Goals] Update Goal',
  props<{ goalId: number; updates: Partial<UserGoal> }>()
);

export const updateGoalSuccess = createAction(
  '[Goals] Update Goal Success',
  props<{ goal: UserGoal }>()
);

export const updateGoalFailure = createAction(
  '[Goals] Update Goal Failure',
  props<{ error: string }>()
);

// Delete goal
export const deleteGoal = createAction(
  '[Goals] Delete Goal',
  props<{ goalId: number }>()
);

export const deleteGoalSuccess = createAction(
  '[Goals] Delete Goal Success',
  props<{ goalId: number }>()
);

export const deleteGoalFailure = createAction(
  '[Goals] Delete Goal Failure',
  props<{ error: string }>()
);

// Update goal progress (usually triggered by activity completion)
export const updateGoalProgress = createAction(
  '[Goals] Update Goal Progress',
  props<{ goalId: number; progressValue: number; activityId?: number }>()
);

export const updateGoalProgressSuccess = createAction(
  '[Goals] Update Goal Progress Success',
  props<{ goal: UserGoal }>()
);

export const updateGoalProgressFailure = createAction(
  '[Goals] Update Goal Progress Failure',
  props<{ error: string }>()
);

// Complete goal manually
export const completeGoal = createAction(
  '[Goals] Complete Goal',
  props<{ goalId: number }>()
);

export const completeGoalSuccess = createAction(
  '[Goals] Complete Goal Success',
  props<{ goal: UserGoal; experiencePointsEarned: number }>()
);

export const completeGoalFailure = createAction(
  '[Goals] Complete Goal Failure',
  props<{ error: string }>()
);

// Archive/abandon goal
export const abandonGoal = createAction(
  '[Goals] Abandon Goal',
  props<{ goalId: number; reason?: string }>()
);

export const abandonGoalSuccess = createAction(
  '[Goals] Abandon Goal Success',
  props<{ goalId: number }>()
);

export const abandonGoalFailure = createAction(
  '[Goals] Abandon Goal Failure',
  props<{ error: string }>()
);

// Goal suggestions and recommendations
export const loadGoalSuggestions = createAction(
  '[Goals] Load Goal Suggestions',
  props<{ basedOnActivity?: boolean; userLevel?: number }>()
);

export const loadGoalSuggestionsSuccess = createAction(
  '[Goals] Load Goal Suggestions Success',
  props<{ suggestions: Partial<CreateGoalRequest>[] }>()
);

export const loadGoalSuggestionsFailure = createAction(
  '[Goals] Load Goal Suggestions Failure',
  props<{ error: string }>()
);

// Adaptive goal adjustments (AI-driven goal modifications)
export const requestGoalAdjustment = createAction(
  '[Goals] Request Goal Adjustment',
  props<{ goalId: number; reason: 'too_easy' | 'too_hard' | 'auto_adaptive' }>()
);

export const requestGoalAdjustmentSuccess = createAction(
  '[Goals] Request Goal Adjustment Success',
  props<{ goal: UserGoal; adjustmentReason: string }>()
);

export const requestGoalAdjustmentFailure = createAction(
  '[Goals] Request Goal Adjustment Failure',
  props<{ error: string }>()
);

// Goal templates and quick setup
export const loadGoalTemplates = createAction('[Goals] Load Goal Templates');

export const loadGoalTemplatesSuccess = createAction(
  '[Goals] Load Goal Templates Success',
  props<{ templates: Array<Omit<CreateGoalRequest, 'startDate' | 'endDate'> & { 
    id: number; 
    name: string; 
    description: string; 
    category: string;
    difficulty: 'beginner' | 'intermediate' | 'advanced';
    estimatedDuration: number; // in days
  }> }>()
);

export const loadGoalTemplatesFailure = createAction(
  '[Goals] Load Goal Templates Failure',
  props<{ error: string }>()
);

export const createGoalFromTemplate = createAction(
  '[Goals] Create Goal From Template',
  props<{ templateId: number; customizations?: Partial<CreateGoalRequest> }>()
);

// Goal analytics and insights
export const loadGoalAnalytics = createAction(
  '[Goals] Load Goal Analytics',
  props<{ period?: 'week' | 'month' | 'quarter' | 'year' }>()
);

export const loadGoalAnalyticsSuccess = createAction(
  '[Goals] Load Goal Analytics Success',
  props<{ analytics: {
    totalGoals: number;
    completedGoals: number;
    activeGoals: number;
    abandonedGoals: number;
    averageCompletionTime: number; // in days
    completionRate: number; // percentage
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
  } }>()
);

export const loadGoalAnalyticsFailure = createAction(
  '[Goals] Load Goal Analytics Failure',
  props<{ error: string }>()
);

// Goal milestones and celebrations
export const loadGoalMilestones = createAction(
  '[Goals] Load Goal Milestones',
  props<{ goalId: number }>()
);

export const loadGoalMilestonesSuccess = createAction(
  '[Goals] Load Goal Milestones Success',
  props<{ goalId: number; milestones: Array<{
    id: number;
    goalId: number;
    milestone: number; // percentage (25, 50, 75, 100)
    achievedAt?: Date;
    message: string;
    reward?: {
      type: 'badge' | 'xp' | 'virtual_item';
      value: string | number;
    };
  }> }>()
);

export const loadGoalMilestonesFailure = createAction(
  '[Goals] Load Goal Milestones Failure',
  props<{ error: string }>()
);

export const achieveMilestone = createAction(
  '[Goals] Achieve Milestone',
  props<{ goalId: number; milestoneId: number }>()
);

export const achieveMilestoneSuccess = createAction(
  '[Goals] Achieve Milestone Success',
  props<{ goalId: number; milestoneId: number; reward?: any }>()
);

// Real-time goal progress tracking
export const trackGoalProgress = createAction(
  '[Goals] Track Goal Progress',
  props<{ activities: Array<{ activityId: number; value: number; unit: string }> }>()
);

export const trackGoalProgressSuccess = createAction(
  '[Goals] Track Goal Progress Success',
  props<{ updatedGoals: UserGoal[] }>()
);

export const trackGoalProgressFailure = createAction(
  '[Goals] Track Goal Progress Failure',
  props<{ error: string }>()
);

// Goal sharing and social features
export const shareGoal = createAction(
  '[Goals] Share Goal',
  props<{ goalId: number; platform: 'feed' | 'friends' | 'external'; message?: string }>()
);

export const shareGoalSuccess = createAction(
  '[Goals] Share Goal Success',
  props<{ goalId: number }>()
);

export const shareGoalFailure = createAction(
  '[Goals] Share Goal Failure',
  props<{ error: string }>()
);

export const loadSharedGoals = createAction(
  '[Goals] Load Shared Goals',
  props<{ friendsOnly?: boolean; page?: number }>()
);

export const loadSharedGoalsSuccess = createAction(
  '[Goals] Load Shared Goals Success',
  props<{ sharedGoals: Array<UserGoal & { 
    sharedBy: { id: number; firstName: string; lastName: string; avatarUrl?: string };
    sharedAt: Date;
    likes: number;
    comments: number;
  }> }>()
);

export const loadSharedGoalsFailure = createAction(
  '[Goals] Load Shared Goals Failure',
  props<{ error: string }>()
);

// UI State management
export const setSelectedGoal = createAction(
  '[Goals] Set Selected Goal',
  props<{ goal: UserGoal | null }>()
);

export const setGoalFilters = createAction(
  '[Goals] Set Goal Filters',
  props<{ filters: {
    status?: GoalStatus[];
    type?: GoalType[];
    frequency?: GoalFrequency[];
    dateRange?: { start: Date; end: Date };
  } }>()
);

export const clearGoalError = createAction('[Goals] Clear Error');

export const refreshGoals = createAction('[Goals] Refresh Goals');

// Goal reminders and notifications
export const scheduleGoalReminder = createAction(
  '[Goals] Schedule Goal Reminder',
  props<{ goalId: number; reminderType: 'daily' | 'weekly' | 'custom'; time?: string }>()
);

export const scheduleGoalReminderSuccess = createAction(
  '[Goals] Schedule Goal Reminder Success',
  props<{ goalId: number; reminderId: number }>()
);

export const scheduleGoalReminderFailure = createAction(
  '[Goals] Schedule Goal Reminder Failure',
  props<{ error: string }>()
);

export const cancelGoalReminder = createAction(
  '[Goals] Cancel Goal Reminder',
  props<{ goalId: number; reminderId: number }>()
);

export const cancelGoalReminderSuccess = createAction(
  '[Goals] Cancel Goal Reminder Success',
  props<{ goalId: number; reminderId: number }>()
);
