import { Pipe, PipeTransform } from '@angular/core';

/**
 * Calories Pipe - formats calorie values for fitness activities.
 * Think of this as the calorie counter display on fitness equipment
 * that shows energy burned in an easy-to-read format.
 */
@Pipe({
  name: 'calories',
  pure: true
})
export class CaloriesPipe implements PipeTransform {

  /**
   * Transform a calorie value into a formatted string.
   * 
   * @param value - Calories as a number
   * @param format - Display format: 'short', 'long', 'detailed'
   * @param showUnit - Whether to include 'cal' or 'kcal' in output
   * @param precision - Number of decimal places for display
   * @returns Formatted calorie string
   * 
   * Examples:
   * - 1234 → "1,234 cal" (short)
   * - 1234 → "1,234 calories" (long)
   * - 1234 → "1.2k cal" (detailed for large numbers)
   */
  transform(
    value: number | null | undefined,
    format: 'short' | 'long' | 'detailed' = 'short',
    showUnit: boolean = true,
    precision?: number
  ): string {

    if (value === null || value === undefined || isNaN(value) || value < 0) {
      return showUnit ? '0 cal' : '0';
    }

    switch (format) {
      case 'long':
        return this.formatLong(value, showUnit, precision);
      case 'detailed':
        return this.formatDetailed(value, showUnit, precision);
      case 'short':
      default:
        return this.formatShort(value, showUnit, precision);
    }
  }

  /**
   * Format calories in short form (e.g., "1,234 cal").
   * Like the basic calorie display on most fitness equipment.
   */
  private formatShort(value: number, showUnit: boolean, precision?: number): string {
    const calculatedPrecision = precision ?? (value >= 100 ? 0 : 1);
    const formattedValue = this.formatNumber(value, calculatedPrecision);
    
    if (!showUnit) return formattedValue;
    
    // Use 'kcal' for values over 1000, 'cal' for smaller values
    const unit = value >= 1000 ? 'kcal' : 'cal';
    const displayValue = value >= 1000 ? value / 1000 : value;
    const finalFormatted = this.formatNumber(displayValue, value >= 1000 ? 1 : calculatedPrecision);
    
    return `${finalFormatted} ${unit}`;
  }

  /**
   * Format calories in long form (e.g., "1,234 calories").
   * Like detailed workout summaries and progress reports.
   */
  private formatLong(value: number, showUnit: boolean, precision?: number): string {
    const calculatedPrecision = precision ?? 0;
    const formattedValue = this.formatNumber(value, calculatedPrecision);
    
    if (!showUnit) return formattedValue;
    
    const unit = value === 1 ? 'calorie' : 'calories';
    return `${formattedValue} ${unit}`;
  }

  /**
   * Format calories with smart abbreviation for large numbers.
   * Like space-efficient displays on mobile fitness apps.
   */
  private formatDetailed(value: number, showUnit: boolean, precision?: number): string {
    let displayValue: number;
    let suffix: string;
    let calculatedPrecision: number;

    if (value >= 1000000) {
      displayValue = value / 1000000;
      suffix = 'M';
      calculatedPrecision = precision ?? 1;
    } else if (value >= 1000) {
      displayValue = value / 1000;
      suffix = 'k';
      calculatedPrecision = precision ?? 1;
    } else {
      displayValue = value;
      suffix = '';
      calculatedPrecision = precision ?? 0;
    }

    const formattedValue = this.formatNumber(displayValue, calculatedPrecision);
    const unit = showUnit ? ' cal' : '';
    
    return `${formattedValue}${suffix}${unit}`;
  }

  /**
   * Format a number with appropriate precision and thousands separators.
   */
  private formatNumber(value: number, precision: number): string {
    const options: Intl.NumberFormatOptions = {
      minimumFractionDigits: 0,
      maximumFractionDigits: precision
    };

    return new Intl.NumberFormat('en-US', options).format(value);
  }

  /**
   * Calculate calories burned based on MET value, weight, and duration.
   * Like the formula used by fitness equipment to estimate energy expenditure.
   */
  static calculateCalories(metValue: number, weightKg: number, durationMinutes: number): number {
    // Standard formula: Calories = MET × weight(kg) × time(hours)
    const hours = durationMinutes / 60;
    return Math.round(metValue * weightKg * hours);
  }

  /**
   * Get calorie burn rate per minute.
   * Like showing real-time calorie burn during exercise.
   */
  static getCalorieRate(totalCalories: number, durationMinutes: number): number {
    if (durationMinutes <= 0) return 0;
    return totalCalories / durationMinutes;
  }

  /**
   * Convert calories to other energy units.
   * Like showing energy expenditure in different measurements.
   */
  static convertCalories(
    calories: number,
    toUnit: 'kilojoules' | 'watts' | 'foodCalories'
  ): number {
    switch (toUnit) {
      case 'kilojoules':
        return calories * 4.184; // 1 cal = 4.184 kJ
      case 'watts':
        // This would need duration to be meaningful
        // Watts = (calories × 4.184) / (time in seconds)
        return calories * 4.184; // Just the energy component
      case 'foodCalories':
        return calories / 1000; // Food calories are actually kcal
      default:
        return calories;
    }
  }

  /**
   * Estimate calories burned for common activities.
   * Like a quick reference for different exercise types.
   */
  static estimateActivityCalories(
    activity: string,
    weightKg: number,
    durationMinutes: number
  ): number {
    const metValues: { [key: string]: number } = {
      'walking': 3.5,
      'running': 8.0,
      'cycling': 7.5,
      'swimming': 8.0,
      'weightlifting': 4.5,
      'yoga': 2.5,
      'dancing': 5.0,
      'hiking': 6.0,
      'basketball': 7.0,
      'tennis': 6.0,
      'soccer': 8.0,
      'rowing': 6.5
    };

    const metValue = metValues[activity.toLowerCase()] || 4.0; // Default MET value
    return this.calculateCalories(metValue, weightKg, durationMinutes);
  }

  /**
   * Calculate daily calorie goal based on user profile.
   * Like setting target calorie burn for fitness goals.
   */
  static calculateDailyCalorieGoal(
    age: number,
    gender: 'male' | 'female',
    weightKg: number,
    heightCm: number,
    activityLevel: 'sedentary' | 'light' | 'moderate' | 'active' | 'very_active',
    goal: 'maintain' | 'lose' | 'gain' = 'maintain'
  ): number {
    // Calculate Basal Metabolic Rate (BMR) using Mifflin-St Jeor Equation
    let bmr: number;
    if (gender === 'male') {
      bmr = (10 * weightKg) + (6.25 * heightCm) - (5 * age) + 5;
    } else {
      bmr = (10 * weightKg) + (6.25 * heightCm) - (5 * age) - 161;
    }

    // Apply activity factor
    const activityFactors = {
      'sedentary': 1.2,
      'light': 1.375,
      'moderate': 1.55,
      'active': 1.725,
      'very_active': 1.9
    };

    let dailyCalories = bmr * activityFactors[activityLevel];

    // Adjust for goal
    switch (goal) {
      case 'lose':
        dailyCalories -= 500; // 500 calorie deficit for ~1 lb/week loss
        break;
      case 'gain':
        dailyCalories += 500; // 500 calorie surplus for ~1 lb/week gain
        break;
      // 'maintain' keeps the calculated value
    }

    return Math.round(dailyCalories);
  }

  /**
   * Get motivational message based on calories burned.
   * Like encouraging feedback from a personal trainer.
   */
  static getMotivationalMessage(caloriesBurned: number): string {
    if (caloriesBurned >= 1000) {
      return "🔥 Incredible! You're on fire today!";
    } else if (caloriesBurned >= 500) {
      return "💪 Awesome workout! You're crushing it!";
    } else if (caloriesBurned >= 250) {
      return "🎯 Great job! Every calorie counts!";
    } else if (caloriesBurned >= 100) {
      return "✨ Nice start! Keep building momentum!";
    } else if (caloriesBurned > 0) {
      return "🌟 Every step counts! You're doing great!";
    } else {
      return "Ready to start your fitness journey?";
    }
  }

  /**
   * Compare calories to relatable food items.
   * Like showing "You burned off a slice of pizza!" for context.
   */
  static compareToFood(calories: number): string {
    const foodComparisons = [
      { food: 'apple', calories: 80, emoji: '🍎' },
      { food: 'banana', calories: 105, emoji: '🍌' },
      { food: 'slice of bread', calories: 120, emoji: '🍞' },
      { food: 'cookie', calories: 150, emoji: '🍪' },
      { food: 'donut', calories: 250, emoji: '🍩' },
      { food: 'slice of pizza', calories: 300, emoji: '🍕' },
      { food: 'burger', calories: 500, emoji: '🍔' },
      { food: 'large fries', calories: 600, emoji: '🍟' }
    ];

    // Find the closest food item
    const closest = foodComparisons.reduce((prev, curr) => 
      Math.abs(curr.calories - calories) < Math.abs(prev.calories - calories) ? curr : prev
    );

    const quantity = Math.round(calories / closest.calories * 10) / 10;
    const quantityText = quantity === 1 ? 'a' : quantity.toString();

    return `${closest.emoji} ${quantityText} ${closest.food}${quantity !== 1 ? 's' : ''}`;
  }

  /**
   * Static method for direct use in components.
   */
  static format(
    calories: number,
    format: 'short' | 'long' | 'detailed' = 'short',
    showUnit: boolean = true
  ): string {
    const pipe = new CaloriesPipe();
    return pipe.transform(calories, format, showUnit);
  }
}
