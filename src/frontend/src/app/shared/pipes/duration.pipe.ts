import { Pipe, PipeTransform } from '@angular/core';

/**
 * Duration Pipe - formats time durations in a human-readable way.
 * Think of this as the digital stopwatch display that shows workout times
 * in formats that are easy for gym members to understand.
 */
@Pipe({
  name: 'duration',
  pure: true
})
export class DurationPipe implements PipeTransform {

  /**
   * Transform a duration value into a formatted string.
   * 
   * @param value - Duration in minutes (number) or seconds (if specified)
   * @param format - Output format: 'short', 'long', 'digital', 'compact'
   * @param inputUnit - Input unit: 'minutes', 'seconds', 'hours'
   * @returns Formatted duration string
   * 
   * Examples:
   * - 65 minutes → "1h 5m" (short)
   * - 65 minutes → "1 hour 5 minutes" (long)
   * - 65 minutes → "01:05:00" (digital)
   * - 65 minutes → "1h5m" (compact)
   */
  transform(
    value: number | null | undefined, 
    format: 'short' | 'long' | 'digital' | 'compact' = 'short',
    inputUnit: 'minutes' | 'seconds' | 'hours' = 'minutes'
  ): string {
    
    if (value === null || value === undefined || isNaN(value) || value < 0) {
      return '0m';
    }

    // Convert input to minutes for internal processing
    let totalMinutes: number;
    switch (inputUnit) {
      case 'seconds':
        totalMinutes = value / 60;
        break;
      case 'hours':
        totalMinutes = value * 60;
        break;
      default:
        totalMinutes = value;
    }

    // Extract time components
    const hours = Math.floor(totalMinutes / 60);
    const minutes = Math.floor(totalMinutes % 60);
    const seconds = Math.floor((totalMinutes % 1) * 60);

    // Format based on requested format
    switch (format) {
      case 'long':
        return this.formatLong(hours, minutes, seconds);
      
      case 'digital':
        return this.formatDigital(hours, minutes, seconds);
      
      case 'compact':
        return this.formatCompact(hours, minutes, seconds);
      
      case 'short':
      default:
        return this.formatShort(hours, minutes, seconds);
    }
  }

  /**
   * Format duration in short form (e.g., "1h 30m", "45m", "2h").
   * Like the time display on gym equipment.
   */
  private formatShort(hours: number, minutes: number, seconds: number): string {
    const parts: string[] = [];
    
    if (hours > 0) {
      parts.push(`${hours}h`);
    }
    
    if (minutes > 0 || (hours === 0 && seconds === 0)) {
      parts.push(`${minutes}m`);
    }
    
    // Only show seconds if it's the only component or if we have a very short duration
    if (hours === 0 && minutes === 0 && seconds > 0) {
      parts.push(`${seconds}s`);
    }
    
    return parts.join(' ');
  }

  /**
   * Format duration in long form (e.g., "1 hour 30 minutes").
   * Like detailed workout summaries in progress reports.
   */
  private formatLong(hours: number, minutes: number, seconds: number): string {
    const parts: string[] = [];
    
    if (hours > 0) {
      parts.push(`${hours} ${hours === 1 ? 'hour' : 'hours'}`);
    }
    
    if (minutes > 0) {
      parts.push(`${minutes} ${minutes === 1 ? 'minute' : 'minutes'}`);
    }
    
    if (seconds > 0 && hours === 0) {
      parts.push(`${seconds} ${seconds === 1 ? 'second' : 'seconds'}`);
    }
    
    if (parts.length === 0) {
      return '0 minutes';
    }
    
    // Join with commas and 'and' for the last item
    if (parts.length === 1) {
      return parts[0];
    } else if (parts.length === 2) {
      return parts.join(' and ');
    } else {
      return parts.slice(0, -1).join(', ') + ', and ' + parts[parts.length - 1];
    }
  }

  /**
   * Format duration in digital clock format (e.g., "01:30:45").
   * Like the timer display on treadmills and fitness equipment.
   */
  private formatDigital(hours: number, minutes: number, seconds: number): string {
    if (hours > 0) {
      return `${hours.toString().padStart(2, '0')}:${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    } else {
      return `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;
    }
  }

  /**
   * Format duration in compact form (e.g., "1h30m", "45m").
   * Like space-efficient displays on mobile fitness apps.
   */
  private formatCompact(hours: number, minutes: number, seconds: number): string {
    const parts: string[] = [];
    
    if (hours > 0) {
      parts.push(`${hours}h`);
    }
    
    if (minutes > 0 || (hours > 0 && seconds === 0)) {
      parts.push(`${minutes}m`);
    }
    
    if (hours === 0 && seconds > 0) {
      if (minutes === 0) {
        parts.push(`${seconds}s`);
      } else {
        // Only show seconds for very precise timing
        parts.push(`${seconds}s`);
      }
    }
    
    return parts.join('') || '0m';
  }

  /**
   * Static method for use in components without pipe.
   * Like a utility function for formatting durations in TypeScript code.
   */
  static format(
    minutes: number, 
    format: 'short' | 'long' | 'digital' | 'compact' = 'short'
  ): string {
    const pipe = new DurationPipe();
    return pipe.transform(minutes, format);
  }

  /**
   * Parse a duration string back to minutes.
   * Like reading a time display and converting it back to raw minutes.
   */
  static parse(durationString: string): number {
    if (!durationString) return 0;
    
    let totalMinutes = 0;
    
    // Handle digital format (HH:MM:SS or MM:SS)
    if (durationString.includes(':')) {
      const parts = durationString.split(':').map(p => parseInt(p, 10));
      if (parts.length === 3) {
        // HH:MM:SS
        totalMinutes = parts[0] * 60 + parts[1] + parts[2] / 60;
      } else if (parts.length === 2) {
        // MM:SS
        totalMinutes = parts[0] + parts[1] / 60;
      }
      return totalMinutes;
    }
    
    // Handle text format (1h 30m, 45m, etc.)
    const hourMatch = durationString.match(/(\d+)h/);
    const minuteMatch = durationString.match(/(\d+)m/);
    const secondMatch = durationString.match(/(\d+)s/);
    
    if (hourMatch) {
      totalMinutes += parseInt(hourMatch[1], 10) * 60;
    }
    
    if (minuteMatch) {
      totalMinutes += parseInt(minuteMatch[1], 10);
    }
    
    if (secondMatch) {
      totalMinutes += parseInt(secondMatch[1], 10) / 60;
    }
    
    return totalMinutes;
  }
}
