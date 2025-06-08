import { ActionReducerMap, MetaReducer } from '@ngrx/store';
import { routerReducer, RouterReducerState } from '@ngrx/router-store';
import { environment } from '../../environments/environment';

// Feature state imports
import { AuthState, authReducer } from './auth/auth.reducer';
// TODO: Import other reducers as they are implemented
// import { UserState, userReducer } from './user/user.reducer';
// import { ActivityState, activityReducer } from './activity/activity.reducer';
// import { GamificationState, gamificationReducer } from './gamification/gamification.reducer';

/**
 * Main application state interface - the central nervous system of our app.
 * Think of this as the main control room where all fitness data flows through.
 * Each feature has its own section, like different monitoring stations.
 */
export interface AppState {
  router: RouterReducerState<any>;
  auth: AuthState;
  // TODO: Add other feature states as they are implemented
  // user: UserState;
  // activity: ActivityState;
  // gamification: GamificationState;
}

/**
 * Root reducer configuration - like the main electrical panel
 * that routes power (data) to different parts of the building (features).
 */
export const appReducers: ActionReducerMap<AppState> = {
  router: routerReducer,
  auth: authReducer
  // TODO: Add other reducers as they are implemented
  // user: userReducer,
  // activity: activityReducer,
  // gamification: gamificationReducer
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
