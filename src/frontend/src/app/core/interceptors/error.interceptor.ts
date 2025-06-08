import { Injectable } from '@angular/core';
import { 
  HttpRequest, 
  HttpHandler, 
  HttpEvent, 
  HttpInterceptor, 
  HttpErrorResponse 
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

import { NotificationService } from '../services/notification.service';
import { environment } from '../../../environments/environment';

/**
 * Error Interceptor - the customer service manager for our fitness app.
 * Think of this as the helpful staff member who handles complaints,
 * explains problems, and tries to resolve issues gracefully.
 */
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private notificationService: NotificationService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      // Retry failed requests (except for certain types)
      retry({
        count: this.shouldRetry(request) ? 2 : 0,
        delay: 1000
      }),
      
      catchError((error: HttpErrorResponse) => {
        this.handleError(error, request);
        return throwError(() => error);
      })
    );
  }

  /**
   * Determine if a request should be retried on failure.
   * Like deciding whether to try fixing a machine before calling maintenance.
   */
  private shouldRetry(request: HttpRequest<any>): boolean {
    // Don't retry authentication requests
    if (request.url.includes('/auth/')) {
      return false;
    }
    
    // Don't retry POST/PUT/DELETE requests (avoid duplicate actions)
    if (['POST', 'PUT', 'DELETE'].includes(request.method)) {
      return false;
    }
    
    return true;
  }

  /**
   * Handle different types of HTTP errors.
   * Like having different protocols for different types of gym issues.
   */
  private handleError(error: HttpErrorResponse, request: HttpRequest<any>): void {
    let userMessage = '';
    let showNotification = true;

    // Don't show notifications for certain requests
    if (this.shouldSuppressNotification(request, error)) {
      showNotification = false;
    }

    if (error.error instanceof ErrorEvent) {
      // Client-side network error
      userMessage = 'Connection problem. Please check your internet connection.';
      this.logError('Network Error', error.error.message, request);
    } else {
      // Server-side error
      userMessage = this.getServerErrorMessage(error);
      this.logError(`HTTP ${error.status}`, error.message, request);
    }

    // Show user-friendly notification
    if (showNotification && userMessage) {
      this.showErrorNotification(error.status, userMessage);
    }
  }

  /**
   * Get user-friendly error message based on HTTP status.
   * Like translating technical problems into understandable language.
   */
  private getServerErrorMessage(error: HttpErrorResponse): string {
    // Try to get message from server response first
    const serverMessage = error.error?.message || error.error?.error;
    
    switch (error.status) {
      case 400:
        return serverMessage || 'Invalid request. Please check your input.';
      
      case 401:
        return 'Your session has expired. Please log in again.';
      
      case 403:
        return 'You don\'t have permission to perform this action.';
      
      case 404:
        return 'The requested resource was not found.';
      
      case 408:
        return 'Request timeout. Please try again.';
      
      case 409:
        return serverMessage || 'This action conflicts with existing data.';
      
      case 422:
        return serverMessage || 'The data provided is invalid.';
      
      case 429:
        return 'Too many requests. Please slow down and try again later.';
      
      case 500:
        return 'Server error. Our team has been notified.';
      
      case 502:
        return 'Service temporarily unavailable. Please try again later.';
      
      case 503:
        return 'Service under maintenance. Please try again later.';
      
      case 504:
        return 'Request timeout. The server took too long to respond.';
      
      default:
        return serverMessage || 'An unexpected error occurred. Please try again.';
    }
  }

  /**
   * Determine if error notification should be suppressed.
   * Like knowing when not to announce certain types of issues.
   */
  private shouldSuppressNotification(request: HttpRequest<any>, error: HttpErrorResponse): boolean {
    // Suppress notifications for background polling requests
    if (request.headers.get('X-Silent-Request') === 'true') {
      return true;
    }
    
    // Suppress 401 errors for auth requests (handled elsewhere)
    if (error.status === 401 && request.url.includes('/auth/')) {
      return true;
    }
    
    // Suppress certain status codes that are handled by components
    const suppressedStatuses = [401]; // Add more as needed
    if (suppressedStatuses.includes(error.status)) {
      return true;
    }
    
    return false;
  }

  /**
   * Show appropriate error notification based on severity.
   * Like choosing the right tone for different types of announcements.
   */
  private showErrorNotification(status: number, message: string): void {
    if (status >= 500) {
      // Server errors - show as error notifications
      this.notificationService.showError(message, 'Retry');
    } else if (status >= 400) {
      // Client errors - show as warnings
      this.notificationService.showWarning(message, 'OK');
    } else {
      // Other errors - show as info
      this.notificationService.showInfo(message, 'Got it');
    }
  }

  /**
   * Log error details for debugging and monitoring.
   * Like keeping a maintenance log of all issues.
   */
  private logError(type: string, message: string, request: HttpRequest<any>): void {
    const errorDetails = {
      type,
      message,
      url: request.url,
      method: request.method,
      timestamp: new Date().toISOString(),
      userAgent: navigator.userAgent,
      userId: this.getCurrentUserId()
    };

    // Log to console in development
    if (!environment.production) {
      console.error('HTTP Error:', errorDetails);
    }

    // In production, you might want to send errors to a logging service
    if (environment.production && environment.logging.enableRemoteLogging) {
      this.sendErrorToLoggingService(errorDetails);
    }
  }

  /**
   * Get current user ID for error tracking.
   * Like noting which member experienced the issue.
   */
  private getCurrentUserId(): string | null {
    try {
      // This would typically come from your auth service or token
      // For now, we'll return null as a placeholder
      return null;
    } catch {
      return null;
    }
  }

  /**
   * Send error to remote logging service.
   * Like reporting issues to the management system.
   */
  private sendErrorToLoggingService(errorDetails: any): void {
    // Implement your error logging service integration here
    // Examples: Sentry, LogRocket, Bugsnag, etc.
    try {
      // Example implementation:
      // this.errorLoggingService.logError(errorDetails);
    } catch (loggingError) {
      console.error('Failed to log error to remote service:', loggingError);
    }
  }
}
