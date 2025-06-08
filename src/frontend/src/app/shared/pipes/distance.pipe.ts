import { Pipe, PipeTransform } from '@angular/core';

/**
 * Distance Pipe - formats distance measurements for fitness activities.
 * Think of this as the distance display on fitness equipment that automatically
 * chooses the best unit and precision for easy reading.
 */
@Pipe({
  name: 'distance',
  pure: true
})
export class DistancePipe implements PipeTransform {

  /**
   * Transform a distance value into a formatted string.
   * 
   * @param value - Distance in kilometers (default) or specified input unit
   * @param unit - Target unit system: 'metric', 'imperial', 'auto'
   * @param precision - Number of decimal places (auto-calculated if not specified)
   * @param inputUnit - Input unit: 'km', 'meters', 'miles', 'feet'
   * @param showUnit - Whether to include the unit in the output
   * @returns Formatted distance string
   * 
   * Examples:
   * - 5.5 km → "5.5 km" (metric)
   * - 5.5 km → "3.4 mi" (imperial)
   * - 0.8 km → "800 m" (auto metric)
   * - 26.2 miles → "42.2 km" (input miles to metric)
   */
  transform(
    value: number | null | undefined,
    unit: 'metric' | 'imperial' | 'auto' = 'auto',
    precision?: number,
    inputUnit: 'km' | 'meters' | 'miles' | 'feet' = 'km',
    showUnit: boolean = true
  ): string {

    if (value === null || value === undefined || isNaN(value) || value < 0) {
      return showUnit ? '0 m' : '0';
    }

    // Convert input to kilometers for internal processing
    let distanceInKm = this.convertToKilometers(value, inputUnit);

    // Determine target unit system
    const targetUnit = unit === 'auto' ? this.autoSelectUnit(distanceInKm) : unit;

    // Format based on target unit
    if (targetUnit === 'imperial') {
      return this.formatImperial(distanceInKm, precision, showUnit);
    } else {
      return this.formatMetric(distanceInKm, precision, showUnit);
    }
  }

  /**
   * Convert input value to kilometers.
   * Like normalizing all distance measurements to a common standard.
   */
  private convertToKilometers(value: number, inputUnit: string): number {
    switch (inputUnit) {
      case 'meters':
        return value / 1000;
      case 'miles':
        return value * 1.60934;
      case 'feet':
        return value * 0.0003048;
      case 'km':
      default:
        return value;
    }
  }

  /**
   * Auto-select the most appropriate unit system.
   * Like choosing between showing distance in miles vs feet based on magnitude.
   */
  private autoSelectUnit(distanceInKm: number): 'metric' | 'imperial' {
    // Default to metric for international appeal
    // In a real app, this could check user preferences or locale
    return 'metric';
  }

  /**
   * Format distance in metric units (km/m).
   * Like the display on European fitness equipment.
   */
  private formatMetric(distanceInKm: number, precision?: number, showUnit: boolean = true): string {
    let distance: number;
    let unit: string;
    let calculatedPrecision: number;

    if (distanceInKm >= 1) {
      // Use kilometers for distances >= 1 km
      distance = distanceInKm;
      unit = 'km';
      calculatedPrecision = precision ?? (distanceInKm >= 10 ? 1 : 2);
    } else {
      // Use meters for distances < 1 km
      distance = distanceInKm * 1000;
      unit = 'm';
      calculatedPrecision = precision ?? 0;
    }

    const formattedDistance = this.formatNumber(distance, calculatedPrecision);
    return showUnit ? `${formattedDistance} ${unit}` : formattedDistance;
  }

  /**
   * Format distance in imperial units (mi/ft/yd).
   * Like the display on American fitness equipment.
   */
  private formatImperial(distanceInKm: number, precision?: number, showUnit: boolean = true): string {
    const distanceInMiles = distanceInKm * 0.621371;
    const distanceInFeet = distanceInKm * 3280.84;
    const distanceInYards = distanceInFeet / 3;

    let distance: number;
    let unit: string;
    let calculatedPrecision: number;

    if (distanceInMiles >= 0.1) {
      // Use miles for longer distances
      distance = distanceInMiles;
      unit = 'mi';
      calculatedPrecision = precision ?? (distanceInMiles >= 10 ? 1 : 2);
    } else if (distanceInYards >= 10) {
      // Use yards for medium distances
      distance = distanceInYards;
      unit = 'yd';
      calculatedPrecision = precision ?? 0;
    } else {
      // Use feet for short distances
      distance = distanceInFeet;
      unit = 'ft';
      calculatedPrecision = precision ?? 0;
    }

    const formattedDistance = this.formatNumber(distance, calculatedPrecision);
    return showUnit ? `${formattedDistance} ${unit}` : formattedDistance;
  }

  /**
   * Format a number with appropriate precision and thousands separators.
   * Like ensuring distance displays are easy to read and understand.
   */
  private formatNumber(value: number, precision: number): string {
    const options: Intl.NumberFormatOptions = {
      minimumFractionDigits: 0,
      maximumFractionDigits: precision
    };

    return new Intl.NumberFormat('en-US', options).format(value);
  }

  /**
   * Get the pace per distance unit (minutes per km or mile).
   * Like calculating running pace from total time and distance.
   */
  static calculatePace(totalMinutes: number, distanceKm: number, unit: 'metric' | 'imperial' = 'metric'): string {
    if (distanceKm <= 0 || totalMinutes <= 0) {
      return unit === 'imperial' ? '0:00 /mi' : '0:00 /km';
    }

    let paceMinutes: number;
    let unitLabel: string;

    if (unit === 'imperial') {
      const distanceInMiles = distanceKm * 0.621371;
      paceMinutes = totalMinutes / distanceInMiles;
      unitLabel = '/mi';
    } else {
      paceMinutes = totalMinutes / distanceKm;
      unitLabel = '/km';
    }

    const minutes = Math.floor(paceMinutes);
    const seconds = Math.round((paceMinutes - minutes) * 60);

    return `${minutes}:${seconds.toString().padStart(2, '0')} ${unitLabel}`;
  }

  /**
   * Calculate speed from distance and time.
   * Like computing average speed during a workout.
   */
  static calculateSpeed(distanceKm: number, totalMinutes: number, unit: 'metric' | 'imperial' = 'metric'): string {
    if (distanceKm <= 0 || totalMinutes <= 0) {
      return unit === 'imperial' ? '0.0 mph' : '0.0 km/h';
    }

    const hours = totalMinutes / 60;

    if (unit === 'imperial') {
      const distanceInMiles = distanceKm * 0.621371;
      const speedMph = distanceInMiles / hours;
      return `${speedMph.toFixed(1)} mph`;
    } else {
      const speedKmh = distanceKm / hours;
      return `${speedKmh.toFixed(1)} km/h`;
    }
  }

  /**
   * Convert between different distance units.
   * Like a universal distance converter for fitness data.
   */
  static convert(
    value: number,
    fromUnit: 'km' | 'meters' | 'miles' | 'feet' | 'yards',
    toUnit: 'km' | 'meters' | 'miles' | 'feet' | 'yards'
  ): number {
    // Convert to meters as intermediate unit
    let valueInMeters: number;

    switch (fromUnit) {
      case 'km':
        valueInMeters = value * 1000;
        break;
      case 'miles':
        valueInMeters = value * 1609.34;
        break;
      case 'feet':
        valueInMeters = value * 0.3048;
        break;
      case 'yards':
        valueInMeters = value * 0.9144;
        break;
      case 'meters':
      default:
        valueInMeters = value;
    }

    // Convert from meters to target unit
    switch (toUnit) {
      case 'km':
        return valueInMeters / 1000;
      case 'miles':
        return valueInMeters / 1609.34;
      case 'feet':
        return valueInMeters / 0.3048;
      case 'yards':
        return valueInMeters / 0.9144;
      case 'meters':
      default:
        return valueInMeters;
    }
  }

  /**
   * Format elevation gain/loss with appropriate units.
   * Like displaying the hills climbed during a hiking workout.
   */
  static formatElevation(
    elevationMeters: number,
    unit: 'metric' | 'imperial' = 'metric',
    showUnit: boolean = true
  ): string {
    if (unit === 'imperial') {
      const elevationFeet = elevationMeters * 3.28084;
      const formatted = Math.round(elevationFeet).toLocaleString();
      return showUnit ? `${formatted} ft` : formatted;
    } else {
      const formatted = Math.round(elevationMeters).toLocaleString();
      return showUnit ? `${formatted} m` : formatted;
    }
  }

  /**
   * Static method for direct use in components.
   * Like a utility function for formatting distances in TypeScript code.
   */
  static format(
    distanceKm: number,
    unit: 'metric' | 'imperial' | 'auto' = 'auto',
    precision?: number,
    showUnit: boolean = true
  ): string {
    const pipe = new DistancePipe();
    return pipe.transform(distanceKm, unit, precision, 'km', showUnit);
  }
}
