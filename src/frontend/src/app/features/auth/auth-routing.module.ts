import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthLayoutComponent } from './components/auth-layout/auth-layout.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ProfileComponent } from './components/profile/profile.component';
import { EmailVerificationComponent } from './components/email-verification/email-verification.component';
import { PasswordResetComponent } from './components/password-reset/password-reset.component';

/**
 * Authentication Routing - The pathways through our membership center.
 * 
 * Like signs and directions in a fitness club, these routes guide users
 * through different areas of the authentication experience:
 * - Main entrance (auth layout)
 * - Registration desk (sign up)
 * - Check-in counter (login)
 * - Member services (profile, verification, password reset)
 */
const routes: Routes = [
  {
    path: '',
    component: AuthLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'login',
        pathMatch: 'full'
      },
      {
        path: 'login',
        component: LoginComponent,
        data: { 
          title: 'Welcome Back',
          description: 'Sign in to continue your fitness journey'
        }
      },
      {
        path: 'register',
        component: RegisterComponent,
        data: { 
          title: 'Join FitnessVibe',
          description: 'Start your gamified fitness adventure today'
        }
      },
      {
        path: 'profile',
        component: ProfileComponent,
        data: { 
          title: 'Your Profile',
          description: 'Manage your fitness identity and preferences'
        }
      },
      {
        path: 'verify-email',
        component: EmailVerificationComponent,
        data: { 
          title: 'Verify Your Email',
          description: 'Confirm your email to unlock all features'
        }
      },
      {
        path: 'reset-password',
        component: PasswordResetComponent,
        data: { 
          title: 'Reset Password',
          description: 'Get back into your fitness routine'
        }
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthRoutingModule { }
