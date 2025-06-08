import { ActionReducerMap, MetaReducer } from '@ngrx/store';
import { routerReducer, RouterReducerState } from '@ngrx/router-store';
import { environment } from '../../environments/environment';

// Feature state imports
import { AuthState, authReducer } from './auth/auth.reducer';
import { ActivitiesState, activitiesReducer } from './activities/activities.reducer';
import { GamificationState, gamificationReducer } from './gamification/gamification.reducer';
import { GoalsState, goalsReducer } from './goals/goals.reducer';
import { SocialState, socialReducer } from './social/social.reducer';
import { UserProfileState, userProfileReducer } from './user-profile/user-profile.reducer';
import { NutritionState, nutritionReducer } from './nutrition/nutrition.reducer';
import { SettingsState, settingsReducer } from './settings/settings.reducer';

/**
 * Main application state interface - the central nervous system of our app.
 * Think of this as the main control room where all fitness data flows through.
 * Each feature has its own section, like different monitoring stations.
 */
export interface AppState {
  router: RouterReducerState<any>;
  auth: AuthState;
  activities: ActivitiesState;
  gamification: GamificationState;
  goals: GoalsState;
  social: SocialState;
  userProfile: UserProfileState;
  nutrition: NutritionState;
  settings: SettingsState;
}

/**
 * Root reducer configuration - like the main electrical panel
 * that routes power (data) to different parts of the building (features).
 */
export const appReducers: ActionReducerMap<AppState> = {
  router: routerReducer,
  auth: authReducer,
  activities: activitiesReducer,
  gamification: gamificationReducer,
  goals: goalsReducer,
  social: socialReducer,
  userProfile: userProfileReducer,
  nutrition: nutritionReducer,
  settings: settingsReducer
};

/**
 * Meta reducers for development and debugging.
 * Think of these as debugging tools that help us understand
 * what's happening under the hood.
 */
export const metaReducers: MetaReducer<AppState>[] = !environment.production ? [] : [];

/**
 * Utility type for strong typing of selectors
 */
export type AppStateType = typeof appReducers;
