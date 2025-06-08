import { createReducer, on } from '@ngrx/store';
import { 
  UserGoal, 
  GoalType, 
  GoalFrequency, 
  GoalStatus,
  CreateGoalRequest 
} from '../../shared/models/user.model';
import * as GoalsActions from './goals.actions';

/**
 * Goals State Interface - the comprehensive goal tracking system for fitness achievements.
 * Think of this as a digital goal planner that tracks personal targets:
 * - Active fitness goals and their progress
 * - Goal templates and suggestions
 * - Achievement analytics and milestones
 * - Social goal sharing and inspiration
 */
export interface GoalsState {
  // User's goals
  goals: UserGoal[];
  goalsLoaded: boolean;
  
  // Currently selected/viewed goal
  selectedGoal: UserGoal | null;
  
  // Goal templates and suggestions
  goalTemplates: Array<Omit<CreateGoalRequest, 'startDate' | 'endDate'> & { 
    id: number; 
    name: string; 
    description: string; 
    category: string;
    difficulty: 'beginner' | 'intermediate' | 'advanced';
    estimatedDuration: number;
  }>;
  goalTemplatesLoaded: boolean;
  
  goalSuggestions: Partial<CreateGoalRequest>[];
  suggestionsLoaded: boolean;
  
  // Analytics and insights
  goalAnalytics: {
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
  } | null;
  analyticsLoaded: boolean;
  
  // Goal milestones
  goalMilestones: { [goalId: number]: Array<{
    id: number;
    goalId: number;
    milestone: number;
    achievedAt?: Date;
    message: string;
    reward?: {
      type: 'badge' | 'xp' | 'virtual_item';
      value: string | number;
    };
  }> };
  
  // Social goals
  sharedGoals: Array<UserGoal & { 
    sharedBy: { id: number; firstName: string; lastName: string; avatarUrl?: string };
    sharedAt: Date;
    likes: number;
    comments: number;
  }>;
  sharedGoalsLoaded: boolean;
  
  // Loading states
  isLoading: boolean;
  isCreating: boolean;
  isUpdating: boolean;
  isDeleting: boolean;
  isLoadingAnalytics: boolean;
  isLoadingSuggestions: boolean;
  isUpdatingProgress: boolean;
  
  // UI State
  filters: {
    status?: GoalStatus[];
    type?: GoalType[];
    frequency?: GoalFrequency[];
    dateRange?: { start: Date; end: Date };
  };
  
  // Error handling
  error: string | null;
  
  // Goal reminders
  goalReminders: { [goalId: number]: Array<{
    id: number;
    type: 'daily' | 'weekly' | 'custom';
    time?: string;
    isActive: boolean;
  }> };
}

/**
 * Initial state - a fresh goal tracking system ready to help achieve fitness dreams.
 */
export const initialState: GoalsState = {
  goals: [],
  goalsLoaded: false,
  
  selectedGoal: null,
  
  goalTemplates: [],
  goalTemplatesLoaded: false,
  
  goalSuggestions: [],
  suggestionsLoaded: false,
  
  goalAnalytics: null,
  analyticsLoaded: false,
  
  goalMilestones: {},
  
  sharedGoals: [],
  sharedGoalsLoaded: false,
  
  isLoading: false,
  isCreating: false,
  isUpdating: false,
  isDeleting: false,
  isLoadingAnalytics: false,
  isLoadingSuggestions: false,
  isUpdatingProgress: false,
  
  filters: {},
  
  error: null,
  
  goalReminders: {}
};

/**
 * Goals Reducer - the goal coach who manages all target setting and achievement tracking.
 * Each action is like a different coaching instruction: "set a new goal", 
 * "track progress", "celebrate achievement", "adjust target", etc.
 */
export const goalsReducer = createReducer(
  initialState,

  // Load goals
  on(GoalsActions.loadGoals, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GoalsActions.loadGoalsSuccess, (state, { goals }) => ({
    ...state,
    goals,
    goalsLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GoalsActions.loadGoalsFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // Load specific goal
  on(GoalsActions.loadGoalById, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GoalsActions.loadGoalByIdSuccess, (state, { goal }) => ({
    ...state,
    selectedGoal: goal,
    // Also update the goal in the goals array if it exists
    goals: state.goals.map(g => g.id === goal.id ? goal : g),
    isLoading: false,
    error: null
  })),

  on(GoalsActions.loadGoalByIdFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // Create goal
  on(GoalsActions.createGoal, (state) => ({
    ...state,
    isCreating: true,
    error: null
  })),

  on(GoalsActions.createGoalSuccess, (state, { goal }) => ({
    ...state,
    goals: [goal, ...state.goals],
    selectedGoal: goal,
    isCreating: false,
    error: null
  })),

  on(GoalsActions.createGoalFailure, (state, { error }) => ({
    ...state,
    isCreating: false,
    error
  })),

  // Update goal
  on(GoalsActions.updateGoal, (state) => ({
    ...state,
    isUpdating: true,
    error: null
  })),

  on(GoalsActions.updateGoalSuccess, (state, { goal }) => ({
    ...state,
    goals: state.goals.map(g => g.id === goal.id ? goal : g),
    selectedGoal: state.selectedGoal?.id === goal.id ? goal : state.selectedGoal,
    isUpdating: false,
    error: null
  })),

  on(GoalsActions.updateGoalFailure, (state, { error }) => ({
    ...state,
    isUpdating: false,
    error
  })),

  // Delete goal
  on(GoalsActions.deleteGoal, (state) => ({
    ...state,
    isDeleting: true,
    error: null
  })),

  on(GoalsActions.deleteGoalSuccess, (state, { goalId }) => ({
    ...state,
    goals: state.goals.filter(g => g.id !== goalId),
    selectedGoal: state.selectedGoal?.id === goalId ? null : state.selectedGoal,
    isDeleting: false,
    error: null
  })),

  on(GoalsActions.deleteGoalFailure, (state, { error }) => ({
    ...state,
    isDeleting: false,
    error
  })),

  // Update goal progress
  on(GoalsActions.updateGoalProgress, (state) => ({
    ...state,
    isUpdatingProgress: true,
    error: null
  })),

  on(GoalsActions.updateGoalProgressSuccess, (state, { goal }) => ({
    ...state,
    goals: state.goals.map(g => g.id === goal.id ? goal : g),
    selectedGoal: state.selectedGoal?.id === goal.id ? goal : state.selectedGoal,
    isUpdatingProgress: false,
    error: null
  })),

  on(GoalsActions.updateGoalProgressFailure, (state, { error }) => ({
    ...state,
    isUpdatingProgress: false,
    error
  })),

  // Complete goal
  on(GoalsActions.completeGoal, (state) => ({
    ...state,
    isUpdating: true,
    error: null
  })),

  on(GoalsActions.completeGoalSuccess, (state, { goal }) => ({
    ...state,
    goals: state.goals.map(g => g.id === goal.id ? goal : g),
    selectedGoal: state.selectedGoal?.id === goal.id ? goal : state.selectedGoal,
    isUpdating: false,
    error: null
  })),

  on(GoalsActions.completeGoalFailure, (state, { error }) => ({
    ...state,
    isUpdating: false,
    error
  })),

  // Abandon goal
  on(GoalsActions.abandonGoal, (state) => ({
    ...state,
    isUpdating: true,
    error: null
  })),

  on(GoalsActions.abandonGoalSuccess, (state, { goalId }) => ({
    ...state,
    goals: state.goals.map(g => 
      g.id === goalId ? { ...g, status: GoalStatus.Abandoned } : g
    ),
    selectedGoal: state.selectedGoal?.id === goalId 
      ? { ...state.selectedGoal, status: GoalStatus.Abandoned } 
      : state.selectedGoal,
    isUpdating: false,
    error: null
  })),

  on(GoalsActions.abandonGoalFailure, (state, { error }) => ({
    ...state,
    isUpdating: false,
    error
  })),

  // Goal suggestions
  on(GoalsActions.loadGoalSuggestions, (state) => ({
    ...state,
    isLoadingSuggestions: true,
    error: null
  })),

  on(GoalsActions.loadGoalSuggestionsSuccess, (state, { suggestions }) => ({
    ...state,
    goalSuggestions: suggestions,
    suggestionsLoaded: true,
    isLoadingSuggestions: false,
    error: null
  })),

  on(GoalsActions.loadGoalSuggestionsFailure, (state, { error }) => ({
    ...state,
    isLoadingSuggestions: false,
    error
  })),

  // Goal adjustment
  on(GoalsActions.requestGoalAdjustment, (state) => ({
    ...state,
    isUpdating: true,
    error: null
  })),

  on(GoalsActions.requestGoalAdjustmentSuccess, (state, { goal }) => ({
    ...state,
    goals: state.goals.map(g => g.id === goal.id ? goal : g),
    selectedGoal: state.selectedGoal?.id === goal.id ? goal : state.selectedGoal,
    isUpdating: false,
    error: null
  })),

  on(GoalsActions.requestGoalAdjustmentFailure, (state, { error }) => ({
    ...state,
    isUpdating: false,
    error
  })),

  // Goal templates
  on(GoalsActions.loadGoalTemplates, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GoalsActions.loadGoalTemplatesSuccess, (state, { templates }) => ({
    ...state,
    goalTemplates: templates,
    goalTemplatesLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GoalsActions.loadGoalTemplatesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GoalsActions.createGoalFromTemplate, (state) => ({
    ...state,
    isCreating: true,
    error: null
  })),

  // Goal analytics
  on(GoalsActions.loadGoalAnalytics, (state) => ({
    ...state,
    isLoadingAnalytics: true,
    error: null
  })),

  on(GoalsActions.loadGoalAnalyticsSuccess, (state, { analytics }) => ({
    ...state,
    goalAnalytics: analytics,
    analyticsLoaded: true,
    isLoadingAnalytics: false,
    error: null
  })),

  on(GoalsActions.loadGoalAnalyticsFailure, (state, { error }) => ({
    ...state,
    isLoadingAnalytics: false,
    error
  })),

  // Goal milestones
  on(GoalsActions.loadGoalMilestones, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GoalsActions.loadGoalMilestonesSuccess, (state, { goalId, milestones }) => ({
    ...state,
    goalMilestones: {
      ...state.goalMilestones,
      [goalId]: milestones
    },
    isLoading: false,
    error: null
  })),

  on(GoalsActions.loadGoalMilestonesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GoalsActions.achieveMilestoneSuccess, (state, { goalId, milestoneId }) => ({
    ...state,
    goalMilestones: {
      ...state.goalMilestones,
      [goalId]: state.goalMilestones[goalId]?.map(m => 
        m.id === milestoneId ? { ...m, achievedAt: new Date() } : m
      ) || []
    }
  })),

  // Track goal progress (bulk update)
  on(GoalsActions.trackGoalProgress, (state) => ({
    ...state,
    isUpdatingProgress: true,
    error: null
  })),

  on(GoalsActions.trackGoalProgressSuccess, (state, { updatedGoals }) => {
    const goalsMap = new Map(updatedGoals.map(goal => [goal.id, goal]));
    
    return {
      ...state,
      goals: state.goals.map(g => goalsMap.get(g.id) || g),
      isUpdatingProgress: false,
      error: null
    };
  }),

  on(GoalsActions.trackGoalProgressFailure, (state, { error }) => ({
    ...state,
    isUpdatingProgress: false,
    error
  })),

  // Shared goals
  on(GoalsActions.loadSharedGoals, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(GoalsActions.loadSharedGoalsSuccess, (state, { sharedGoals }) => ({
    ...state,
    sharedGoals,
    sharedGoalsLoaded: true,
    isLoading: false,
    error: null
  })),

  on(GoalsActions.loadSharedGoalsFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(GoalsActions.shareGoalSuccess, (state, { goalId }) => ({
    ...state,
    goals: state.goals.map(g => 
      g.id === goalId ? { ...g, /* add shared flag if needed */ } : g
    ),
    error: null
  })),

  // UI state management
  on(GoalsActions.setSelectedGoal, (state, { goal }) => ({
    ...state,
    selectedGoal: goal
  })),

  on(GoalsActions.setGoalFilters, (state, { filters }) => ({
    ...state,
    filters: { ...state.filters, ...filters }
  })),

  on(GoalsActions.clearGoalError, (state) => ({
    ...state,
    error: null
  })),

  // Refresh goals
  on(GoalsActions.refreshGoals, (state) => ({
    ...state,
    goalsLoaded: false,
    analyticsLoaded: false,
    suggestionsLoaded: false,
    isLoading: true
  })),

  // Goal reminders
  on(GoalsActions.scheduleGoalReminderSuccess, (state, { goalId, reminderId }) => ({
    ...state,
    goalReminders: {
      ...state.goalReminders,
      [goalId]: [
        ...(state.goalReminders[goalId] || []),
        {
          id: reminderId,
          type: 'daily', // This would come from the action payload in real implementation
          isActive: true
        }
      ]
    }
  })),

  on(GoalsActions.cancelGoalReminderSuccess, (state, { goalId, reminderId }) => ({
    ...state,
    goalReminders: {
      ...state.goalReminders,
      [goalId]: state.goalReminders[goalId]?.filter(r => r.id !== reminderId) || []
    }
  }))
);
