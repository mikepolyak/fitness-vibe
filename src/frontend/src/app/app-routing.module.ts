import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './core/guards/auth.guard';

/**
 * App routing configuration - the roadmap for navigating our fitness journey.
 * Think of routes like different areas in a fitness center:
 * - Welcome area (landing/auth)
 * - Main workout floor (dashboard)
 * - Personal training rooms (activities)
 * - Social lounge (community)
 * - Progress tracking room (analytics)
 */
const routes: Routes = [
  // Default redirect to dashboard for authenticated users
  {
    path: '',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/dashboard/dashboard.module').then(m => m.DashboardModule)
  },
  
  // Authentication routes (login, register, forgot password)
  {
    path: 'auth',
    loadChildren: () => import('./features/auth/auth.module').then(m => m.AuthModule)
  },

  // Onboarding flow for new users
  {
    path: 'onboarding',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/onboarding/onboarding.module').then(m => m.OnboardingModule)
  },

  // Activity tracking and logging
  {
    path: 'activities',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/activities/activities.module').then(m => m.ActivitiesModule)
  },

  // Gamification features (badges, challenges, levels)
  {
    path: 'achievements',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/gamification/gamification.module').then(m => m.GamificationModule)
  },

  // Social features (friends, clubs, leaderboards)
  {
    path: 'social',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/social/social.module').then(m => m.SocialModule)
  },

  // Goals and challenges
  {
    path: 'goals',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/goals/goals.module').then(m => m.GoalsModule)
  },

  // Analytics and insights
  {
    path: 'insights',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/analytics/analytics.module').then(m => m.AnalyticsModule)
  },

  // User profile and settings
  {
    path: 'profile',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/profile/profile.module').then(m => m.ProfileModule)
  },

  // Settings and preferences
  {
    path: 'settings',
    canActivate: [AuthGuard],
    loadChildren: () => import('./features/settings/settings.module').then(m => m.SettingsModule)
  },

  // 404 fallback
  {
    path: '**',
    loadChildren: () => import('./features/not-found/not-found.module').then(m => m.NotFoundModule)
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes, {
    // Enable router preloading for better performance
    preloadingStrategy: undefined, // Will implement custom preloading
    
    // Enable tracing for debugging (development only)
    enableTracing: false,
    
    // Scroll to top on route change
    scrollPositionRestoration: 'top',
    
    // Anchor scrolling support
    anchorScrolling: 'enabled',
    
    // Relative link resolution
    relativeLinkResolution: 'legacy'
  })],
  exports: [RouterModule]
})
export class AppRoutingModule { }
