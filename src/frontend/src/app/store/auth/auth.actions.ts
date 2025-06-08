import { createAction, props } from '@ngrx/store';
import { User } from '../../shared/models/user.model';

/**
 * Authentication Actions - the commands that trigger authentication state changes.
 * Think of these as different types of membership activities at a fitness center:
 * joining, checking in, checking out, updating profile, etc.
 */

// Login actions
export const login = createAction(
  '[Auth] Login',
  props<{ email: string; password: string; rememberMe?: boolean }>()
);

export const loginSuccess = createAction(
  '[Auth] Login Success',
  props<{ user: User; token: string; refreshToken: string }>()
);

export const loginFailure = createAction(
  '[Auth] Login Failure',
  props<{ error: string }>()
);

// Registration actions
export const register = createAction(
  '[Auth] Register',
  props<{ 
    email: string; 
    password: string; 
    firstName: string; 
    lastName: string;
    fitnessLevel?: string;
    primaryGoal?: string;
  }>()
);

export const registerSuccess = createAction(
  '[Auth] Register Success',
  props<{ user: User; token: string; refreshToken: string }>()
);

export const registerFailure = createAction(
  '[Auth] Register Failure',
  props<{ error: string }>()
);

// Token refresh actions
export const refreshToken = createAction('[Auth] Refresh Token');

export const refreshTokenSuccess = createAction(
  '[Auth] Refresh Token Success',
  props<{ token: string; refreshToken: string }>()
);

export const refreshTokenFailure = createAction(
  '[Auth] Refresh Token Failure',
  props<{ error: string }>()
);

// Load current user (on app startup)
export const loadCurrentUser = createAction('[Auth] Load Current User');

export const loadCurrentUserSuccess = createAction(
  '[Auth] Load Current User Success',
  props<{ user: User }>()
);

export const loadCurrentUserFailure = createAction(
  '[Auth] Load Current User Failure',
  props<{ error: string }>()
);

// Logout actions
export const logout = createAction('[Auth] Logout');

export const logoutSuccess = createAction('[Auth] Logout Success');

// Password reset actions
export const requestPasswordReset = createAction(
  '[Auth] Request Password Reset',
  props<{ email: string }>()
);

export const requestPasswordResetSuccess = createAction(
  '[Auth] Request Password Reset Success'
);

export const requestPasswordResetFailure = createAction(
  '[Auth] Request Password Reset Failure',
  props<{ error: string }>()
);

export const resetPassword = createAction(
  '[Auth] Reset Password',
  props<{ token: string; newPassword: string }>()
);

export const resetPasswordSuccess = createAction(
  '[Auth] Reset Password Success'
);

export const resetPasswordFailure = createAction(
  '[Auth] Reset Password Failure',
  props<{ error: string }>()
);

// Email verification actions
export const verifyEmail = createAction(
  '[Auth] Verify Email',
  props<{ token: string }>()
);

export const verifyEmailSuccess = createAction(
  '[Auth] Verify Email Success'
);

export const verifyEmailFailure = createAction(
  '[Auth] Verify Email Failure',
  props<{ error: string }>()
);

// Clear auth errors
export const clearAuthError = createAction('[Auth] Clear Error');
