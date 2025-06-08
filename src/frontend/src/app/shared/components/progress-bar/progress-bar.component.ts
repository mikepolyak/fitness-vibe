import { Component, Input, ChangeDetectionStrategy } from '@angular/core';

/**
 * Progress Bar Component - visual representation of progress toward goals.
 * Think of this as the progress meters you see on fitness equipment,
 * showing how close you are to completing your workout or achieving a goal.
 */
@Component({
  selector: 'fv-progress-bar',
  templateUrl: './progress-bar.component.html',
  styleUrls: ['./progress-bar.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProgressBarComponent {
  /**
   * Current progress value (0-100 or current/max).
   * Like the current number on a step counter.
   */
  @Input() value: number = 0;
  
  /**
   * Maximum value for the progress bar.
   * Like the target number of steps or minutes you're aiming for.
   */
  @Input() max: number = 100;
  
  /**
   * Minimum value for the progress bar.
   * Usually 0, but can be different for specialized cases.
   */
  @Input() min: number = 0;
  
  /**
   * Visual style/theme of the progress bar.
   * Like choosing different colors for different types of goals.
   */
  @Input() color: 'primary' | 'secondary' | 'success' | 'warning' | 'danger' | 'info' = 'primary';
  
  /**
   * Size of the progress bar.
   * Like choosing between a thin line or a thick bar for visibility.
   */
  @Input() size: 'xs' | 'sm' | 'md' | 'lg' | 'xl' = 'md';
  
  /**
   * Whether to show percentage text inside or near the bar.
   * Like displaying "75%" on your progress meter.
   */
  @Input() showPercentage: boolean = false;
  
  /**
   * Whether to show current/max values (e.g., "750/1000").
   * Like showing "7,500 / 10,000 steps".
   */
  @Input() showValues: boolean = false;
  
  /**
   * Custom label for the progress bar.
   * Like "Daily Steps Goal" or "Workout Progress".
   */
  @Input() label: string = '';
  
  /**
   * Position of the label relative to the bar.
   */
  @Input() labelPosition: 'top' | 'bottom' | 'inline' = 'top';
  
  /**
   * Whether to animate the progress bar.
   * Like a smooth filling animation when progress increases.
   */
  @Input() animated: boolean = true;
  
  /**
   * Whether to show a striped pattern (for indeterminate progress).
   * Like a loading bar with moving stripes.
   */
  @Input() striped: boolean = false;
  
  /**
   * Custom suffix for values (e.g., "steps", "minutes", "km").
   * Like adding units to make the numbers meaningful.
   */
  @Input() valueSuffix: string = '';
  
  /**
   * Whether the progress bar is in an indeterminate state.
   * Like showing activity without a specific percentage.
   */
  @Input() indeterminate: boolean = false;
  
  /**
   * Custom CSS classes to apply.
   */
  @Input() customClass: string = '';
  
  /**
   * Whether to show milestone markers on the bar.
   * Like showing quarter-marks on a progress bar.
   */
  @Input() showMilestones: boolean = false;
  
  /**
   * Milestone values to display as markers.
   */
  @Input() milestones: number[] = [25, 50, 75];

  /**
   * Calculate the progress percentage.
   * Like converting raw numbers to a percentage for display.
   */
  get progressPercentage(): number {
    if (this.indeterminate) return 0;
    
    const range = this.max - this.min;
    if (range <= 0) return 0;
    
    const adjustedValue = Math.max(this.min, Math.min(this.max, this.value));
    const progress = ((adjustedValue - this.min) / range) * 100;
    
    return Math.round(progress * 100) / 100; // Round to 2 decimal places
  }

  /**
   * Get CSS classes for the progress bar container.
   * Like choosing the right styling based on size and color.
   */
  get progressBarClasses(): string {
    const classes = [
      'progress-bar',
      `progress-bar--${this.size}`,
      `progress-bar--${this.color}`,
      this.customClass
    ];
    
    if (this.animated) {
      classes.push('progress-bar--animated');
    }
    
    if (this.striped) {
      classes.push('progress-bar--striped');
    }
    
    if (this.indeterminate) {
      classes.push('progress-bar--indeterminate');
    }
    
    if (this.showMilestones && this.milestones.length > 0) {
      classes.push('progress-bar--with-milestones');
    }
    
    return classes.filter(Boolean).join(' ');
  }

  /**
   * Get the display text for current/max values.
   * Like showing "7,500 / 10,000 steps" in a readable format.
   */
  get valuesText(): string {
    const currentFormatted = this.formatNumber(this.value);
    const maxFormatted = this.formatNumber(this.max);
    const suffix = this.valueSuffix ? ` ${this.valueSuffix}` : '';
    
    return `${currentFormatted} / ${maxFormatted}${suffix}`;
  }

  /**
   * Get the display text for percentage.
   * Like showing "75%" in a clean format.
   */
  get percentageText(): string {
    return `${Math.round(this.progressPercentage)}%`;
  }

  /**
   * Check if the goal is completed.
   * Like determining if someone has reached their target.
   */
  get isCompleted(): boolean {
    return this.value >= this.max;
  }

  /**
   * Get milestone positions for display.
   * Like calculating where to put quarter-marks on the bar.
   */
  get milestonePositions(): Array<{ position: number; value: number; reached: boolean }> {
    if (!this.showMilestones) return [];
    
    return this.milestones.map(milestone => ({
      position: ((milestone - this.min) / (this.max - this.min)) * 100,
      value: milestone,
      reached: this.value >= milestone
    })).filter(m => m.position >= 0 && m.position <= 100);
  }

  /**
   * Format numbers for display.
   * Like making large numbers readable with appropriate formatting.
   */
  private formatNumber(value: number): string {
    if (value >= 1000000) {
      return (value / 1000000).toFixed(1) + 'M';
    } else if (value >= 1000) {
      return (value / 1000).toFixed(1) + 'K';
    } else {
      return value.toLocaleString();
    }
  }

  /**
   * Get motivational message based on progress.
   * Like encouraging words from a personal trainer.
   */
  get motivationalMessage(): string {
    const percentage = this.progressPercentage;
    
    if (percentage >= 100) {
      return '🎉 Goal achieved! Fantastic work!';
    } else if (percentage >= 75) {
      return '🔥 Almost there! Keep pushing!';
    } else if (percentage >= 50) {
      return '💪 Halfway done! You\'re doing great!';
    } else if (percentage >= 25) {
      return '🚀 Great start! Keep the momentum!';
    } else if (percentage > 0) {
      return '✨ Every step counts! You\'ve got this!';
    } else {
      return '🎯 Ready to start your journey?';
    }
  }

  /**
   * Get the estimated time to completion based on current rate.
   * Like predicting when you'll reach your goal at the current pace.
   */
  getEstimatedCompletion(ratePerHour?: number): string {
    if (!ratePerHour || this.value >= this.max) {
      return '';
    }
    
    const remaining = this.max - this.value;
    const hoursRemaining = remaining / ratePerHour;
    
    if (hoursRemaining < 1) {
      const minutes = Math.round(hoursRemaining * 60);
      return `~${minutes} min remaining`;
    } else if (hoursRemaining < 24) {
      const hours = Math.round(hoursRemaining);
      return `~${hours} hour${hours !== 1 ? 's' : ''} remaining`;
    } else {
      const days = Math.round(hoursRemaining / 24);
      return `~${days} day${days !== 1 ? 's' : ''} remaining`;
    }
  }

  /**
   * Get progress category for styling and messaging.
   * Like categorizing progress levels for different treatments.
   */
  get progressCategory(): 'started' | 'progressing' | 'nearly-done' | 'completed' {
    const percentage = this.progressPercentage;
    
    if (percentage >= 100) return 'completed';
    if (percentage >= 75) return 'nearly-done';
    if (percentage >= 25) return 'progressing';
    return 'started';
  }
}
