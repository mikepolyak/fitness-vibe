import { Component, Input, ChangeDetectionStrategy } from '@angular/core';

/**
 * Loading Spinner Component - the visual indicator for ongoing activities.
 * Think of this as the "equipment in use" light that shows when something
 * is actively happening in our fitness app.
 */
@Component({
  selector: 'fv-loading-spinner',
  templateUrl: './loading-spinner.component.html',
  styleUrls: ['./loading-spinner.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class LoadingSpinnerComponent {
  /**
   * Size of the spinner - like choosing between a small indicator light
   * and a big "processing" sign.
   */
  @Input() size: 'small' | 'medium' | 'large' = 'medium';
  
  /**
   * Message to display with the spinner.
   * Like the text that explains what's currently happening.
   */
  @Input() message: string = '';
  
  /**
   * Whether to show the spinner overlay.
   * Like deciding if the loading indicator should cover the whole area.
   */
  @Input() overlay: boolean = false;
  
  /**
   * Color theme for the spinner.
   * Like choosing the right visual style for different contexts.
   */
  @Input() color: 'primary' | 'accent' | 'warn' = 'primary';
  
  /**
   * Whether to center the spinner.
   * Like deciding where to place the "in use" sign.
   */
  @Input() centered: boolean = true;

  /**
   * Get CSS classes for the spinner based on inputs.
   * Like assembling the right combination of visual styles.
   */
  get spinnerClasses(): string {
    const classes = [
      'loading-spinner',
      `loading-spinner--${this.size}`,
      `loading-spinner--${this.color}`
    ];
    
    if (this.centered) {
      classes.push('loading-spinner--centered');
    }
    
    if (this.overlay) {
      classes.push('loading-spinner--overlay');
    }
    
    return classes.join(' ');
  }

  /**
   * Get the diameter for Material spinner based on size.
   * Like choosing the right size indicator light for the situation.
   */
  get spinnerDiameter(): number {
    switch (this.size) {
      case 'small': return 20;
      case 'large': return 60;
      default: return 40;
    }
  }
}
