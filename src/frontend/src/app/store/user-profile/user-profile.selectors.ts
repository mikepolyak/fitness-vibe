import { createFeatureSelector, createSelector } from '@ngrx/store';
import { UserProfileState } from './user-profile.reducer';

export const selectUserProfileState = createFeatureSelector<UserProfileState>('userProfile');

export const selectUserProfile = createSelector(
  selectUserProfileState,
  state => state.profile
);

export const selectUserPreferences = createSelector(
  selectUserProfileState,
  state => state.preferences
);

export const selectUserProfileLoading = createSelector(
  selectUserProfileState,
  state => state.loading
);

export const selectUserProfileError = createSelector(
  selectUserProfileState,
  state => state.error
);
