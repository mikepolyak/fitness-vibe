import { Injectable } from '@angular/core';
import { 
  HttpRequest, 
  HttpHandler, 
  HttpEvent, 
  HttpInterceptor, 
  HttpErrorResponse 
} from '@angular/common/http';
import { Observable, throwError, BehaviorSubject } from 'rxjs';
import { catchError, filter, take, switchMap } from 'rxjs/operators';
import { Store } from '@ngrx/store';

import { AppState } from '../../store/app.state';
import { TokenService } from '../services/token.service';
import { refreshToken, logout } from '../../store/auth/auth.actions';

/**
 * Auth Interceptor - the security checkpoint for all API communications.
 * Think of this as the staff member who checks and attaches your membership
 * credentials to every request you make within the gym.
 */
@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private isRefreshing = false;
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(
    private tokenService: TokenService,
    private store: Store<AppState>
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Add auth token to request if available
    const authRequest = this.addTokenToRequest(request);

    return next.handle(authRequest).pipe(
      catchError((error: HttpErrorResponse) => {
        // Handle 401 errors (unauthorized)
        if (error.status === 401 && !authRequest.url.includes('/auth/')) {
          return this.handle401Error(authRequest, next);
        }
        
        return throwError(() => error);
      })
    );
  }

  /**
   * Add authentication token to request headers.
   * Like attaching your membership card to every gym service request.
   */
  private addTokenToRequest(request: HttpRequest<any>): HttpRequest<any> {
    const token = this.tokenService.getAccessToken();
    
    if (token && !this.isAuthRequest(request)) {
      return request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }
    
    return request;
  }

  /**
   * Check if this is an authentication-related request.
   * Like identifying if someone is at the registration desk vs. using gym services.
   */
  private isAuthRequest(request: HttpRequest<any>): boolean {
    return request.url.includes('/auth/') || 
           request.url.includes('/login') || 
           request.url.includes('/register');
  }

  /**
   * Handle 401 unauthorized errors by attempting token refresh.
   * Like trying to renew an expired membership card automatically.
   */
  private handle401Error(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!this.isRefreshing) {
      this.isRefreshing = true;
      this.refreshTokenSubject.next(null);

      const refreshToken = this.tokenService.getRefreshToken();
      
      if (refreshToken) {
        // Dispatch refresh token action
        this.store.dispatch(refreshToken());
        
        // Wait for token refresh to complete
        return this.waitForTokenRefresh().pipe(
          switchMap(() => {
            this.isRefreshing = false;
            const newToken = this.tokenService.getAccessToken();
            
            if (newToken) {
              // Retry the original request with new token
              const newRequest = request.clone({
                setHeaders: {
                  Authorization: `Bearer ${newToken}`
                }
              });
              return next.handle(newRequest);
            }
            
            // If no new token, logout user
            this.store.dispatch(logout());
            return throwError(() => new Error('Token refresh failed'));
          }),
          catchError((error) => {
            this.isRefreshing = false;
            this.store.dispatch(logout());
            return throwError(() => error);
          })
        );
      } else {
        // No refresh token available, logout immediately
        this.isRefreshing = false;
        this.store.dispatch(logout());
        return throwError(() => new Error('No refresh token available'));
      }
    } else {
      // Token refresh is already in progress, wait for it
      return this.refreshTokenSubject.pipe(
        filter(token => token !== null),
        take(1),
        switchMap(() => {
          const newToken = this.tokenService.getAccessToken();
          if (newToken) {
            const newRequest = request.clone({
              setHeaders: {
                Authorization: `Bearer ${newToken}`
              }
            });
            return next.handle(newRequest);
          }
          return throwError(() => new Error('Token refresh failed'));
        })
      );
    }
  }

  /**
   * Wait for token refresh to complete.
   * Like waiting in line while your membership card is being renewed.
   */
  private waitForTokenRefresh(): Observable<any> {
    return new Observable(observer => {
      const checkToken = () => {
        const token = this.tokenService.getAccessToken();
        if (token && !this.tokenService.isTokenExpired(token)) {
          this.refreshTokenSubject.next(token);
          observer.next(token);
          observer.complete();
        } else {
          // Check again in 100ms
          setTimeout(checkToken, 100);
        }
      };
      
      // Start checking
      checkToken();
      
      // Timeout after 10 seconds
      setTimeout(() => {
        observer.error(new Error('Token refresh timeout'));
      }, 10000);
    });
  }
}
