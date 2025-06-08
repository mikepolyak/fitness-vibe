import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AuthState } from './auth.reducer';

/**
 * Auth Selectors - the specialized staff who can quickly find specific information
 * about a member's status. Think of them as expert receptionists who can instantly
 * tell you if someone is checked in, what their membership level is, etc.
 */

// Feature selector - gets the entire auth section of the store
export const selectAuthState = createFeatureSelector<AuthState>('auth');

// Basic state selectors
export const selectCurrentUser = createSelector(
  selectAuthState,
  (state: AuthState) => state.user
);

export const selectIsAuthenticated = createSelector(
  selectAuthState,
  (state: AuthState) => state.isAuthenticated
);

export const selectAuthToken = createSelector(
  selectAuthState,
  (state: AuthState) => state.token
);

export const selectRefreshToken = createSelector(
  selectAuthState,
  (state: AuthState) => state.refreshToken
);

export const selectAuthError = createSelector(
  selectAuthState,
  (state: AuthState) => state.error
);

// Loading state selectors
export const selectIsLoading = createSelector(
  selectAuthState,
  (state: AuthState) => state.isLoading
);

export const selectIsLoggingIn = createSelector(
  selectAuthState,
  (state: AuthState) => state.isLoggingIn
);

export const selectIsRegistering = createSelector(
  selectAuthState,
  (state: AuthState) => state.isRegistering
);

export const selectIsRefreshingToken = createSelector(
  selectAuthState,
  (state: AuthState) => state.isRefreshingToken
);

export const selectIsResettingPassword = createSelector(
  selectAuthState,
  (state: AuthState) => state.isResettingPassword
);

export const selectIsVerifyingEmail = createSelector(
  selectAuthState,
  (state: AuthState) => state.isVerifyingEmail
);

// Computed selectors for complex logic
export const selectUserDisplayName = createSelector(
  selectCurrentUser,
  (user) => user ? `${user.firstName} ${user.lastName}` : null
);

export const selectUserLevel = createSelector(
  selectCurrentUser,
  (user) => user?.level ?? 1
);

export const selectUserExperience = createSelector(
  selectCurrentUser,
  (user) => user?.experiencePoints ?? 0
);

export const selectUserProgress = createSelector(
  selectCurrentUser,
  (user) => {
    if (!user) return { currentXP: 0, nextLevelXP: 100, progress: 0 };
    
    const currentLevel = user.level;
    const currentXP = user.experiencePoints;
    
    // Calculate XP needed for current level and next level
    // Each level requires: level * 100 XP (100, 200, 300, etc.)
    const currentLevelRequiredXP = currentLevel === 1 ? 0 : 
      Array.from({length: currentLevel - 1}, (_, i) => (i + 1) * 100)
        .reduce((sum, xp) => sum + xp, 0);
    
    const nextLevelRequiredXP = currentLevelRequiredXP + (currentLevel * 100);
    const progressInCurrentLevel = currentXP - currentLevelRequiredXP;
    const progressNeeded = currentLevel * 100;
    const progressPercentage = (progressInCurrentLevel / progressNeeded) * 100;
    
    return {
      currentXP: progressInCurrentLevel,
      nextLevelXP: progressNeeded,
      progress: Math.min(100, Math.max(0, progressPercentage)),
      totalXP: currentXP,
      level: currentLevel
    };
  }
);

export const selectIsEmailVerified = createSelector(
  selectCurrentUser,
  (user) => user?.isEmailVerified ?? false
);

export const selectUserAvatar = createSelector(
  selectCurrentUser,
  (user) => user?.avatarUrl ?? null
);

export const selectUserFitnessProfile = createSelector(
  selectCurrentUser,
  (user) => user ? {
    fitnessLevel: user.fitnessLevel,
    primaryGoal: user.primaryGoal,
    preferences: user.preferences
  } : null
);

// Authentication status checks
export const selectCanAccess = createSelector(
  selectIsAuthenticated,
  selectIsEmailVerified,
  (isAuth, isVerified) => isAuth && isVerified
);

export const selectNeedsEmailVerification = createSelector(
  selectIsAuthenticated,
  selectIsEmailVerified,
  (isAuth, isVerified) => isAuth && !isVerified
);

// Error handling selectors
export const selectHasAuthError = createSelector(
  selectAuthError,
  (error) => !!error
);

export const selectAuthErrorMessage = createSelector(
  selectAuthError,
  (error) => error || 'An unexpected error occurred'
);

// Any loading state
export const selectIsAnyAuthLoading = createSelector(
  selectIsLoading,
  selectIsLoggingIn,
  selectIsRegistering,
  selectIsRefreshingToken,
  selectIsResettingPassword,
  selectIsVerifyingEmail,
  (isLoading, isLoggingIn, isRegistering, isRefreshing, isResetting, isVerifying) =>
    isLoading || isLoggingIn || isRegistering || isRefreshing || isResetting || isVerifying
);
