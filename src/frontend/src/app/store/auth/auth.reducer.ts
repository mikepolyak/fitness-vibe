import { createReducer, on } from '@ngrx/store';
import { User } from '../../shared/models/user.model';
import * as AuthActions from './auth.actions';

/**
 * Authentication State Interface - the membership record for our fitness app.
 * Think of this as the digital membership card that tracks:
 * - Who you are (user profile)
 * - Whether you're currently "checked in" (authenticated)
 * - Your access credentials (tokens)
 * - Any issues with your membership (errors)
 */
export interface AuthState {
  user: User | null;
  token: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
  
  // Specific loading states for better UX
  isLoggingIn: boolean;
  isRegistering: boolean;
  isRefreshingToken: boolean;
  isResettingPassword: boolean;
  isVerifyingEmail: boolean;
}

/**
 * Initial state - what the membership record looks like for a new visitor.
 * Everyone starts as a guest until they sign up or log in.
 */
export const initialState: AuthState = {
  user: null,
  token: null,
  refreshToken: null,
  isAuthenticated: false,
  isLoading: false,
  error: null,
  isLoggingIn: false,
  isRegistering: false,
  isRefreshingToken: false,
  isResettingPassword: false,
  isVerifyingEmail: false
};

/**
 * Auth Reducer - the membership desk clerk who processes all membership changes.
 * Each action is like a different type of form being submitted at the front desk.
 */
export const authReducer = createReducer(
  initialState,

  // Login flow
  on(AuthActions.login, (state) => ({
    ...state,
    isLoggingIn: true,
    isLoading: true,
    error: null
  })),

  on(AuthActions.loginSuccess, (state, { user, token, refreshToken }) => ({
    ...state,
    user,
    token,
    refreshToken,
    isAuthenticated: true,
    isLoggingIn: false,
    isLoading: false,
    error: null
  })),

  on(AuthActions.loginFailure, (state, { error }) => ({
    ...state,
    user: null,
    token: null,
    refreshToken: null,
    isAuthenticated: false,
    isLoggingIn: false,
    isLoading: false,
    error
  })),

  // Registration flow
  on(AuthActions.register, (state) => ({
    ...state,
    isRegistering: true,
    isLoading: true,
    error: null
  })),

  on(AuthActions.registerSuccess, (state, { user, token, refreshToken }) => ({
    ...state,
    user,
    token,
    refreshToken,
    isAuthenticated: true,
    isRegistering: false,
    isLoading: false,
    error: null
  })),

  on(AuthActions.registerFailure, (state, { error }) => ({
    ...state,
    user: null,
    token: null,
    refreshToken: null,
    isAuthenticated: false,
    isRegistering: false,
    isLoading: false,
    error
  })),

  // Token refresh flow
  on(AuthActions.refreshToken, (state) => ({
    ...state,
    isRefreshingToken: true,
    error: null
  })),

  on(AuthActions.refreshTokenSuccess, (state, { token, refreshToken }) => ({
    ...state,
    token,
    refreshToken,
    isRefreshingToken: false,
    error: null
  })),

  on(AuthActions.refreshTokenFailure, (state, { error }) => ({
    ...state,
    user: null,
    token: null,
    refreshToken: null,
    isAuthenticated: false,
    isRefreshingToken: false,
    error
  })),

  // Load current user flow
  on(AuthActions.loadCurrentUser, (state) => ({
    ...state,
    isLoading: true,
    error: null
  })),

  on(AuthActions.loadCurrentUserSuccess, (state, { user }) => ({
    ...state,
    user,
    isAuthenticated: true,
    isLoading: false,
    error: null
  })),

  on(AuthActions.loadCurrentUserFailure, (state, { error }) => ({
    ...state,
    isLoading: false,
    error
  })),

  // Logout flow
  on(AuthActions.logout, (state) => ({
    ...state,
    isLoading: true
  })),

  on(AuthActions.logoutSuccess, () => ({
    ...initialState
  })),

  // Password reset flow
  on(AuthActions.requestPasswordReset, (state) => ({
    ...state,
    isResettingPassword: true,
    isLoading: true,
    error: null
  })),

  on(AuthActions.requestPasswordResetSuccess, (state) => ({
    ...state,
    isResettingPassword: false,
    isLoading: false,
    error: null
  })),

  on(AuthActions.requestPasswordResetFailure, (state, { error }) => ({
    ...state,
    isResettingPassword: false,
    isLoading: false,
    error
  })),

  on(AuthActions.resetPassword, (state) => ({
    ...state,
    isResettingPassword: true,
    isLoading: true,
    error: null
  })),

  on(AuthActions.resetPasswordSuccess, (state) => ({
    ...state,
    isResettingPassword: false,
    isLoading: false,
    error: null
  })),

  on(AuthActions.resetPasswordFailure, (state, { error }) => ({
    ...state,
    isResettingPassword: false,
    isLoading: false,
    error
  })),

  // Email verification flow
  on(AuthActions.verifyEmail, (state) => ({
    ...state,
    isVerifyingEmail: true,
    isLoading: true,
    error: null
  })),

  on(AuthActions.verifyEmailSuccess, (state) => ({
    ...state,
    isVerifyingEmail: false,
    isLoading: false,
    error: null,
    user: state.user ? { ...state.user, isEmailVerified: true } : null
  })),

  on(AuthActions.verifyEmailFailure, (state, { error }) => ({
    ...state,
    isVerifyingEmail: false,
    isLoading: false,
    error
  })),

  // Clear errors
  on(AuthActions.clearAuthError, (state) => ({
    ...state,
    error: null
  }))
);
