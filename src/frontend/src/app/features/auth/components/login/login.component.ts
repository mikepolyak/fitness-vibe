import { Component, OnInit, ChangeDetectionStrategy, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { AppState } from '../../../../store/app.state';
import * as AuthActions from '../../../../store/auth/auth.actions';
import * as AuthSelectors from '../../../../store/auth/auth.selectors';

/**
 * Login Component - The member check-in desk.
 * 
 * Think of this as the friendly check-in counter at your favorite gym:
 * - Quick and easy for returning members
 * - Secure verification of credentials
 * - Helpful guidance for those who forgot their details
 * - Clear path to membership signup for newcomers
 */
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoginComponent implements OnInit, OnDestroy {
  
  loginForm: FormGroup;
  hidePassword = true;
  
  // Observable streams for reactive UI
  isLoading$: Observable<boolean>;
  isLoggingIn$: Observable<boolean>;
  error$: Observable<string | null>;
  
  private destroy$ = new Subject<void>();

  constructor(
    private fb: FormBuilder,
    private store: Store<AppState>,
    private router: Router
  ) {
    this.createForm();
    this.setupObservables();
  }

  ngOnInit(): void {
    this.clearAnyPreviousErrors();
    this.listenForSuccessfulLogin();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Form Creation - Like designing the check-in form at a gym.
   * Simple, clear fields that members can fill out quickly.
   */
  private createForm(): void {
    this.loginForm = this.fb.group({
      email: [
        '', 
        [
          Validators.required, 
          Validators.email,
          Validators.maxLength(100)
        ]
      ],
      password: [
        '', 
        [
          Validators.required,
          Validators.minLength(6),
          Validators.maxLength(100)
        ]
      ],
      rememberMe: [false]
    });
  }

  /**
   * Setup reactive observables for UI state management
   */
  private setupObservables(): void {
    this.isLoading$ = this.store.select(AuthSelectors.selectIsLoading);
    this.isLoggingIn$ = this.store.select(AuthSelectors.selectIsLoggingIn);
    this.error$ = this.store.select(AuthSelectors.selectError);
  }

  /**
   * Clear any lingering error messages when component loads
   */
  private clearAnyPreviousErrors(): void {
    this.store.dispatch(AuthActions.clearAuthError());
  }

  /**
   * Listen for successful login to redirect user to dashboard
   */
  private listenForSuccessfulLogin(): void {
    this.store.select(AuthSelectors.selectIsAuthenticated)
      .pipe(takeUntil(this.destroy$))
      .subscribe(isAuthenticated => {
        if (isAuthenticated) {
          this.router.navigate(['/dashboard']);
        }
      });
  }

  /**
   * Handle login form submission
   */
  onSubmit(): void {
    if (this.loginForm.valid) {
      const { email, password, rememberMe } = this.loginForm.value;
      
      this.store.dispatch(AuthActions.login({
        email: email.trim().toLowerCase(),
        password,
        rememberMe
      }));
    } else {
      this.markFormGroupTouched();
    }
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  private markFormGroupTouched(): void {
    Object.keys(this.loginForm.controls).forEach(key => {
      const control = this.loginForm.get(key);
      control?.markAsTouched();
    });
  }

  /**
   * Toggle password visibility
   */
  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }

  /**
   * Navigate to registration page
   */
  navigateToRegister(): void {
    this.router.navigate(['/auth/register']);
  }

  /**
   * Navigate to password reset page
   */
  navigateToPasswordReset(): void {
    this.router.navigate(['/auth/reset-password']);
  }

  /**
   * Quick demo login for development/demo purposes
   */
  loginAsDemo(): void {
    this.loginForm.patchValue({
      email: 'demo@fitnessvibe.com',
      password: 'demo123'
    });
    this.onSubmit();
  }

  /**
   * Get error message for form field validation
   */
  getFieldError(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    
    if (field?.hasError('required')) {
      return `${this.getFieldDisplayName(fieldName)} is required`;
    }
    
    if (field?.hasError('email')) {
      return 'Please enter a valid email address';
    }
    
    if (field?.hasError('minlength')) {
      const minLength = field.errors?.['minlength'].requiredLength;
      return `Password must be at least ${minLength} characters`;
    }
    
    if (field?.hasError('maxlength')) {
      const maxLength = field.errors?.['maxlength'].requiredLength;
      return `${this.getFieldDisplayName(fieldName)} cannot exceed ${maxLength} characters`;
    }
    
    return '';
  }

  /**
   * Helper to get user-friendly field names for error messages
   */
  private getFieldDisplayName(fieldName: string): string {
    const displayNames: { [key: string]: string } = {
      email: 'Email',
      password: 'Password'
    };
    return displayNames[fieldName] || fieldName;
  }

  /**
   * Check if field should show error
   */
  shouldShowFieldError(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field?.invalid && (field?.dirty || field?.touched));
  }
}
