import { createAction, props } from '@ngrx/store';
import { 
  Activity, 
  UserActivity, 
  ActivityTemplate, 
  Challenge, 
  UserChallenge,
  CreateActivityRequest,
  UpdateActivityRequest,
  ActivityStatsResponse,
  PersonalBest
} from '../../shared/models/activity.model';

/**
 * Activities Actions - the commands that trigger activity-related state changes.
 * Think of these as different workout session activities: starting a workout,
 * logging completion, viewing progress, joining challenges, etc.
 */

// Load available activities (catalog of exercises)
export const loadActivities = createAction('[Activities] Load Activities');

export const loadActivitiesSuccess = createAction(
  '[Activities] Load Activities Success',
  props<{ activities: Activity[] }>()
);

export const loadActivitiesFailure = createAction(
  '[Activities] Load Activities Failure',
  props<{ error: string }>()
);

// User Activities CRUD operations
export const loadUserActivities = createAction(
  '[Activities] Load User Activities',
  props<{ userId?: number; page?: number; limit?: number; filters?: any }>()
);

export const loadUserActivitiesSuccess = createAction(
  '[Activities] Load User Activities Success',
  props<{ activities: UserActivity[]; totalCount: number; hasMore: boolean }>()
);

export const loadUserActivitiesFailure = createAction(
  '[Activities] Load User Activities Failure',
  props<{ error: string }>()
);

export const loadActivityById = createAction(
  '[Activities] Load Activity By ID',
  props<{ activityId: number }>()
);

export const loadActivityByIdSuccess = createAction(
  '[Activities] Load Activity By ID Success',
  props<{ activity: UserActivity }>()
);

export const loadActivityByIdFailure = createAction(
  '[Activities] Load Activity By ID Failure',
  props<{ error: string }>()
);

export const createActivity = createAction(
  '[Activities] Create Activity',
  props<{ activity: CreateActivityRequest }>()
);

export const createActivitySuccess = createAction(
  '[Activities] Create Activity Success',
  props<{ activity: UserActivity }>()
);

export const createActivityFailure = createAction(
  '[Activities] Create Activity Failure',
  props<{ error: string }>()
);

export const updateActivity = createAction(
  '[Activities] Update Activity',
  props<{ activityId: number; updates: UpdateActivityRequest }>()
);

export const updateActivitySuccess = createAction(
  '[Activities] Update Activity Success',
  props<{ activity: UserActivity }>()
);

export const updateActivityFailure = createAction(
  '[Activities] Update Activity Failure',
  props<{ error: string }>()
);

export const deleteActivity = createAction(
  '[Activities] Delete Activity',
  props<{ activityId: number }>()
);

export const deleteActivitySuccess = createAction(
  '[Activities] Delete Activity Success',
  props<{ activityId: number }>()
);

export const deleteActivityFailure = createAction(
  '[Activities] Delete Activity Failure',
  props<{ error: string }>()
);

// Activity Templates
export const loadActivityTemplates = createAction('[Activities] Load Activity Templates');

export const loadActivityTemplatesSuccess = createAction(
  '[Activities] Load Activity Templates Success',
  props<{ templates: ActivityTemplate[] }>()
);

export const loadActivityTemplatesFailure = createAction(
  '[Activities] Load Activity Templates Failure',
  props<{ error: string }>()
);

export const createActivityTemplate = createAction(
  '[Activities] Create Activity Template',
  props<{ template: Omit<ActivityTemplate, 'id' | 'userId' | 'useCount' | 'createdAt' | 'updatedAt'> }>()
);

export const createActivityTemplateSuccess = createAction(
  '[Activities] Create Activity Template Success',
  props<{ template: ActivityTemplate }>()
);

export const createActivityTemplateFailure = createAction(
  '[Activities] Create Activity Template Failure',
  props<{ error: string }>()
);

export const useActivityTemplate = createAction(
  '[Activities] Use Activity Template',
  props<{ templateId: number }>()
);

export const useActivityTemplateSuccess = createAction(
  '[Activities] Use Activity Template Success',
  props<{ template: ActivityTemplate }>()
);

// Activity Statistics
export const loadActivityStats = createAction(
  '[Activities] Load Activity Stats',
  props<{ userId?: number; period?: 'week' | 'month' | 'quarter' | 'year' }>()
);

export const loadActivityStatsSuccess = createAction(
  '[Activities] Load Activity Stats Success',
  props<{ stats: ActivityStatsResponse }>()
);

export const loadActivityStatsFailure = createAction(
  '[Activities] Load Activity Stats Failure',
  props<{ error: string }>()
);

// Personal Bests
export const loadPersonalBests = createAction('[Activities] Load Personal Bests');

export const loadPersonalBestsSuccess = createAction(
  '[Activities] Load Personal Bests Success',
  props<{ personalBests: PersonalBest[] }>()
);

export const loadPersonalBestsFailure = createAction(
  '[Activities] Load Personal Bests Failure',
  props<{ error: string }>()
);

// Challenges
export const loadChallenges = createAction(
  '[Activities] Load Challenges',
  props<{ type?: 'available' | 'joined' | 'completed'; page?: number }>()
);

export const loadChallengesSuccess = createAction(
  '[Activities] Load Challenges Success',
  props<{ challenges: Challenge[]; hasMore: boolean }>()
);

export const loadChallengesFailure = createAction(
  '[Activities] Load Challenges Failure',
  props<{ error: string }>()
);

export const loadUserChallenges = createAction('[Activities] Load User Challenges');

export const loadUserChallengesSuccess = createAction(
  '[Activities] Load User Challenges Success',
  props<{ userChallenges: UserChallenge[] }>()
);

export const loadUserChallengesFailure = createAction(
  '[Activities] Load User Challenges Failure',
  props<{ error: string }>()
);

export const joinChallenge = createAction(
  '[Activities] Join Challenge',
  props<{ challengeId: number }>()
);

export const joinChallengeSuccess = createAction(
  '[Activities] Join Challenge Success',
  props<{ userChallenge: UserChallenge }>()
);

export const joinChallengeFailure = createAction(
  '[Activities] Join Challenge Failure',
  props<{ error: string }>()
);

export const leaveChallenge = createAction(
  '[Activities] Leave Challenge',
  props<{ challengeId: number }>()
);

export const leaveChallengeSuccess = createAction(
  '[Activities] Leave Challenge Success',
  props<{ challengeId: number }>()
);

export const leaveChallengeFailure = createAction(
  '[Activities] Leave Challenge Failure',
  props<{ error: string }>()
);

// Live Activity Tracking
export const startLiveActivity = createAction(
  '[Activities] Start Live Activity',
  props<{ activityId: number; location?: string }>()
);

export const startLiveActivitySuccess = createAction(
  '[Activities] Start Live Activity Success',
  props<{ liveActivity: UserActivity }>()
);

export const startLiveActivityFailure = createAction(
  '[Activities] Start Live Activity Failure',
  props<{ error: string }>()
);

export const updateLiveActivity = createAction(
  '[Activities] Update Live Activity',
  props<{ 
    duration: number; 
    distance?: number; 
    heartRate?: number; 
    pace?: number;
    routePoint?: { latitude: number; longitude: number; elevation?: number } 
  }>()
);

export const pauseLiveActivity = createAction('[Activities] Pause Live Activity');

export const resumeLiveActivity = createAction('[Activities] Resume Live Activity');

export const stopLiveActivity = createAction(
  '[Activities] Stop Live Activity',
  props<{ notes?: string; rating?: number; photos?: string[] }>()
);

export const stopLiveActivitySuccess = createAction(
  '[Activities] Stop Live Activity Success',
  props<{ activity: UserActivity }>()
);

export const stopLiveActivityFailure = createAction(
  '[Activities] Stop Live Activity Failure',
  props<{ error: string }>()
);

// UI State actions
export const setSelectedActivity = createAction(
  '[Activities] Set Selected Activity',
  props<{ activity: UserActivity | null }>()
);

export const setActivityFilters = createAction(
  '[Activities] Set Activity Filters',
  props<{ filters: any }>()
);

export const clearActivityError = createAction('[Activities] Clear Error');

export const refreshActivities = createAction('[Activities] Refresh Activities');
