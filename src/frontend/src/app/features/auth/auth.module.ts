import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Material imports
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSelectModule } from '@angular/material/select';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDividerModule } from '@angular/material/divider';

// Feature components
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { ProfileComponent } from './components/profile/profile.component';
import { AuthLayoutComponent } from './components/auth-layout/auth-layout.component';
import { EmailVerificationComponent } from './components/email-verification/email-verification.component';
import { PasswordResetComponent } from './components/password-reset/password-reset.component';

// Shared module
import { SharedModule } from '../../shared/shared.module';

// Routing
import { AuthRoutingModule } from './auth-routing.module';

/**
 * Auth Feature Module - The membership and onboarding center.
 * 
 * Think of this as the front desk of a premium fitness club where:
 * - New visitors sign up for memberships (registration)
 * - Existing members check in (login)
 * - Members update their profiles and preferences
 * - Staff verify member credentials and manage access
 */
@NgModule({
  declarations: [
    LoginComponent,
    RegisterComponent,
    ProfileComponent,
    AuthLayoutComponent,
    EmailVerificationComponent,
    PasswordResetComponent
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterModule,
    SharedModule,
    AuthRoutingModule,
    
    // Material Design components for a polished, modern experience
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatCheckboxModule,
    MatSelectModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTabsModule,
    MatDividerModule
  ]
})
export class AuthModule { }
