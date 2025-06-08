import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ActivitiesState } from './activities.reducer';
import { ActivityCategory, ActivityType, ChallengeStatus } from '../../shared/models/activity.model';

/**
 * Activities Selectors - the specialized trainers who can quickly find specific information
 * about workouts and fitness activities. Think of them as expert fitness consultants
 * who can instantly tell you about your performance trends, achievements, etc.
 */

// Feature selector - gets the entire activities section of the store
export const selectActivitiesState = createFeatureSelector<ActivitiesState>('activities');

// Basic state selectors
export const selectActivities = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.activities
);

export const selectActivitiesLoaded = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.activitiesLoaded
);

export const selectUserActivities = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.userActivities
);

export const selectUserActivitiesLoaded = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.userActivitiesLoaded
);

export const selectUserActivitiesTotalCount = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.userActivitiesTotalCount
);

export const selectUserActivitiesHasMore = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.userActivitiesHasMore
);

export const selectSelectedActivity = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.selectedActivity
);

export const selectActivityTemplates = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.templates
);

export const selectTemplatesLoaded = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.templatesLoaded
);

export const selectActivityStats = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.activityStats
);

export const selectPersonalBests = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.personalBests
);

export const selectAvailableChallenges = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.availableChallenges
);

export const selectUserChallenges = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.userChallenges
);

export const selectChallengesLoaded = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.challengesLoaded
);

export const selectChallengesHasMore = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.challengesHasMore
);

// Live activity selectors
export const selectLiveActivity = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.liveActivity
);

export const selectIsLiveActivityActive = createSelector(
  selectLiveActivity,
  (liveActivity) => liveActivity.isActive
);

export const selectIsLiveActivityPaused = createSelector(
  selectLiveActivity,
  (liveActivity) => liveActivity.isPaused
);

export const selectLiveActivityMetrics = createSelector(
  selectLiveActivity,
  (liveActivity) => liveActivity.liveMetrics
);

export const selectLiveActivityDuration = createSelector(
  selectLiveActivity,
  (liveActivity) => {
    if (!liveActivity.isActive || !liveActivity.startTime) return 0;
    
    const now = new Date();
    const startTime = new Date(liveActivity.startTime);
    const totalElapsed = now.getTime() - startTime.getTime();
    const netDuration = totalElapsed - liveActivity.totalPausedDuration;
    
    // If currently paused, subtract the current pause time
    if (liveActivity.isPaused && liveActivity.pausedTime) {
      const currentPauseDuration = now.getTime() - new Date(liveActivity.pausedTime).getTime();
      return Math.max(0, netDuration - currentPauseDuration);
    }
    
    return Math.max(0, netDuration);
  }
);

// Loading state selectors
export const selectActivitiesError = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.error
);

export const selectIsLoading = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.isLoading
);

export const selectIsCreating = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.isCreating
);

export const selectIsUpdating = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.isUpdating
);

export const selectIsDeleting = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.isDeleting
);

export const selectIsLoadingStats = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.isLoadingStats
);

export const selectIsJoiningChallenge = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.isJoiningChallenge
);

export const selectActivityFilters = createSelector(
  selectActivitiesState,
  (state: ActivitiesState) => state.filters
);

// Computed selectors for complex analytics
export const selectActivitiesByCategory = createSelector(
  selectUserActivities,
  (activities) => {
    const categoryCounts: { [key in ActivityCategory]?: number } = {};
    
    activities.forEach(activity => {
      const category = activity.activity.category;
      categoryCounts[category] = (categoryCounts[category] || 0) + 1;
    });
    
    return categoryCounts;
  }
);

export const selectActivitiesByType = createSelector(
  selectUserActivities,
  (activities) => {
    const typeCounts = {} as { [key in ActivityType]: number };
    Object.values(ActivityType).forEach(type => typeCounts[type] = 0);
    
    activities?.forEach(activity => {
      if (activity?.activity?.type) {
        typeCounts[activity.activity.type]++;
      }
    });
    
    return typeCounts;
  }
);

export const selectRecentActivities = createSelector(
  selectUserActivities,
  (activities) => {
    const now = new Date();
    const oneWeekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
    
    return activities
      .filter(activity => new Date(activity.completedAt) >= oneWeekAgo)
      .sort((a, b) => new Date(b.completedAt).getTime() - new Date(a.completedAt).getTime())
      .slice(0, 5);
  }
);

export const selectTopActivitiesByDuration = createSelector(
  selectUserActivities,
  (activities) => {
    return [...activities]
      .sort((a, b) => b.duration - a.duration)
      .slice(0, 10);
  }
);

export const selectTopActivitiesByDistance = createSelector(
  selectUserActivities,
  (activities) => {
    return [...activities]
      .filter(activity => activity.distance && activity.distance > 0)
      .sort((a, b) => (b.distance || 0) - (a.distance || 0))
      .slice(0, 10);
  }
);

export const selectWeeklyActivitySummary = createSelector(
  selectUserActivities,
  (activities) => {
    const now = new Date();
    const oneWeekAgo = new Date(now.getTime() - 7 * 24 * 60 * 60 * 1000);
    
    const weeklyActivities = activities.filter(
      activity => new Date(activity.completedAt) >= oneWeekAgo
    );
    
    const activitiesWithRating = weeklyActivities.filter(a => a.userRating != null);
    
    return {
      totalActivities: weeklyActivities.length,
      totalDuration: weeklyActivities.reduce((sum, activity) => sum + (activity.duration || 0), 0),
      totalDistance: weeklyActivities.reduce((sum, activity) => sum + (activity.distance ?? 0), 0),
      totalCalories: weeklyActivities.reduce((sum, activity) => sum + (activity.caloriesBurned ?? 0), 0),
      averageRating: activitiesWithRating.length > 0 
        ? activitiesWithRating.reduce((sum, a) => sum + (a.userRating ?? 0), 0) / activitiesWithRating.length
        : 0
    };
  }
);

export const selectActivityStreaks = createSelector(
  selectUserActivities,
  (activities) => {
    if (!activities?.length) return { currentStreak: 0, longestStreak: 0 };
    
    // Convert dates to ISO date strings for reliable comparison
    const dateToKey = (date: Date): string => date.toISOString().split('T')[0];
    const today = dateToKey(new Date());
    const yesterday = dateToKey(new Date(Date.now() - 24 * 60 * 60 * 1000));
    
    const activitiesByDate = new Map<string, number>();
    activities.forEach(activity => {
      if (!activity.completedAt) return;
      const dateKey = dateToKey(new Date(activity.completedAt));
      activitiesByDate.set(dateKey, (activitiesByDate.get(dateKey) || 0) + 1);
    });
    
    const dates = Array.from(activitiesByDate.keys())
      .sort((a, b) => b.localeCompare(a));
    
    if (!dates.length) return { currentStreak: 0, longestStreak: 0 };
    
    // Calculate current streak using ISO date strings
    let currentStreak = 0;
    let streakStart = dates[0] === today || dates[0] === yesterday ? 0 : -1;
    
    if (streakStart >= 0) {
      let checkDate = new Date();
      for (let i = streakStart; i < dates.length; i++) {
        const expectedKey = dateToKey(checkDate);
        if (dates[i] === expectedKey) {
          currentStreak++;
          checkDate.setDate(checkDate.getDate() - 1);
        } else {
          break;
        }
      }
    }
    
    // Calculate longest streak using ISO date strings
    let longestStreak = 0;
    let tempStreak = 1;
    
    for (let i = 1; i < dates.length; i++) {
      const dayDiff = Math.round(
        (new Date(dates[i-1]).getTime() - new Date(dates[i]).getTime()) / 
        (24 * 60 * 60 * 1000)
      );
      
      if (dayDiff === 1) {
        tempStreak++;
      } else {
        longestStreak = Math.max(longestStreak, tempStreak);
        tempStreak = 1;
      }
    }
    
    return { 
      currentStreak, 
      longestStreak: Math.max(longestStreak, tempStreak, currentStreak) 
    };
  }
);

// Challenge-related selectors
export const selectActiveChallenges = createSelector(
  selectAvailableChallenges,
  (challenges) => challenges.filter(challenge => challenge.status === ChallengeStatus.Active)
);

export const selectJoinedChallenges = createSelector(
  selectUserChallenges,
  (userChallenges) => userChallenges.filter(uc => !uc.isCompleted)
);

export const selectCompletedChallenges = createSelector(
  selectUserChallenges,
  (userChallenges) => userChallenges.filter(uc => uc.isCompleted)
);

export const selectChallengeProgress = createSelector(
  selectUserChallenges,
  (userChallenges) => {
    return userChallenges
      .filter(uc => uc?.challenge != null)
      .map(uc => ({
        challengeId: uc.challengeId,
        title: uc.challenge.title,
        progress: uc.currentProgress ?? 0,
        target: uc.challenge.targetValue,
        progressPercentage: uc.challenge.targetValue > 0 
          ? ((uc.currentProgress ?? 0) / uc.challenge.targetValue) * 100
          : 0,
        isCompleted: uc.isCompleted ?? false,
        timeRemaining: uc.challenge.endDate
          ? Math.max(0, Math.ceil(
              (new Date(uc.challenge.endDate).getTime() - Date.now()) / (1000 * 60 * 60 * 24)
            ))
          : 0
      }));
  }
);

// Template selectors
export const selectFavoriteTemplates = createSelector(
  selectActivityTemplates,
  (templates) => [...templates].sort((a, b) => b.useCount - a.useCount).slice(0, 5)
);

export const selectTemplatesByActivity = createSelector(
  selectActivityTemplates,
  (templates) => {
    if (!templates?.length) return {};
    
    const templatesByActivity: { [key: number]: typeof templates } = {};
    
    templates.forEach(template => {
      if (!template?.activityId) return;
      
      if (!templatesByActivity[template.activityId]) {
        templatesByActivity[template.activityId] = [];
      }
      templatesByActivity[template.activityId].push(template);
    });
    
    return templatesByActivity;
  }
);

// Error and loading aggregation
export const selectHasActivitiesError = createSelector(
  selectActivitiesError,
  (error) => !!error
);

export const selectIsAnyActivitiesLoading = createSelector(
  selectIsLoading,
  selectIsCreating,
  selectIsUpdating,
  selectIsDeleting,
  selectIsLoadingStats,
  selectIsJoiningChallenge,
  (isLoading, isCreating, isUpdating, isDeleting, isLoadingStats, isJoiningChallenge) =>
    isLoading || isCreating || isUpdating || isDeleting || isLoadingStats || isJoiningChallenge
);

// Activity search and filtering
export const selectFilteredActivities = createSelector(
  selectUserActivities,
  selectActivityFilters,
  (activities, filters) => {
    if (!activities?.length) return [];
    if (!filters) return activities;
    
    return activities.filter(activity => {
      if (!activity) return false;
      
      const matchesType = !filters.activityType?.length || 
        filters.activityType.includes(activity.activity?.type);
      
      const matchesCategory = !filters.category?.length || 
        filters.category.includes(activity.activity?.category);
      
      const matchesDateRange = !filters.dateRange || (
        activity.completedAt && 
        new Date(activity.completedAt) >= filters.dateRange.start && 
        new Date(activity.completedAt) <= filters.dateRange.end
      );
      
      const matchesDuration = (
        filters.minDuration === undefined || 
        activity.duration >= filters.minDuration
      ) && (
        filters.maxDuration === undefined || 
        activity.duration <= filters.maxDuration
      );
      
      return matchesType && matchesCategory && matchesDateRange && matchesDuration;
    });
  }
);

// Personal best selectors
export const selectPersonalBestsByActivity = createSelector(
  selectPersonalBests,
  (personalBests) => {
    const bestsByActivity: { [activityId: number]: typeof personalBests } = {};
    
    personalBests.forEach(pb => {
      if (!bestsByActivity[pb.activityId]) {
        bestsByActivity[pb.activityId] = [];
      }
      bestsByActivity[pb.activityId].push(pb);
    });
    
    return bestsByActivity;
  }
);

export const selectRecentPersonalBests = createSelector(
  selectPersonalBests,
  (personalBests) => {
    const thirtyDaysAgo = new Date(Date.now() - 30 * 24 * 60 * 60 * 1000);
    
    return personalBests
      .filter(pb => new Date(pb.achievedAt) >= thirtyDaysAgo)
      .sort((a, b) => new Date(b.achievedAt).getTime() - new Date(a.achievedAt).getTime());
  }
);
