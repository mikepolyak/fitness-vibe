import { Injectable } from '@angular/core';
import { 
  CanActivate, 
  CanActivateChild, 
  CanLoad, 
  Route, 
  UrlSegment, 
  ActivatedRouteSnapshot, 
  RouterStateSnapshot, 
  Router 
} from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, take, tap, catchError } from 'rxjs/operators';
import { Store } from '@ngrx/store';

import { AppState } from '../../store/app.state';
import { selectIsAuthenticated, selectCurrentUser } from '../../store/auth/auth.selectors';
import { loadCurrentUser } from '../../store/auth/auth.actions';
import { TokenService } from '../services/token.service';
import { NotificationService } from '../services/notification.service';

/**
 * Authentication Guard - the security checkpoint for our fitness app.
 * Think of this as the front desk staff who check membership cards
 * before allowing access to different areas of the gym.
 */
@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild, CanLoad {

  constructor(
    private store: Store<AppState>,
    private router: Router,
    private tokenService: TokenService,
    private notificationService: NotificationService
  ) {}

  /**
   * Main guard logic - checks if user can access a route.
   * Like verifying membership status before gym entry.
   */
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.checkAuthentication(state.url);
  }

  /**
   * Child route guard - checks access to nested routes.
   * Like checking access to specific gym areas (pool, weights, etc.).
   */
  canActivateChild(
    childRoute: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.canActivate(childRoute, state);
  }

  /**
   * Module loading guard - checks if user can load lazy modules.
   * Like pre-checking membership before opening restricted areas.
   */
  canLoad(route: Route, segments: UrlSegment[]): Observable<boolean> {
    const url = segments.map(segment => segment.path).join('/');
    return this.checkAuthentication(`/${url}`);
  }

  /**
   * Core authentication check logic.
   * This is like the main security process at the gym entrance.
   */
  private checkAuthentication(url: string): Observable<boolean> {
    // First, check if we have a valid token
    if (!this.tokenService.hasValidToken()) {
      this.redirectToLogin(url);
      return of(false);
    }

    // Check authentication state in store
    return this.store.select(selectIsAuthenticated).pipe(
      take(1),
      tap(isAuthenticated => {
        if (!isAuthenticated) {
          // Try to load current user if we have a token but not authenticated in store
          this.store.dispatch(loadCurrentUser());
        }
      }),
      map(isAuthenticated => {
        if (!isAuthenticated) {
          this.redirectToLogin(url);
          return false;
        }
        return true;
      }),
      catchError(() => {
        this.redirectToLogin(url);
        return of(false);
      })
    );
  }

  /**
   * Redirect to login page with return URL.
   * Like escorting someone to the registration desk.
   */
  private redirectToLogin(returnUrl: string): void {
    // Clear any invalid tokens
    this.tokenService.clearExpiredTokens();
    
    // Show friendly message
    this.notificationService.showInfo(
      'Please log in to access this feature.',
      'Login'
    );

    // Navigate to login with return URL
    this.router.navigate(['/auth/login'], {
      queryParams: { returnUrl }
    });
  }
}

/**
 * Email Verification Guard - ensures email is verified for sensitive features.
 * Like checking if someone has completed their membership registration.
 */
@Injectable({
  providedIn: 'root'
})
export class EmailVerifiedGuard implements CanActivate {

  constructor(
    private store: Store<AppState>,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> {
    return this.store.select(selectCurrentUser).pipe(
      take(1),
      map(user => {
        if (!user) {
          this.router.navigate(['/auth/login']);
          return false;
        }

        if (!user.isEmailVerified) {
          this.notificationService.showWarning(
            'Please verify your email address to access this feature.',
            'Verify Now'
          );
          this.router.navigate(['/auth/verify-email']);
          return false;
        }

        return true;
      })
    );
  }
}

/**
 * Guest Guard - prevents authenticated users from accessing auth pages.
 * Like preventing current members from going to the registration desk.
 */
@Injectable({
  providedIn: 'root'
})
export class GuestGuard implements CanActivate {

  constructor(
    private store: Store<AppState>,
    private router: Router
  ) {}

  canActivate(): Observable<boolean> {
    return this.store.select(selectIsAuthenticated).pipe(
      take(1),
      map(isAuthenticated => {
        if (isAuthenticated) {
          this.router.navigate(['/dashboard']);
          return false;
        }
        return true;
      })
    );
  }
}

/**
 * Role Guard - checks if user has required roles.
 * Like checking if someone has premium membership for exclusive areas.
 */
@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {

  constructor(
    private tokenService: TokenService,
    private router: Router,
    private notificationService: NotificationService
  ) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const requiredRoles = route.data['roles'] as string[] || [];
    
    if (requiredRoles.length === 0) {
      return true; // No specific roles required
    }

    const userRoles = this.tokenService.getUserRolesFromToken();
    const hasRequiredRole = requiredRoles.some(role => userRoles.includes(role));

    if (!hasRequiredRole) {
      this.notificationService.showWarning(
        'You don\'t have permission to access this feature.',
        'Contact Support'
      );
      this.router.navigate(['/dashboard']);
      return false;
    }

    return true;
  }
}

/**
 * Onboarding Guard - ensures new users complete onboarding.
 * Like making sure new members get a proper gym orientation.
 */
@Injectable({
  providedIn: 'root'
})
export class OnboardingGuard implements CanActivate {

  constructor(
    private store: Store<AppState>,
    private router: Router
  ) {}

  canActivate(): Observable<boolean> {
    return this.store.select(selectCurrentUser).pipe(
      take(1),
      map(user => {
        if (!user) {
          this.router.navigate(['/auth/login']);
          return false;
        }

        // Check if user needs onboarding (new user with no experience)
        if (user.level === 1 && user.experiencePoints === 0) {
          this.router.navigate(['/onboarding']);
          return false;
        }

        return true;
      })
    );
  }
}
