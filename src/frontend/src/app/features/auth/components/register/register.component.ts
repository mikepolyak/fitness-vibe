import { Component, OnInit, ChangeDetectionStrategy, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { AppState } from '../../../../store/app.state';
import * as AuthActions from '../../../../store/auth/auth.actions';
import * as AuthSelectors from '../../../../store/auth/auth.selectors';
import { FitnessLevel, FitnessGoal } from '../../../../shared/models/user.model';

/**
 * Register Component - The membership enrollment center.
 * 
 * Think of this as the welcoming enrollment desk where new members:
 * - Fill out their fitness profile and goals
 * - Choose their starting fitness level
 * - Set up their account credentials
 * - Begin their gamified fitness adventure
 */
@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class RegisterComponent implements OnInit, OnDestroy {
  
  registerForm: FormGroup;
  hidePassword = true;
  hideConfirmPassword = true;
  
  // Enum references for template use
  fitnessLevels = Object.values(FitnessLevel);
  fitnessGoals = Object.values(FitnessGoal);
  
  // Observable streams for reactive UI
  isLoading$: Observable<boolean>;
  isRegistering$: Observable<boolean>;
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
    this.listenForSuccessfulRegistration();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  /**
   * Form Creation - Like designing the membership application.
   * Comprehensive yet user-friendly form that captures essential info.
   */
  private createForm(): void {
    this.registerForm = this.fb.group({
      // Personal Information
      firstName: [
        '', 
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(50),
          Validators.pattern(/^[a-zA-Z\s]*$/)
        ]
      ],
      lastName: [
        '', 
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(50),
          Validators.pattern(/^[a-zA-Z\s]*$/)
        ]
      ],
      email: [
        '', 
        [
          Validators.required, 
          Validators.email,
          Validators.maxLength(100)
        ]
      ],
      
      // Security
      password: [
        '', 
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(100),
          this.passwordStrengthValidator
        ]
      ],
      confirmPassword: [
        '', 
        [Validators.required]
      ],
      
      // Fitness Profile
      fitnessLevel: [FitnessLevel.Beginner, Validators.required],
      primaryGoal: [FitnessGoal.StayActive, Validators.required],
      
      // Terms and Privacy
      agreeToTerms: [false, Validators.requiredTrue],
      allowMarketing: [true] // Optional marketing emails
    }, {
      validators: this.passwordMatchValidator
    });
  }

  /**
   * Custom validator for password strength
   */
  private passwordStrengthValidator(control: AbstractControl): { [key: string]: any } | null {
    const value = control.value;
    if (!value) return null;

    const hasNumber = /[0-9]/.test(value);
    const hasUpper = /[A-Z]/.test(value);
    const hasLower = /[a-z]/.test(value);
    const hasSpecial = /[#?!@$%^&*-]/.test(value);

    const valid = hasNumber && hasUpper && hasLower && (hasSpecial || value.length >= 12);
    
    if (!valid) {
      return { 
        passwordStrength: {
          hasNumber,
          hasUpper,
          hasLower,
          hasSpecial,
          length: value.length
        }
      };
    }

    return null;
  }

  /**
   * Custom validator to ensure passwords match
   */
  private passwordMatchValidator(control: AbstractControl): { [key: string]: any } | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }

    return null;
  }

  /**
   * Setup reactive observables for UI state management
   */
  private setupObservables(): void {
    this.isLoading$ = this.store.select(AuthSelectors.selectIsLoading);
    this.isRegistering$ = this.store.select(AuthSelectors.selectIsRegistering);
    this.error$ = this.store.select(AuthSelectors.selectError);
  }

  /**
   * Clear any lingering error messages when component loads
   */
  private clearAnyPreviousErrors(): void {
    this.store.dispatch(AuthActions.clearAuthError());
  }

  /**
   * Listen for successful registration to redirect user
   */
  private listenForSuccessfulRegistration(): void {
    this.store.select(AuthSelectors.selectIsAuthenticated)
      .pipe(takeUntil(this.destroy$))
      .subscribe(isAuthenticated => {
        if (isAuthenticated) {
          this.router.navigate(['/auth/verify-email']);
        }
      });
  }

  /**
   * Handle registration form submission
   */
  onSubmit(): void {
    if (this.registerForm.valid) {
      const formValue = this.registerForm.value;
      
      this.store.dispatch(AuthActions.register({
        email: formValue.email.trim().toLowerCase(),
        password: formValue.password,
        firstName: formValue.firstName.trim(),
        lastName: formValue.lastName.trim(),
        fitnessLevel: formValue.fitnessLevel,
        primaryGoal: formValue.primaryGoal
      }));
    } else {
      this.markFormGroupTouched();
    }
  }

  /**
   * Mark all form fields as touched to show validation errors
   */
  private markFormGroupTouched(): void {
    Object.keys(this.registerForm.controls).forEach(key => {
      const control = this.registerForm.get(key);
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
   * Toggle confirm password visibility
   */
  toggleConfirmPasswordVisibility(): void {
    this.hideConfirmPassword = !this.hideConfirmPassword;
  }

  /**
   * Navigate to login page
   */
  navigateToLogin(): void {
    this.router.navigate(['/auth/login']);
  }

  /**
   * Get user-friendly fitness level display text
   */
  getFitnessLevelDisplay(level: FitnessLevel): string {
    const displays = {
      [FitnessLevel.Beginner]: 'Beginner - Just starting out',
      [FitnessLevel.Intermediate]: 'Intermediate - Some experience',
      [FitnessLevel.Advanced]: 'Advanced - Regular exerciser',
      [FitnessLevel.Expert]: 'Expert - Highly experienced'
    };
    return displays[level];
  }

  /**
   * Get user-friendly fitness goal display text
   */
  getFitnessGoalDisplay(goal: FitnessGoal): string {
    const displays = {
      [FitnessGoal.LoseWeight]: 'Lose Weight - Shed those extra pounds',
      [FitnessGoal.BuildMuscle]: 'Build Muscle - Gain strength and size',
      [FitnessGoal.ImproveEndurance]: 'Improve Endurance - Go the distance',
      [FitnessGoal.StayActive]: 'Stay Active - Maintain a healthy lifestyle',
      [FitnessGoal.CompeteInEvents]: 'Compete in Events - Train for competition',
      [FitnessGoal.Rehabilitation]: 'Rehabilitation - Recover and rebuild'
    };
    return displays[goal];
  }

  /**
   * Get error message for form field validation
   */
  getFieldError(fieldName: string): string {
    const field = this.registerForm.get(fieldName);
    
    if (field?.hasError('required')) {
      return `${this.getFieldDisplayName(fieldName)} is required`;
    }
    
    if (field?.hasError('email')) {
      return 'Please enter a valid email address';
    }
    
    if (field?.hasError('minlength')) {
      const minLength = field.errors?.['minlength'].requiredLength;
      return `${this.getFieldDisplayName(fieldName)} must be at least ${minLength} characters`;
    }
    
    if (field?.hasError('maxlength')) {
      const maxLength = field.errors?.['maxlength'].requiredLength;
      return `${this.getFieldDisplayName(fieldName)} cannot exceed ${maxLength} characters`;
    }
    
    if (field?.hasError('pattern')) {
      if (fieldName === 'firstName' || fieldName === 'lastName') {
        return 'Only letters and spaces are allowed';
      }
    }
    
    if (field?.hasError('passwordStrength')) {
      return this.getPasswordStrengthError(field.errors?.['passwordStrength']);
    }
    
    if (fieldName === 'confirmPassword' && this.registerForm.hasError('passwordMismatch')) {
      return 'Passwords do not match';
    }
    
    return '';
  }

  /**
   * Get specific password strength error message
   */
  private getPasswordStrengthError(strengthInfo: any): string {
    const requirements = [];
    
    if (!strengthInfo.hasUpper) requirements.push('uppercase letter');
    if (!strengthInfo.hasLower) requirements.push('lowercase letter');
    if (!strengthInfo.hasNumber) requirements.push('number');
    if (!strengthInfo.hasSpecial && strengthInfo.length < 12) requirements.push('special character');
    
    if (requirements.length > 0) {
      return `Password needs: ${requirements.join(', ')}`;
    }
    
    return 'Password requirements not met';
  }

  /**
   * Helper to get user-friendly field names for error messages
   */
  private getFieldDisplayName(fieldName: string): string {
    const displayNames: { [key: string]: string } = {
      firstName: 'First name',
      lastName: 'Last name',
      email: 'Email',
      password: 'Password',
      confirmPassword: 'Confirm password'
    };
    return displayNames[fieldName] || fieldName;
  }

  /**
   * Check if field should show error
   */
  shouldShowFieldError(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(field?.invalid && (field?.dirty || field?.touched));
  }

  /**
   * Check if passwords match error should be shown
   */
  shouldShowPasswordMismatch(): boolean {
    const confirmPassword = this.registerForm.get('confirmPassword');
    return !!(this.registerForm.hasError('passwordMismatch') && 
              confirmPassword?.touched);
  }

  /**
   * Get password strength indicator class
   */
  getPasswordStrengthClass(): string {
    const password = this.registerForm.get('password');
    if (!password?.value) return '';
    
    const strength = this.calculatePasswordStrength(password.value);
    return `strength-${strength}`;
  }

  /**
   * Calculate password strength for visual indicator
   */
  private calculatePasswordStrength(password: string): string {
    if (password.length < 6) return 'weak';
    
    const hasNumber = /[0-9]/.test(password);
    const hasUpper = /[A-Z]/.test(password);
    const hasLower = /[a-z]/.test(password);
    const hasSpecial = /[#?!@$%^&*-]/.test(password);
    
    const criteria = [hasNumber, hasUpper, hasLower, hasSpecial].filter(Boolean).length;
    
    if (criteria >= 4 && password.length >= 12) return 'excellent';
    if (criteria >= 3 && password.length >= 10) return 'good';
    if (criteria >= 2 && password.length >= 8) return 'fair';
    return 'weak';
  }
}
