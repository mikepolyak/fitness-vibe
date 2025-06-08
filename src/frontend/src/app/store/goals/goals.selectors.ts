import { createFeatureSelector, createSelector } from '@ngrx/store';
import { GoalsState } from './goals.reducer';
import { GoalStatus, GoalType, GoalFrequency, UserUtils } from '../../shared/models/user.model';

/**
 * Goals Selectors - the specialized goal coaches who can quickly analyze progress,
 * identify patterns, and provide insights about fitness goal achievement.
 * Think of them as expert goal consultants who understand the science of motivation.
 */

// Feature selector - gets the entire goals section of the store
export const selectGoalsState = createFeatureSelector<GoalsState>('goals');

// Basic state selectors
export const selectGoals = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.goals
);

export const selectGoalsLoaded = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.goalsLoaded
);

export const selectSelectedGoal = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.selectedGoal
);

export const selectGoalTemplates = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.goalTemplates
);

export const selectGoalTemplatesLoaded = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.goalTemplatesLoaded
);

export const selectGoalSuggestions = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.goalSuggestions
);

export const selectSuggestionsLoaded = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.suggestionsLoaded
);

export const selectGoalAnalytics = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.goalAnalytics
);

export const selectAnalyticsLoaded = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.analyticsLoaded
);

export const selectGoalMilestones = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.goalMilestones
);

export const selectSharedGoals = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.sharedGoals
);

export const selectSharedGoalsLoaded = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.sharedGoalsLoaded
);

export const selectGoalReminders = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.goalReminders
);

// Loading state selectors
export const selectGoalsError = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.error
);

export const selectIsLoading = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.isLoading
);

export const selectIsCreating = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.isCreating
);

export const selectIsUpdating = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.isUpdating
);

export const selectIsDeleting = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.isDeleting
);

export const selectIsLoadingAnalytics = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.isLoadingAnalytics
);

export const selectIsLoadingSuggestions = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.isLoadingSuggestions
);

export const selectIsUpdatingProgress = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.isUpdatingProgress
);

export const selectGoalFilters = createSelector(
  selectGoalsState,
  (state: GoalsState) => state.filters
);

// Computed selectors for goal categories and analysis
export const selectActiveGoals = createSelector(
  selectGoals,
  (goals) => goals.filter(goal => goal.status === GoalStatus.Active)
);

export const selectCompletedGoals = createSelector(
  selectGoals,
  (goals) => goals.filter(goal => goal.status === GoalStatus.Completed)
);

export const selectExpiredGoals = createSelector(
  selectGoals,
  (goals) => goals.filter(goal => goal.status === GoalStatus.Expired)
);

export const selectAbandonedGoals = createSelector(
  selectGoals,
  (goals) => goals.filter(goal => goal.status === GoalStatus.Abandoned)
);

export const selectOverdueGoals = createSelector(
  selectActiveGoals,
  (activeGoals) => {
    const now = new Date();
    return activeGoals.filter(goal => new Date(goal.endDate) < now);
  }
);

export const selectGoalsDueSoon = createSelector(
  selectActiveGoals,
  (activeGoals) => {
    const now = new Date();
    const threeDaysFromNow = new Date(now.getTime() + 3 * 24 * 60 * 60 * 1000);
    
    return activeGoals.filter(goal => {
      const endDate = new Date(goal.endDate);
      return endDate >= now && endDate <= threeDaysFromNow;
    });
  }
);

// Goal progress analysis
export const selectGoalsWithProgress = createSelector(
  selectGoals,
  (goals) => goals.map(goal => ({
    ...goal,
    progressPercentage: UserUtils.getGoalProgressPercentage(goal),
    isOverdue: UserUtils.isGoalOverdue(goal),
    timeRemainingInDays: UserUtils.getTimeRemainingInDays(goal)
  }))
);

export const selectGoalsByProgress = createSelector(
  selectGoalsWithProgress,
  (goalsWithProgress) => {
    const categories = {
      notStarted: goalsWithProgress.filter(g => g.progressPercentage === 0),
      inProgress: goalsWithProgress.filter(g => g.progressPercentage > 0 && g.progressPercentage < 100),
      nearCompletion: goalsWithProgress.filter(g => g.progressPercentage >= 75 && g.progressPercentage < 100),
      completed: goalsWithProgress.filter(g => g.progressPercentage >= 100)
    };
    
    return categories;
  }
);

export const selectGoalsByType = createSelector(
  selectGoals,
  (goals) => {
    const goalsByType: { [key in GoalType]?: typeof goals } = {};
    
    goals.forEach(goal => {
      if (!goalsByType[goal.type]) {
        goalsByType[goal.type] = [];
      }
      goalsByType[goal.type]!.push(goal);
    });
    
    return goalsByType;
  }
);

export const selectGoalsByFrequency = createSelector(
  selectGoals,
  (goals) => {
    const goalsByFrequency: { [key in GoalFrequency]?: typeof goals } = {};
    
    goals.forEach(goal => {
      if (!goalsByFrequency[goal.frequency]) {
        goalsByFrequency[goal.frequency] = [];
      }
      goalsByFrequency[goal.frequency]!.push(goal);
    });
    
    return goalsByFrequency;
  }
);

// Goal performance metrics
export const selectGoalCompletionRate = createSelector(
  selectGoals,
  (goals) => {
    if (goals.length === 0) return 0;
    
    const completedGoals = goals.filter(g => g.status === GoalStatus.Completed);
    return (completedGoals.length / goals.length) * 100;
  }
);

export const selectAverageGoalDuration = createSelector(
  selectCompletedGoals,
  (completedGoals) => {
    if (completedGoals.length === 0) return 0;
    
    const totalDuration = completedGoals.reduce((sum, goal) => {
      const startDate = new Date(goal.startDate);
      const endDate = new Date(goal.endDate);
      return sum + (endDate.getTime() - startDate.getTime());
    }, 0);
    
    return totalDuration / (completedGoals.length * 24 * 60 * 60 * 1000); // Convert to days
  }
);

export const selectGoalStreaks = createSelector(
  selectCompletedGoals,
  (completedGoals) => {
    if (completedGoals.length === 0) return { currentStreak: 0, longestStreak: 0 };
    
    // Sort by completion date
    const sortedGoals = [...completedGoals].sort(
      (a, b) => new Date(b.endDate).getTime() - new Date(a.endDate).getTime()
    );
    
    // Calculate current streak (goals completed in the last 30 days)
    const thirtyDaysAgo = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000);
    let currentStreak = 0;
    
    for (const goal of sortedGoals) {
      if (new Date(goal.endDate) >= thirtyDaysAgo) {
        currentStreak++;
      } else {
        break;
      }
    }
    
    // Calculate longest streak (consecutive months with completed goals)
    const goalsByMonth = new Map<string, number>();
    sortedGoals.forEach(goal => {
      const monthKey = new Date(goal.endDate).toISOString().substring(0, 7); // YYYY-MM
      goalsByMonth.set(monthKey, (goalsByMonth.get(monthKey) || 0) + 1);
    });
    
    const months = Array.from(goalsByMonth.keys()).sort().reverse();
    let longestStreak = 0;
    let tempStreak = 0;
    
    for (let i = 0; i < months.length; i++) {
      if (i === 0 || isConsecutiveMonth(months[i], months[i - 1])) {
        tempStreak++;
      } else {
        longestStreak = Math.max(longestStreak, tempStreak);
        tempStreak = 1;
      }
    }
    longestStreak = Math.max(longestStreak, tempStreak);
    
    return { currentStreak, longestStreak };
  }
);

// Helper function for streak calculation
function isConsecutiveMonth(current: string, previous: string): boolean {
  const currentDate = new Date(current + '-01');
  const previousDate = new Date(previous + '-01');
  
  const diffInMonths = (currentDate.getFullYear() - previousDate.getFullYear()) * 12 + 
                      (currentDate.getMonth() - previousDate.getMonth());
  
  return diffInMonths === -1; // Previous month
}

// Goal filtering
export const selectFilteredGoals = createSelector(
  selectGoalsWithProgress,
  selectGoalFilters,
  (goalsWithProgress, filters) => {
    let filtered = [...goalsWithProgress];
    
    if (filters.status && filters.status.length > 0) {
      filtered = filtered.filter(goal => filters.status!.includes(goal.status));
    }
    
    if (filters.type && filters.type.length > 0) {
      filtered = filtered.filter(goal => filters.type!.includes(goal.type));
    }
    
    if (filters.frequency && filters.frequency.length > 0) {
      filtered = filtered.filter(goal => filters.frequency!.includes(goal.frequency));
    }
    
    if (filters.dateRange) {
      filtered = filtered.filter(goal => {
        const goalStart = new Date(goal.startDate);
        const goalEnd = new Date(goal.endDate);
        return (goalStart >= filters.dateRange!.start && goalStart <= filters.dateRange!.end) ||
               (goalEnd >= filters.dateRange!.start && goalEnd <= filters.dateRange!.end) ||
               (goalStart <= filters.dateRange!.start && goalEnd >= filters.dateRange!.end);
      });
    }
    
    return filtered;
  }
);

// Goal templates by difficulty
export const selectGoalTemplatesByDifficulty = createSelector(
  selectGoalTemplates,
  (templates) => {
    const templatesByDifficulty = {
      beginner: templates.filter(t => t.difficulty === 'beginner'),
      intermediate: templates.filter(t => t.difficulty === 'intermediate'),
      advanced: templates.filter(t => t.difficulty === 'advanced')
    };
    
    return templatesByDifficulty;
  }
);

export const selectGoalTemplatesByCategory = createSelector(
  selectGoalTemplates,
  (templates) => {
    const templatesByCategory: { [category: string]: typeof templates } = {};
    
    templates.forEach(template => {
      if (!templatesByCategory[template.category]) {
        templatesByCategory[template.category] = [];
      }
      templatesByCategory[template.category].push(template);
    });
    
    return templatesByCategory;
  }
);

// Milestone tracking
export const selectGoalMilestonesById = (goalId: number) => createSelector(
  selectGoalMilestones,
  (milestones) => milestones[goalId] || []
);

export const selectAchievedMilestones = createSelector(
  selectGoalMilestones,
  (milestonesMap) => {
    const achieved: any[] = [];
    
    Object.values(milestonesMap).forEach(milestones => {
      achieved.push(...milestones.filter(m => m.achievedAt));
    });
    
    return achieved.sort((a, b) => 
      new Date(b.achievedAt!).getTime() - new Date(a.achievedAt!).getTime()
    );
  }
);

export const selectRecentAchievedMilestones = createSelector(
  selectAchievedMilestones,
  (achieved) => {
    const sevenDaysAgo = new Date(Date.now() - 7 * 24 * 60 * 60 * 1000);
    return achieved.filter(m => new Date(m.achievedAt!) >= sevenDaysAgo);
  }
);

// Priority and urgency analysis
export const selectHighPriorityGoals = createSelector(
  selectActiveGoals,
  (activeGoals) => {
    const now = new Date();
    
    return activeGoals
      .map(goal => {
        const timeRemaining = UserUtils.getTimeRemainingInDays(goal);
        const progress = UserUtils.getGoalProgressPercentage(goal);
        
        // Calculate priority score based on urgency and progress
        let priorityScore = 0;
        
        // Urgency factor (higher score for less time remaining)
        if (timeRemaining <= 1) priorityScore += 10;
        else if (timeRemaining <= 3) priorityScore += 8;
        else if (timeRemaining <= 7) priorityScore += 6;
        else if (timeRemaining <= 14) priorityScore += 4;
        else if (timeRemaining <= 30) priorityScore += 2;
        
        // Progress factor (higher score for goals with some progress but not near completion)
        if (progress > 0 && progress < 25) priorityScore += 3;
        else if (progress >= 25 && progress < 50) priorityScore += 5;
        else if (progress >= 50 && progress < 75) priorityScore += 4;
        else if (progress >= 75 && progress < 90) priorityScore += 6;
        else if (progress >= 90) priorityScore += 8;
        
        // Goal type factor (some goals might be more important)
        if (goal.type === GoalType.LoseWeight || goal.type === GoalType.BuildMuscle) {
          priorityScore += 2;
        }
        
        return { ...goal, priorityScore, timeRemaining, progress };
      })
      .filter(goal => goal.priorityScore >= 5)
      .sort((a, b) => b.priorityScore - a.priorityScore)
      .slice(0, 5); // Top 5 high priority goals
  }
);

// Social goals analysis
export const selectTrendingSharedGoals = createSelector(
  selectSharedGoals,
  (sharedGoals) => {
    return [...sharedGoals]
      .sort((a, b) => (b.likes + b.comments) - (a.likes + a.comments))
      .slice(0, 10);
  }
);

export const selectRecentSharedGoals = createSelector(
  selectSharedGoals,
  (sharedGoals) => {
    const threeDaysAgo = new Date(Date.now() - 3 * 24 * 60 * 60 * 1000);
    
    return sharedGoals
      .filter(goal => new Date(goal.sharedAt) >= threeDaysAgo)
      .sort((a, b) => new Date(b.sharedAt).getTime() - new Date(a.sharedAt).getTime());
  }
);

// Error and loading aggregation
export const selectHasGoalsError = createSelector(
  selectGoalsError,
  (error) => !!error
);

export const selectIsAnyGoalsLoading = createSelector(
  selectIsLoading,
  selectIsCreating,
  selectIsUpdating,
  selectIsDeleting,
  selectIsLoadingAnalytics,
  selectIsLoadingSuggestions,
  selectIsUpdatingProgress,
  (isLoading, isCreating, isUpdating, isDeleting, isLoadingAnalytics, isLoadingSuggestions, isUpdatingProgress) =>
    isLoading || isCreating || isUpdating || isDeleting || isLoadingAnalytics || isLoadingSuggestions || isUpdatingProgress
);

// Dashboard summary selectors
export const selectGoalsDashboardSummary = createSelector(
  selectActiveGoals,
  selectCompletedGoals,
  selectGoalsDueSoon,
  selectOverdueGoals,
  selectGoalCompletionRate,
  (activeGoals, completedGoals, dueSoon, overdue, completionRate) => ({
    totalActive: activeGoals.length,
    totalCompleted: completedGoals.length,
    dueSoon: dueSoon.length,
    overdue: overdue.length,
    completionRate: Math.round(completionRate),
    needsAttention: overdue.length + dueSoon.length
  })
);

// Goal progress insights
export const selectProgressInsights = createSelector(
  selectGoalsWithProgress,
  selectGoalAnalytics,
  (goalsWithProgress, analytics) => {
    const insights: string[] = [];
    
    // Analyze progress patterns
    const inProgressGoals = goalsWithProgress.filter(g => 
      g.progressPercentage > 0 && g.progressPercentage < 100 && g.status === GoalStatus.Active
    );
    
    if (inProgressGoals.length > 0) {
      const avgProgress = inProgressGoals.reduce((sum, g) => sum + g.progressPercentage, 0) / inProgressGoals.length;
      
      if (avgProgress >= 75) {
        insights.push("You're crushing your goals! Most are near completion. 🎯");
      } else if (avgProgress >= 50) {
        insights.push("Great momentum! You're halfway to achieving most of your goals. 💪");
      } else if (avgProgress < 25) {
        insights.push("Time to accelerate! Focus on 2-3 goals to build momentum. 🚀");
      }
    }
    
    // Check for overdue goals
    const overdueCount = goalsWithProgress.filter(g => g.isOverdue).length;
    if (overdueCount > 0) {
      insights.push(`${overdueCount} goal${overdueCount > 1 ? 's are' : ' is'} overdue. Consider adjusting targets or timelines. ⏰`);
    }
    
    // Success pattern insights
    if (analytics && analytics.completionRate >= 80) {
      insights.push("Excellent goal achievement rate! You're a goal-setting superstar! ⭐");
    } else if (analytics && analytics.completionRate >= 60) {
      insights.push("Good goal completion rate. Consider reviewing what makes your successful goals work. 📈");
    }
    
    return insights;
  }
);
