import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

import { environment } from '../../../environments/environment';
import { 
  User, 
  AuthResponse, 
  TokenRefreshResponse, 
  CreateUserRequest 
} from '../../shared/models/user.model';

/**
 * Authentication Service - the main security desk for our fitness app.
 * Think of this as the professional staff who handle all membership
 * registrations, check-ins, and access control for the fitness center.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = `${environment.apiUrl}/auth`;

  constructor(private http: HttpClient) {}

  /**
   * User login - like checking in at the gym with your membership card.
   * Verifies credentials and provides access tokens for the session.
   */
  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, {
      email: email.trim().toLowerCase(),
      password
    }).pipe(
      map(response => ({
        ...response,
        user: {
          ...response.user,
          createdAt: new Date(response.user.createdAt),
          updatedAt: response.user.updatedAt ? new Date(response.user.updatedAt) : undefined,
          lastActiveDate: new Date(response.user.lastActiveDate),
          dateOfBirth: response.user.dateOfBirth ? new Date(response.user.dateOfBirth) : undefined
        }
      })),
      catchError(this.handleError)
    );
  }

  /**
   * User registration - like signing up for a new gym membership.
   * Creates a new user account and immediately provides access.
   */
  register(request: CreateUserRequest): Observable<AuthResponse> {
    const payload = {
      ...request,
      email: request.email.trim().toLowerCase()
    };

    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, payload).pipe(
      map(response => ({
        ...response,
        user: {
          ...response.user,
          createdAt: new Date(response.user.createdAt),
          updatedAt: response.user.updatedAt ? new Date(response.user.updatedAt) : undefined,
          lastActiveDate: new Date(response.user.lastActiveDate),
          dateOfBirth: response.user.dateOfBirth ? new Date(response.user.dateOfBirth) : undefined
        }
      })),
      catchError(this.handleError)
    );
  }

  /**
   * Get current user - like checking your current membership status.
   * Retrieves the latest user information from the server.
   */
  getCurrentUser(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/me`).pipe(
      map(user => ({
        ...user,
        createdAt: new Date(user.createdAt),
        updatedAt: user.updatedAt ? new Date(user.updatedAt) : undefined,
        lastActiveDate: new Date(user.lastActiveDate),
        dateOfBirth: user.dateOfBirth ? new Date(user.dateOfBirth) : undefined
      })),
      catchError(this.handleError)
    );
  }

  /**
   * Refresh authentication token - like renewing your gym access card.
   * Uses the refresh token to get a new access token without re-login.
   */
  refreshToken(refreshToken: string): Observable<TokenRefreshResponse> {
    return this.http.post<TokenRefreshResponse>(`${this.apiUrl}/refresh`, {
      refreshToken
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * User logout - like checking out of the gym.
   * Invalidates the current session on the server.
   */
  logout(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/logout`, {}).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Request password reset - like requesting a new access code.
   * Sends a password reset email to the user.
   */
  requestPasswordReset(email: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/forgot-password`, {
      email: email.trim().toLowerCase()
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Reset password - like setting a new access code.
   * Uses the reset token from email to set a new password.
   */
  resetPassword(token: string, newPassword: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/reset-password`, {
      token,
      newPassword
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Verify email address - like confirming your contact information.
   * Validates the email verification token sent to the user.
   */
  verifyEmail(token: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/verify-email`, {
      token
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Resend email verification - like requesting a new confirmation code.
   * Sends a new verification email to the user.
   */
  resendEmailVerification(): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/resend-verification`, {}).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Change password - like updating your gym access code.
   * Allows authenticated users to change their password.
   */
  changePassword(currentPassword: string, newPassword: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/change-password`, {
      currentPassword,
      newPassword
    }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Check if email exists - like checking if someone is already a member.
   * Validates email availability during registration.
   */
  checkEmailExists(email: string): Observable<boolean> {
    return this.http.get<{ exists: boolean }>(`${this.apiUrl}/check-email`, {
      params: { email: email.trim().toLowerCase() }
    }).pipe(
      map(response => response.exists),
      catchError(this.handleError)
    );
  }

  /**
   * Validate reset token - like checking if an access code is still valid.
   * Verifies that a password reset token is valid and not expired.
   */
  validateResetToken(token: string): Observable<boolean> {
    return this.http.get<{ valid: boolean }>(`${this.apiUrl}/validate-reset-token`, {
      params: { token }
    }).pipe(
      map(response => response.valid),
      catchError(this.handleError)
    );
  }

  /**
   * Error handling - like having a helpful staff member explain any issues.
   * Provides user-friendly error messages for common authentication problems.
   */
  private handleError = (error: HttpErrorResponse): Observable<never> => {
    let errorMessage = 'An unexpected error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Network error: ${error.error.message}`;
    } else {
      // Server-side error
      switch (error.status) {
        case 400:
          errorMessage = error.error?.message || 'Invalid request data';
          break;
        case 401:
          errorMessage = 'Invalid email or password';
          break;
        case 403:
          errorMessage = 'Access denied';
          break;
        case 404:
          errorMessage = 'Service not available';
          break;
        case 409:
          errorMessage = error.error?.message || 'Email already exists';
          break;
        case 422:
          errorMessage = error.error?.message || 'Invalid data provided';
          break;
        case 429:
          errorMessage = 'Too many requests. Please try again later';
          break;
        case 500:
          errorMessage = 'Server error. Please try again later';
          break;
        default:
          errorMessage = error.error?.message || `Error ${error.status}: ${error.statusText}`;
      }
    }

    console.error('Auth Service Error:', error);
    return throwError(() => new Error(errorMessage));
  };
}
