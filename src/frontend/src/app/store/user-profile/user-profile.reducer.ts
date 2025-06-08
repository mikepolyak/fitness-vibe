import { createReducer, on } from '@ngrx/store';
import * as UserProfileActions from './user-profile.actions';

export interface UserProfileState {
  profile: any;
  preferences: any;
  loading: boolean;
  error: any;
}

export const initialState: UserProfileState = {
  profile: null,
  preferences: null,
  loading: false,
  error: null
};

export const userProfileReducer = createReducer(
  initialState,
  on(UserProfileActions.loadUserProfile, state => ({
    ...state,
    loading: true
  })),
  on(UserProfileActions.loadUserProfileSuccess, (state, { profile }) => ({
    ...state,
    loading: false,
    profile,
    error: null
  })),
  on(UserProfileActions.loadUserProfileFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(UserProfileActions.updateUserProfile, state => ({
    ...state,
    loading: true
  })),
  on(UserProfileActions.updateUserProfileSuccess, (state, { profile }) => ({
    ...state,
    loading: false,
    profile,
    error: null
  })),
  on(UserProfileActions.updateUserProfileFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(UserProfileActions.updateUserPreferences, state => ({
    ...state,
    loading: true
  })),
  on(UserProfileActions.updateUserPreferencesSuccess, (state, { preferences }) => ({
    ...state,
    loading: false,
    preferences,
    error: null
  })),
  on(UserProfileActions.updateUserPreferencesFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  }))
);
