import { createAction, props } from '@ngrx/store';

export const loadUserProfile = createAction('[User Profile] Load Profile');
export const loadUserProfileSuccess = createAction(
  '[User Profile] Load Profile Success',
  props<{ profile: any }>()
);
export const loadUserProfileFailure = createAction(
  '[User Profile] Load Profile Failure',
  props<{ error: any }>()
);

export const updateUserProfile = createAction(
  '[User Profile] Update Profile',
  props<{ profile: any }>()
);
export const updateUserProfileSuccess = createAction(
  '[User Profile] Update Profile Success',
  props<{ profile: any }>()
);
export const updateUserProfileFailure = createAction(
  '[User Profile] Update Profile Failure',
  props<{ error: any }>()
);

export const updateUserPreferences = createAction(
  '[User Profile] Update Preferences',
  props<{ preferences: any }>()
);
export const updateUserPreferencesSuccess = createAction(
  '[User Profile] Update Preferences Success',
  props<{ preferences: any }>()
);
export const updateUserPreferencesFailure = createAction(
  '[User Profile] Update Preferences Failure',
  props<{ error: any }>()
);
