import { createReducer, on } from '@ngrx/store';
import { 
  Activity, 
  UserActivity, 
  ActivityTemplate, 
  Challenge, 
  UserChallenge,
  ActivityStatsResponse,
  PersonalBest,
  RoutePoint
} from '../../shared/models/activity.model';
import * as ActivitiesActions from './activities.actions';

/**
 * Activities State Interface - the comprehensive tracking system for fitness activities.
 * Think of this as the digital logbook that tracks everything about workouts:
 * - Available exercise types (activities catalog)
 * - Personal workout history (userActivities)
 * - Favorite workout templates
 * - Live tracking session data
 * - Performance statistics
 * - Challenge participation
 */
export interface ActivitiesState {
  // Available activities catalog
  activities: Activity[];
  activitiesLoaded: boolean;
  
  // User's activity history
  userActivities: UserActivity[];
  userActivitiesLoaded: boolean;
  userActivitiesTotalCount: number;
  userActivitiesHasMore: boolean;
  
  // Currently selected/viewed activity
  selectedActivity: UserActivity | null;
  
  // Activity templates
  templates: ActivityTemplate[];
  templatesLoaded: boolean;
  
  // Statistics and analytics
  activityStats: ActivityStatsResponse | null;
  personalBests: PersonalBest[];
  
  // Challenges
  availableChallenges: Challenge[];
  userChallenges: UserChallenge[];
  challengesLoaded: boolean;
  challengesHasMore: boolean;
  
  // Live activity tracking
  liveActivity: {
    activity: UserActivity | null;
    isActive: boolean;
    isPaused: boolean;
    startTime: Date | null;
    pausedTime: Date | null;
    totalPausedDuration: number; // in milliseconds
    liveMetrics: {
      duration: number; // in minutes
      distance: number; // in kilometers
      averageHeartRate: number;
      currentPace: number; // minutes per km
      routePoints: RoutePoint[];
    };
  };
  
  // Loading states for better UX
  isLoading: boolean;
  isCreating: boolean;
  isUpdating: boolean;
  isDeleting: boolean;
  isLoadingStats: boolean;
  isJoiningChallenge: boolean;
  
  // UI State
  filters: {
    activityType?: string[];
    category?: string[];
    dateRange?: { start: Date; end: Date };
    minDuration?: number;
    maxDuration?: number;
  };
  
  // Error handling
  error: string | null;
}

/**
 * Initial state - a fresh workout logbook ready to track fitness journey.
 */
export const initialState: ActivitiesState = {
  activities: [],
  activitiesLoaded: false,
  
  userActivities: [],
  userActivitiesLoaded: false,
  userActivitiesTotalCount: 0,
  userActivitiesHasMore: false,
  
  selectedActivity: null,
  
  templates: [],
  templatesLoaded: false,
  
  activityStats: null,
  personalBests: [],
  
  availableChallenges: [],
  userChallenges: [],
  challengesLoaded: false,
  challengesHasMore: false,
  
  liveActivity: {
    activity: null,
    isActive: false,
    isPaused: false,
    startTime: null,
    pausedTime: null,
    totalPausedDuration: 0,
    liveMetrics: {
      duration: 0,
      distance: 0,
      averageHeartRate: 0,
      currentPace: 0,
      routePoints: []
    }
  },
  
  isLoading: false,
  isCreating: false,
  isUpdating: false,
  isDeleting: false,
  isLoadingStats: false,
  isJoiningChallenge: false,
  
  filters: {},
  
  error: null
};

/**
 * Activities Reducer - the fitness instructor who manages all workout records.
 * Each action is like a different instruction: "log a new workout", 
 * "start live tracking", "view past performance", etc.
 */
export const activitiesReducer = createReducer(
  initialState,

  // Load activities catalog
  on(ActivitiesActions.loadActivities, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(ActivitiesActions.loadActivitiesSuccess, (state, { activities }) => ({
    ...state,
    activities,
    activitiesLoaded: true,
    isLoading: false,
    error: null
  })),

  on(ActivitiesActions.loadActivitiesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // Load user activities
  on(ActivitiesActions.loadUserActivities, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(ActivitiesActions.loadUserActivitiesSuccess, (state, { activities, totalCount, hasMore }) => ({
    ...state,
    userActivities: activities,
    userActivitiesLoaded: true,
    userActivitiesTotalCount: totalCount,
    userActivitiesHasMore: hasMore,
    isLoading: false,
    error: null
  })),

  on(ActivitiesActions.loadUserActivitiesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // Load specific activity
  on(ActivitiesActions.loadActivityById, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(ActivitiesActions.loadActivityByIdSuccess, (state, { activity }) => ({
    ...state,
    selectedActivity: activity,
    isLoading: false,
    error: null
  })),

  on(ActivitiesActions.loadActivityByIdFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // Create activity
  on(ActivitiesActions.createActivity, (state) => ({
    ...state,
    isCreating: true,
    error: null
  })),

  on(ActivitiesActions.createActivitySuccess, (state, { activity }) => ({
    ...state,
    userActivities: [activity, ...state.userActivities],
    userActivitiesTotalCount: state.userActivitiesTotalCount + 1,
    selectedActivity: activity,
    isCreating: false,
    error: null
  })),

  on(ActivitiesActions.createActivityFailure, (state, { error }) => ({
    ...state,
    isCreating: false,
    error
  })),

  // Update activity
  on(ActivitiesActions.updateActivity, (state) => ({
    ...state,
    isUpdating: true,
    error: null
  })),

  on(ActivitiesActions.updateActivitySuccess, (state, { activity }) => ({
    ...state,
    userActivities: state.userActivities.map(a => 
      a.id === activity.id ? activity : a
    ),
    selectedActivity: state.selectedActivity?.id === activity.id ? activity : state.selectedActivity,
    isUpdating: false,
    error: null
  })),

  on(ActivitiesActions.updateActivityFailure, (state, { error }) => ({
    ...state,
    isUpdating: false,
    error
  })),

  // Delete activity
  on(ActivitiesActions.deleteActivity, (state) => ({
    ...state,
    isDeleting: true,
    error: null
  })),

  on(ActivitiesActions.deleteActivitySuccess, (state, { activityId }) => ({
    ...state,
    userActivities: state.userActivities.filter(a => a.id !== activityId),
    userActivitiesTotalCount: state.userActivitiesTotalCount - 1,
    selectedActivity: state.selectedActivity?.id === activityId ? null : state.selectedActivity,
    isDeleting: false,
    error: null
  })),

  on(ActivitiesActions.deleteActivityFailure, (state, { error }) => ({
    ...state,
    isDeleting: false,
    error
  })),

  // Activity templates
  on(ActivitiesActions.loadActivityTemplates, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(ActivitiesActions.loadActivityTemplatesSuccess, (state, { templates }) => ({
    ...state,
    templates,
    templatesLoaded: true,
    isLoading: false,
    error: null
  })),

  on(ActivitiesActions.loadActivityTemplatesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(ActivitiesActions.createActivityTemplateSuccess, (state, { template }) => ({
    ...state,
    templates: [...state.templates, template],
    isCreating: false,
    error: null
  })),

  on(ActivitiesActions.useActivityTemplateSuccess, (state, { template }) => ({
    ...state,
    templates: state.templates.map(t => 
      t.id === template.id ? template : t
    )
  })),

  // Activity statistics
  on(ActivitiesActions.loadActivityStats, (state) => ({
    ...state,
    isLoadingStats: true,
    error: null
  })),

  on(ActivitiesActions.loadActivityStatsSuccess, (state, { stats }) => ({
    ...state,
    activityStats: stats,
    isLoadingStats: false,
    error: null
  })),

  on(ActivitiesActions.loadActivityStatsFailure, (state, { error }) => ({
    ...state,
    isLoadingStats: false,
    error
  })),

  // Personal bests
  on(ActivitiesActions.loadPersonalBestsSuccess, (state, { personalBests }) => ({
    ...state,
    personalBests,
    error: null
  })),

  // Challenges
  on(ActivitiesActions.loadChallenges, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(ActivitiesActions.loadChallengesSuccess, (state, { challenges, hasMore }) => ({
    ...state,
    availableChallenges: challenges,
    challengesLoaded: true,
    challengesHasMore: hasMore,
    isLoading: false,
    error: null
  })),

  on(ActivitiesActions.loadChallengesFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(ActivitiesActions.loadUserChallengesSuccess, (state, { userChallenges }) => ({
    ...state,
    userChallenges,
    error: null
  })),

  on(ActivitiesActions.joinChallenge, (state) => ({
    ...state,
    isJoiningChallenge: true,
    error: null
  })),

  on(ActivitiesActions.joinChallengeSuccess, (state, { userChallenge }) => ({
    ...state,
    userChallenges: [...state.userChallenges, userChallenge],
    isJoiningChallenge: false,
    error: null
  })),

  on(ActivitiesActions.joinChallengeFailure, (state, { error }) => ({
    ...state,
    isJoiningChallenge: false,
    error
  })),

  on(ActivitiesActions.leaveChallengeSuccess, (state, { challengeId }) => ({
    ...state,
    userChallenges: state.userChallenges.filter(uc => uc.challengeId !== challengeId),
    error: null
  })),

  // Live activity tracking
  on(ActivitiesActions.startLiveActivity, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(ActivitiesActions.startLiveActivitySuccess, (state, { liveActivity }) => ({
    ...state,
    liveActivity: {
      ...state.liveActivity,
      activity: liveActivity,
      isActive: true,
      isPaused: false,
      startTime: new Date(),
      pausedTime: null,
      totalPausedDuration: 0,
      liveMetrics: {
        duration: 0,
        distance: 0,
        averageHeartRate: 0,
        currentPace: 0,
        routePoints: []
      }
    },
    isLoading: false,
    error: null
  })),

  on(ActivitiesActions.startLiveActivityFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  on(ActivitiesActions.updateLiveActivity, (state, { duration, distance, heartRate, pace, routePoint }) => {
    const updatedMetrics = { ...state.liveActivity.liveMetrics };
    
    if (duration !== undefined) updatedMetrics.duration = duration;
    if (distance !== undefined) updatedMetrics.distance = distance;
    if (heartRate !== undefined) updatedMetrics.averageHeartRate = heartRate;
    if (pace !== undefined) updatedMetrics.currentPace = pace;
    
    if (routePoint) {
      updatedMetrics.routePoints = [
        ...updatedMetrics.routePoints,
        { ...routePoint, timestamp: new Date(), heartRate, pace }
      ];
    }

    return {
      ...state,
      liveActivity: {
        ...state.liveActivity,
        liveMetrics: updatedMetrics
      }
    };
  }),

  on(ActivitiesActions.pauseLiveActivity, (state) => ({
    ...state,
    liveActivity: {
      ...state.liveActivity,
      isPaused: true,
      pausedTime: new Date()
    }
  })),

  on(ActivitiesActions.resumeLiveActivity, (state) => {
    const now = new Date();
    const pausedTime = state.liveActivity.pausedTime;
    const additionalPausedDuration = pausedTime ? now.getTime() - pausedTime.getTime() : 0;

    return {
      ...state,
      liveActivity: {
        ...state.liveActivity,
        isPaused: false,
        pausedTime: null,
        totalPausedDuration: state.liveActivity.totalPausedDuration + additionalPausedDuration
      }
    };
  }),

  on(ActivitiesActions.stopLiveActivity, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(ActivitiesActions.stopLiveActivitySuccess, (state, { activity }) => ({
    ...state,
    userActivities: [activity, ...state.userActivities],
    userActivitiesTotalCount: state.userActivitiesTotalCount + 1,
    selectedActivity: activity,
    liveActivity: {
      activity: null,
      isActive: false,
      isPaused: false,
      startTime: null,
      pausedTime: null,
      totalPausedDuration: 0,
      liveMetrics: {
        duration: 0,
        distance: 0,
        averageHeartRate: 0,
        currentPace: 0,
        routePoints: []
      }
    },
    isLoading: false,
    error: null
  })),

  on(ActivitiesActions.stopLiveActivityFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // UI state management
  on(ActivitiesActions.setSelectedActivity, (state, { activity }) => ({
    ...state,
    selectedActivity: activity
  })),

  on(ActivitiesActions.setActivityFilters, (state, { filters }) => ({
    ...state,
    filters: { ...state.filters, ...filters }
  })),

  on(ActivitiesActions.clearActivityError, (state) => ({
    ...state,
    error: null
  })),

  // Refresh activities
  on(ActivitiesActions.refreshActivities, (state) => ({
    ...state,
    userActivitiesLoaded: false,
    templatesLoaded: false,
    challengesLoaded: false,
    isLoading: true
  }))
);
