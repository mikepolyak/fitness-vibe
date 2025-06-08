import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig, MatSnackBarRef, SimpleSnackBar } from '@angular/material/snack-bar';
import { ComponentType } from '@angular/cdk/portal';

/**
 * Notification Service - the communication system for our fitness app.
 * Think of this as the gym's announcement system that keeps members
 * informed about achievements, updates, and important information.
 */
@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private readonly defaultConfig: MatSnackBarConfig = {
    duration: 4000,
    horizontalPosition: 'end',
    verticalPosition: 'top',
  };

  constructor(private snackBar: MatSnackBar) {}

  /**
   * Show success notification.
   * Like announcing an achievement over the gym's PA system.
   */
  showSuccess(message: string, action?: string, config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const successConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      panelClass: ['success-snackbar'],
      duration: config?.duration ?? 5000
    };

    return this.snackBar.open(message, action || 'Close', successConfig);
  }

  /**
   * Show error notification.
   * Like alerting members about important issues that need attention.
   */
  showError(message: string, action?: string, config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const errorConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      panelClass: ['error-snackbar'],
      duration: config?.duration ?? 8000 // Longer duration for errors
    };

    return this.snackBar.open(message, action || 'Dismiss', errorConfig);
  }

  /**
   * Show warning notification.
   * Like giving members a heads-up about something they should know.
   */
  showWarning(message: string, action?: string, config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const warningConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      panelClass: ['warning-snackbar'],
      duration: config?.duration ?? 6000
    };

    return this.snackBar.open(message, action || 'OK', warningConfig);
  }

  /**
   * Show info notification.
   * Like sharing helpful tips or updates with gym members.
   */
  showInfo(message: string, action?: string, config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const infoConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      panelClass: ['info-snackbar'],
      duration: config?.duration ?? 4000
    };

    return this.snackBar.open(message, action || 'Got it', infoConfig);
  }

  /**
   * Show achievement notification with special styling.
   * Like celebrating a member's milestone with fanfare.
   */
  showAchievement(message: string, action?: string, config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const achievementConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      panelClass: ['achievement-snackbar'],
      duration: config?.duration ?? 7000 // Longer duration for achievements
    };

    return this.snackBar.open(`🎉 ${message}`, action || 'Awesome!', achievementConfig);
  }

  /**
   * Show level up notification.
   * Like announcing when someone reaches a new fitness level.
   */
  showLevelUp(newLevel: number, action?: string, config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const levelUpConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      panelClass: ['level-up-snackbar'],
      duration: config?.duration ?? 8000
    };

    const message = `🚀 Congratulations! You've reached Level ${newLevel}!`;
    return this.snackBar.open(message, action || 'Keep Going!', levelUpConfig);
  }

  /**
   * Show motivation notification.
   * Like a personal trainer giving encouraging words.
   */
  showMotivation(message: string, action?: string, config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const motivationConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      panelClass: ['motivation-snackbar'],
      duration: config?.duration ?? 6000
    };

    return this.snackBar.open(`💪 ${message}`, action || 'Let\'s Go!', motivationConfig);
  }

  /**
   * Show streak notification.
   * Like celebrating consecutive workout days.
   */
  showStreak(days: number, action?: string, config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const streakConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      panelClass: ['streak-snackbar'],
      duration: config?.duration ?? 6000
    };

    const message = `🔥 Amazing! You're on a ${days}-day streak!`;
    return this.snackBar.open(message, action || 'Keep it up!', streakConfig);
  }

  /**
   * Show custom notification with component.
   * Like displaying a detailed announcement with custom formatting.
   */
  showCustom<T>(component: ComponentType<T>, config?: MatSnackBarConfig): MatSnackBarRef<T> {
    const customConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config
    };

    return this.snackBar.openFromComponent(component, customConfig);
  }

  /**
   * Show persistent notification that requires user action.
   * Like important announcements that members must acknowledge.
   */
  showPersistent(message: string, action: string = 'OK'): MatSnackBarRef<SimpleSnackBar> {
    const persistentConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      duration: 0, // No auto-dismiss
      panelClass: ['persistent-snackbar']
    };

    return this.snackBar.open(message, action, persistentConfig);
  }

  /**
   * Show quick toast notification.
   * Like a brief status update that doesn't interrupt the user.
   */
  showQuick(message: string, config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const quickConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      duration: 2000, // Short duration
      panelClass: ['quick-snackbar']
    };

    return this.snackBar.open(message, '', quickConfig);
  }

  /**
   * Show loading notification.
   * Like telling members that something is processing in the background.
   */
  showLoading(message: string = 'Processing...', config?: MatSnackBarConfig): MatSnackBarRef<SimpleSnackBar> {
    const loadingConfig: MatSnackBarConfig = {
      ...this.defaultConfig,
      ...config,
      duration: 0, // No auto-dismiss
      panelClass: ['loading-snackbar']
    };

    return this.snackBar.open(message, '', loadingConfig);
  }

  /**
   * Dismiss all current notifications.
   * Like turning off the announcement system.
   */
  dismissAll(): void {
    this.snackBar.dismiss();
  }

  /**
   * Show offline notification.
   * Like alerting members when internet connection is lost.
   */
  showOffline(): MatSnackBarRef<SimpleSnackBar> {
    return this.showWarning(
      'You are currently offline. Some features may not be available.',
      'Retry',
      { duration: 0 }
    );
  }

  /**
   * Show online notification.
   * Like welcoming members back when connection is restored.
   */
  showOnline(): MatSnackBarRef<SimpleSnackBar> {
    return this.showSuccess(
      'Connection restored! All features are now available.',
      'Great',
      { duration: 3000 }
    );
  }

  /**
   * Show update available notification.
   * Like informing members about new gym features or equipment.
   */
  showUpdateAvailable(): MatSnackBarRef<SimpleSnackBar> {
    return this.showInfo(
      'A new version of FitnessVibe is available!',
      'Update',
      { duration: 0 }
    );
  }

  /**
   * Show goal reminder notification.
   * Like a personal trainer reminding you about your fitness goals.
   */
  showGoalReminder(goalTitle: string, progress: number): MatSnackBarRef<SimpleSnackBar> {
    const message = `🎯 Remember your goal: ${goalTitle} (${progress.toFixed(0)}% complete)`;
    return this.showMotivation(message, 'View Goal', { duration: 6000 });
  }

  /**
   * Show workout reminder notification.
   * Like a friendly nudge to get moving.
   */
  showWorkoutReminder(): MatSnackBarRef<SimpleSnackBar> {
    const motivationalMessages = [
      "Time to get moving! Your body will thank you later.",
      "Ready to crush your fitness goals today?",
      "Every workout counts! Let's make today great.",
      "Your future self is counting on you!",
      "Turn your 'I can't' into 'I can' and your dreams into plans!"
    ];
    
    const randomMessage = motivationalMessages[Math.floor(Math.random() * motivationalMessages.length)];
    return this.showMotivation(randomMessage, 'Start Workout', { duration: 8000 });
  }
}
