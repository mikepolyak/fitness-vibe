import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, exhaustMap, catchError, tap, switchMap } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';

import { AuthService } from '../../core/services/auth.service';
import { TokenService } from '../../core/services/token.service';
import { NotificationService } from '../../core/services/notification.service';
import * as AuthActions from './auth.actions';

/**
 * Auth Effects - the specialized workers who handle all the complex tasks
 * related to authentication. Think of them as the behind-the-scenes staff
 * who process membership applications, handle renewals, and manage access.
 */
@Injectable()
export class AuthEffects {

  constructor(
    private actions$: Actions,
    private authService: AuthService,
    private tokenService: TokenService,
    private notificationService: NotificationService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  /**
   * Login Effect - handles the login process
   * Like the front desk checking your membership card and letting you in
   */
  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      exhaustMap(({ email, password }) =>
        this.authService.login(email, password).pipe(
          map(response => {
            // Store tokens securely
            this.tokenService.setTokens(response.token, response.refreshToken);
            
            // Show welcome message
            this.notificationService.showSuccess(
              `Welcome back, ${response.user.firstName}! Ready for your fitness journey today?`
            );
            
            return AuthActions.loginSuccess({
              user: response.user,
              token: response.token,
              refreshToken: response.refreshToken
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              error.message || 'Login failed. Please check your credentials.'
            );
            return of(AuthActions.loginFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Registration Effect - handles new user registration
   * Like signing up for a gym membership with all the onboarding
   */
  register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.register),
      exhaustMap(({ email, password, firstName, lastName, fitnessLevel, primaryGoal }) =>
        this.authService.register({
          email,
          password,
          firstName,
          lastName,
          fitnessLevel,
          primaryGoal
        }).pipe(
          map(response => {
            // Store tokens securely
            this.tokenService.setTokens(response.token, response.refreshToken);
            
            // Show welcome message for new user
            this.notificationService.showSuccess(
              `Welcome to FitnessVibe, ${response.user.firstName}! Your fitness journey starts now! 🎉`
            );
            
            return AuthActions.registerSuccess({
              user: response.user,
              token: response.token,
              refreshToken: response.refreshToken
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              error.message || 'Registration failed. Please try again.'
            );
            return of(AuthActions.registerFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Current User Effect - checks if user is already authenticated
   * Like checking if someone already has a valid membership when they arrive
   */
  loadCurrentUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loadCurrentUser),
      switchMap(() => {
        const token = this.tokenService.getAccessToken();
        if (!token) {
          return of(AuthActions.loadCurrentUserFailure({ error: 'No token found' }));
        }

        return this.authService.getCurrentUser().pipe(
          map(user => AuthActions.loadCurrentUserSuccess({ user })),
          catchError(error => {
            // Token might be expired, try to refresh
            return this.tryRefreshToken();
          })
        );
      })
    )
  );

  /**
   * Token Refresh Effect - automatically refreshes expired tokens
   * Like automatically renewing a membership card before it expires
   */
  refreshToken$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.refreshToken),
      switchMap(() => {
        const refreshToken = this.tokenService.getRefreshToken();
        if (!refreshToken) {
          return of(AuthActions.refreshTokenFailure({ error: 'No refresh token found' }));
        }

        return this.authService.refreshToken(refreshToken).pipe(
          map(response => {
            this.tokenService.setTokens(response.token, response.refreshToken);
            return AuthActions.refreshTokenSuccess({
              token: response.token,
              refreshToken: response.refreshToken
            });
          }),
          catchError(error => {
            this.tokenService.clearTokens();
            return of(AuthActions.refreshTokenFailure({ error: error.message }));
          })
        );
      })
    )
  );

  /**
   * Logout Effect - handles user logout
   * Like checking out of the gym and clearing your locker
   */
  logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logout),
      switchMap(() =>
        this.authService.logout().pipe(
          map(() => {
            this.tokenService.clearTokens();
            this.router.navigate(['/auth/login']);
            this.notificationService.showInfo('You have been logged out. See you next time! 👋');
            return AuthActions.logoutSuccess();
          }),
          catchError(() => {
            // Even if logout fails on server, clear local tokens
            this.tokenService.clearTokens();
            this.router.navigate(['/auth/login']);
            return of(AuthActions.logoutSuccess());
          })
        )
      )
    )
  );

  /**
   * Password Reset Request Effect
   * Like requesting a new key when you've lost your gym access card
   */
  requestPasswordReset$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.requestPasswordReset),
      exhaustMap(({ email }) =>
        this.authService.requestPasswordReset(email).pipe(
          map(() => {
            this.notificationService.showSuccess(
              'Password reset instructions have been sent to your email.'
            );
            return AuthActions.requestPasswordResetSuccess();
          }),
          catchError(error => {
            this.notificationService.showError(
              error.message || 'Failed to send password reset email.'
            );
            return of(AuthActions.requestPasswordResetFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Password Reset Effect
   * Like using your new temporary access code to set a new permanent password
   */
  resetPassword$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.resetPassword),
      exhaustMap(({ token, newPassword }) =>
        this.authService.resetPassword(token, newPassword).pipe(
          map(() => {
            this.notificationService.showSuccess(
              'Password reset successfully! You can now log in with your new password.'
            );
            this.router.navigate(['/auth/login']);
            return AuthActions.resetPasswordSuccess();
          }),
          catchError(error => {
            this.notificationService.showError(
              error.message || 'Failed to reset password.'
            );
            return of(AuthActions.resetPasswordFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Email Verification Effect
   * Like confirming your contact information to complete membership setup
   */
  verifyEmail$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.verifyEmail),
      exhaustMap(({ token }) =>
        this.authService.verifyEmail(token).pipe(
          map(() => {
            this.notificationService.showSuccess(
              'Email verified successfully! You now have full access to all features. 🎉'
            );
            return AuthActions.verifyEmailSuccess();
          }),
          catchError(error => {
            this.notificationService.showError(
              error.message || 'Email verification failed.'
            );
            return of(AuthActions.verifyEmailFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Post-login navigation effect
   * Like guiding a member to the right area after they check in
   */
  navigateAfterAuth$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loginSuccess, AuthActions.registerSuccess),
      tap(({ user }) => {
        // Navigate based on user status
        if (!user.isEmailVerified) {
          this.router.navigate(['/auth/verify-email']);
        } else if (user.level === 1 && user.experiencePoints === 0) {
          // New user - show onboarding
          this.router.navigate(['/onboarding']);
        } else {
          // Existing user - go to dashboard
          this.router.navigate(['/dashboard']);
        }
      })
    ),
    { dispatch: false }
  );

  /**
   * Auto-logout on token refresh failure
   * Like automatically ending someone's session if their membership can't be renewed
   */
  autoLogoutOnTokenFailure$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.refreshTokenFailure),
      tap(() => {
        this.router.navigate(['/auth/login']);
        this.notificationService.showWarning(
          'Your session has expired. Please log in again to continue.'
        );
      }),
      map(() => AuthActions.logoutSuccess())
    )
  );

  /**
   * Helper method to try token refresh
   */
  private tryRefreshToken() {
    const refreshToken = this.tokenService.getRefreshToken();
    if (!refreshToken) {
      return of(AuthActions.loadCurrentUserFailure({ error: 'No refresh token' }));
    }

    return this.authService.refreshToken(refreshToken).pipe(
      switchMap(response => {
        this.tokenService.setTokens(response.token, response.refreshToken);
        return this.authService.getCurrentUser().pipe(
          map(user => AuthActions.loadCurrentUserSuccess({ user }))
        );
      }),
      catchError(error => {
        this.tokenService.clearTokens();
        return of(AuthActions.loadCurrentUserFailure({ error: error.message }));
      })
    );
  }
}
