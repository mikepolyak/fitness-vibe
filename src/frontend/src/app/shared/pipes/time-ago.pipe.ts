import { Pipe, PipeTransform } from '@angular/core';

/**
 * Time Ago Pipe - shows how much time has passed since a given date.
 * Think of this as the "posted X minutes ago" display on social media,
 * but for fitness activities and achievements.
 */
@Pipe({
  name: 'timeAgo',
  pure: false // Not pure because time changes constantly
})
export class TimeAgoPipe implements PipeTransform {

  /**
   * Transform a date into a relative time string.
   * 
   * @param value - Date to compare against current time
   * @param format - Output format: 'short', 'long', 'precise'
   * @param showSuffix - Whether to include "ago" suffix
   * @returns Formatted time ago string
   * 
   * Examples:
   * - 2 minutes ago → "2m" (short)
   * - 2 minutes ago → "2 minutes ago" (long)
   * - 2 minutes ago → "2 min ago" (precise)
   */
  transform(
    value: Date | string | number | null | undefined,
    format: 'short' | 'long' | 'precise' = 'long',
    showSuffix: boolean = true
  ): string {

    if (!value) {
      return format === 'short' ? 'now' : 'just now';
    }

    const date = this.parseDate(value);
    if (!date || isNaN(date.getTime())) {
      return 'unknown';
    }

    const now = new Date();
    const diffInMs = now.getTime() - date.getTime();
    
    // Handle future dates
    if (diffInMs < 0) {
      return this.formatFuture(-diffInMs, format, showSuffix);
    }

    return this.formatPast(diffInMs, format, showSuffix);
  }

  /**
   * Parse various date formats into a Date object.
   * Like a universal date reader that understands different formats.
   */
  private parseDate(value: Date | string | number): Date {
    if (value instanceof Date) {
      return value;
    }
    
    if (typeof value === 'number') {
      return new Date(value);
    }
    
    if (typeof value === 'string') {
      return new Date(value);
    }
    
    return new Date();
  }

  /**
   * Format time that has passed (past dates).
   * Like showing "completed 30 minutes ago" for workouts.
   */
  private formatPast(diffInMs: number, format: string, showSuffix: boolean): string {
    const seconds = Math.floor(diffInMs / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);
    const weeks = Math.floor(days / 7);
    const months = Math.floor(days / 30);
    const years = Math.floor(days / 365);

    let timeValue: number;
    let timeUnit: string;

    if (years > 0) {
      timeValue = years;
      timeUnit = 'year';
    } else if (months > 0) {
      timeValue = months;
      timeUnit = 'month';
    } else if (weeks > 0) {
      timeValue = weeks;
      timeUnit = 'week';
    } else if (days > 0) {
      timeValue = days;
      timeUnit = 'day';
    } else if (hours > 0) {
      timeValue = hours;
      timeUnit = 'hour';
    } else if (minutes > 0) {
      timeValue = minutes;
      timeUnit = 'minute';
    } else {
      timeValue = Math.max(1, seconds);
      timeUnit = 'second';
    }

    return this.formatTimeString(timeValue, timeUnit, format, showSuffix, false);
  }

  /**
   * Format time in the future.
   * Like showing "starts in 2 hours" for scheduled workouts.
   */
  private formatFuture(diffInMs: number, format: string, showSuffix: boolean): string {
    const result = this.formatPast(diffInMs, format, false);
    
    if (format === 'short') {
      return `in ${result}`;
    } else {
      return showSuffix ? `in ${result}` : result;
    }
  }

  /**
   * Format the time string based on the specified format.
   * Like choosing between different display styles for time.
   */
  private formatTimeString(
    value: number, 
    unit: string, 
    format: string, 
    showSuffix: boolean, 
    isFuture: boolean
  ): string {
    
    switch (format) {
      case 'short':
        return this.formatShort(value, unit, showSuffix);
      
      case 'precise':
        return this.formatPrecise(value, unit, showSuffix);
      
      case 'long':
      default:
        return this.formatLong(value, unit, showSuffix);
    }
  }

  /**
   * Format in short form (e.g., "2m", "1h", "3d").
   * Like compact time displays on mobile fitness apps.
   */
  private formatShort(value: number, unit: string, showSuffix: boolean): string {
    const shortUnits: { [key: string]: string } = {
      'second': 's',
      'minute': 'm',
      'hour': 'h',
      'day': 'd',
      'week': 'w',
      'month': 'mo',
      'year': 'y'
    };

    const shortUnit = shortUnits[unit] || unit.charAt(0);
    return `${value}${shortUnit}`;
  }

  /**
   * Format in precise form (e.g., "2 min ago", "1 hr ago").
   * Like abbreviated but clear time displays.
   */
  private formatPrecise(value: number, unit: string, showSuffix: boolean): string {
    const preciseUnits: { [key: string]: string } = {
      'second': 'sec',
      'minute': 'min',
      'hour': 'hr',
      'day': 'day',
      'week': 'wk',
      'month': 'mo',
      'year': 'yr'
    };

    const preciseUnit = preciseUnits[unit] || unit;
    const pluralUnit = value === 1 ? preciseUnit : preciseUnit + (preciseUnit.endsWith('s') ? '' : 's');
    const suffix = showSuffix ? ' ago' : '';
    
    return `${value} ${pluralUnit}${suffix}`;
  }

  /**
   * Format in long form (e.g., "2 minutes ago", "1 hour ago").
   * Like full, natural language time descriptions.
   */
  private formatLong(value: number, unit: string, showSuffix: boolean): string {
    const pluralUnit = value === 1 ? unit : unit + 's';
    const suffix = showSuffix ? ' ago' : '';
    
    return `${value} ${pluralUnit}${suffix}`;
  }

  /**
   * Get relative time with smart precision.
   * Like choosing the most appropriate time unit automatically.
   */
  static getSmartTimeAgo(date: Date, now: Date = new Date()): string {
    const diffInMs = now.getTime() - date.getTime();
    const seconds = Math.floor(diffInMs / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);
    const days = Math.floor(hours / 24);

    if (seconds < 60) {
      return 'just now';
    } else if (minutes < 60) {
      return `${minutes}m ago`;
    } else if (hours < 24) {
      return `${hours}h ago`;
    } else if (days < 7) {
      return `${days}d ago`;
    } else {
      // For older dates, show actual date
      return date.toLocaleDateString(undefined, { 
        month: 'short', 
        day: 'numeric' 
      });
    }
  }

  /**
   * Check if a date is today.
   * Like determining if an activity happened today.
   */
  static isToday(date: Date, now: Date = new Date()): boolean {
    return date.toDateString() === now.toDateString();
  }

  /**
   * Check if a date is yesterday.
   * Like determining if an activity happened yesterday.
   */
  static isYesterday(date: Date, now: Date = new Date()): boolean {
    const yesterday = new Date(now);
    yesterday.setDate(yesterday.getDate() - 1);
    return date.toDateString() === yesterday.toDateString();
  }

  /**
   * Get a contextual time description.
   * Like providing context-aware time descriptions for fitness activities.
   */
  static getContextualTime(date: Date, context: 'activity' | 'goal' | 'achievement' = 'activity'): string {
    const now = new Date();
    
    if (this.isToday(date, now)) {
      return context === 'activity' ? 'Today' : 'Set today';
    }
    
    if (this.isYesterday(date, now)) {
      return context === 'activity' ? 'Yesterday' : 'Set yesterday';
    }
    
    const diffInMs = now.getTime() - date.getTime();
    const days = Math.floor(diffInMs / (1000 * 60 * 60 * 24));
    
    if (days < 7) {
      const dayName = date.toLocaleDateString(undefined, { weekday: 'long' });
      return context === 'activity' ? dayName : `Set on ${dayName}`;
    }
    
    if (days < 30) {
      const weeks = Math.floor(days / 7);
      return context === 'activity' 
        ? `${weeks} week${weeks > 1 ? 's' : ''} ago`
        : `Set ${weeks} week${weeks > 1 ? 's' : ''} ago`;
    }
    
    return date.toLocaleDateString(undefined, {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  /**
   * Get workout timing context.
   * Like showing when during the day a workout happened.
   */
  static getWorkoutTimingContext(date: Date): string {
    const hour = date.getHours();
    
    if (hour < 6) {
      return 'Late night workout 🌙';
    } else if (hour < 12) {
      return 'Morning workout 🌅';
    } else if (hour < 17) {
      return 'Afternoon workout ☀️';
    } else if (hour < 21) {
      return 'Evening workout 🌆';
    } else {
      return 'Night workout 🌃';
    }
  }

  /**
   * Static method for direct use in components.
   */
  static format(
    date: Date,
    format: 'short' | 'long' | 'precise' = 'long',
    showSuffix: boolean = true
  ): string {
    const pipe = new TimeAgoPipe();
    return pipe.transform(date, format, showSuffix);
  }
}
