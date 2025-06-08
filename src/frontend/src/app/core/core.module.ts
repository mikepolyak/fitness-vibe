import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

// NgRx
import { EffectsModule } from '@ngrx/effects';
import { AuthEffects } from '../store/auth/auth.effects';

// Guards
import { 
  AuthGuard, 
  EmailVerifiedGuard, 
  GuestGuard, 
  RoleGuard, 
  OnboardingGuard 
} from './guards/auth.guard';

// Services
import { AuthService } from './services/auth.service';
import { TokenService } from './services/token.service';
import { NotificationService } from './services/notification.service';

// Interceptors
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { ErrorInterceptor } from './interceptors/error.interceptor';
import { LoadingInterceptor } from './interceptors/loading.interceptor';

/**
 * Core Module - the essential infrastructure of our fitness app.
 * Think of this as the foundational systems of a gym: electrical, plumbing,
 * security, and management systems that everything else depends on.
 * 
 * This module should only be imported once in the AppModule.
 */
@NgModule({
  imports: [
    CommonModule,
    
    // Register effects for the auth feature
    EffectsModule.forFeature([
      AuthEffects
    ])
  ],
  providers: [
    // Guards - our security checkpoint staff
    AuthGuard,
    EmailVerifiedGuard,
    GuestGuard,
    RoleGuard,
    OnboardingGuard,

    // Core services - the essential management staff
    AuthService,
    TokenService,
    NotificationService,

    // HTTP Interceptors - the behind-the-scenes coordinators
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: LoadingInterceptor,
      multi: true
    }
  ]
})
export class CoreModule {
  /**
   * Prevents CoreModule from being imported multiple times.
   * Like ensuring there's only one main electrical panel in the building.
   */
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error(
        'CoreModule is already loaded. Import it only once in AppModule.'
      );
    }
  }
}
