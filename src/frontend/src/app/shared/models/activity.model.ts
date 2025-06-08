/**
 * Activity Models - representing the core of fitness tracking.
 * Think of activities as the different "classes" or "sessions" available
 * at a fitness center - each with its own characteristics and metrics.
 */

/**
 * Activity - a type of fitness activity that users can perform.
 * Like the catalog of classes available at a gym: yoga, spinning, weight training, etc.
 */
export interface Activity {
  id: number;
  name: string;
  description?: string;
  type: ActivityType;
  category: ActivityCategory;
  iconUrl?: string;
  metValue: number; // Metabolic Equivalent of Task
  isActive: boolean;
}

/**
 * UserActivity - a specific instance of a user performing an activity.
 * Think of this as a detailed workout log entry - capturing everything
 * about a specific exercise session.
 */
export interface UserActivity {
  id: number;
  userId: number;
  activityId: number;
  activity: Activity;
  
  // Timing
  startedAt: Date;
  completedAt: Date;
  duration: number; // in minutes
  location?: string;
  
  // Performance metrics
  distance?: number; // in kilometers
  steps?: number;
  caloriesBurned?: number;
  averageHeartRate?: number;
  maxHeartRate?: number;
  averagePace?: number; // minutes per kilometer
  elevationGain?: number; // in meters
  
  // User input
  userRating?: number; // 1-5 scale
  notes?: string;
  photos: string[]; // Array of photo URLs
  
  // Social and gamification
  isPublic: boolean;
  experiencePointsEarned: number;
  
  // GPS and environmental data
  routeData?: string; // JSON GPS coordinates
  weatherConditions?: string; // JSON weather data
  
  // Timestamps
  createdAt: Date;
  updatedAt?: Date;
}

/**
 * Activity Route - GPS tracking data for outdoor activities.
 * Like the path you took during your run or bike ride,
 * captured with detailed location points.
 */
export interface ActivityRoute {
  id: number;
  userActivityId: number;
  routePoints: RoutePoint[];
  totalDistance: number;
  totalElevationGain: number;
  averageSpeed: number;
  maxSpeed: number;
}

/**
 * Route Point - a single GPS coordinate in an activity route.
 * Each point captures a moment in time during the activity.
 */
export interface RoutePoint {
  latitude: number;
  longitude: number;
  elevation?: number;
  timestamp: Date;
  heartRate?: number;
  pace?: number;
}

/**
 * Activity Template - predefined activity configurations.
 * Like having workout templates ready to go - "Quick 30-min run",
 * "Morning yoga session", "Strength training", etc.
 */
export interface ActivityTemplate {
  id: number;
  userId: number;
  name: string;
  activityId: number;
  activity: Activity;
  defaultDuration?: number; // in minutes
  defaultLocation?: string;
  isPublic: boolean;
  useCount: number; // How many times user has used this template
  
  createdAt: Date;
  updatedAt?: Date;
}

/**
 * Challenge - gamified fitness challenges.
 * Think of challenges like special events or competitions
 * that motivate users to push beyond their normal routine.
 */
export interface Challenge {
  id: number;
  title: string;
  description: string;
  imageUrl?: string;
  type: ChallengeType;
  category: ChallengeCategory;
  
  // Challenge parameters
  targetValue: number;
  unit: string;
  duration: number; // in days
  
  // Participation
  maxParticipants?: number;
  currentParticipants: number;
  isPublic: boolean;
  
  // Rewards
  xpReward: number;
  badgeReward?: number; // Badge ID
  
  // Timing
  startDate: Date;
  endDate: Date;
  registrationDeadline?: Date;
  
  // Status
  status: ChallengeStatus;
  
  createdAt: Date;
  updatedAt?: Date;
}

/**
 * User Challenge Participation - tracking user's progress in challenges.
 */
export interface UserChallenge {
  id: number;
  userId: number;
  challengeId: number;
  challenge: Challenge;
  
  joinedAt: Date;
  currentProgress: number;
  isCompleted: boolean;
  completedAt?: Date;
  finalRank?: number;
  
  // Progress tracking
  progressHistory: ChallengeProgress[];
}

/**
 * Challenge Progress - daily/periodic progress updates.
 */
export interface ChallengeProgress {
  date: Date;
  progress: number;
  dailyProgress: number;
}

// Enums

export enum ActivityType {
  Indoor = 'Indoor',
  Outdoor = 'Outdoor',
  Virtual = 'Virtual',
  Manual = 'Manual'
}

export enum ActivityCategory {
  Cardio = 'Cardio',
  Strength = 'Strength',
  Flexibility = 'Flexibility',
  Sports = 'Sports',
  Recreation = 'Recreation',
  Outdoor = 'Outdoor',
  Water = 'Water',
  Winter = 'Winter',
  MartialArts = 'Martial_Arts',
  Dance = 'Dance',
  Other = 'Other'
}

export enum ChallengeType {
  Individual = 'Individual',
  Team = 'Team',
  Community = 'Community',
  Streak = 'Streak',
  Virtual = 'Virtual'
}

export enum ChallengeCategory {
  Distance = 'Distance',
  Duration = 'Duration',
  Frequency = 'Frequency',
  Steps = 'Steps',
  Calories = 'Calories',
  Consistency = 'Consistency',
  Social = 'Social'
}

export enum ChallengeStatus {
  Draft = 'Draft',
  Open = 'Open',
  Active = 'Active',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

// Request/Response types

export interface CreateActivityRequest {
  activityId: number;
  startedAt: Date;
  completedAt: Date;
  distance?: number;
  steps?: number;
  location?: string;
  userRating?: number;
  notes?: string;
  isPublic?: boolean;
}

export interface UpdateActivityRequest {
  distance?: number;
  steps?: number;
  averageHeartRate?: number;
  maxHeartRate?: number;
  elevationGain?: number;
  userRating?: number;
  notes?: string;
  isPublic?: boolean;
}

export interface CreateChallengeRequest {
  title: string;
  description: string;
  type: ChallengeType;
  category: ChallengeCategory;
  targetValue: number;
  unit: string;
  duration: number;
  startDate: Date;
  endDate: Date;
  maxParticipants?: number;
  isPublic?: boolean;
}

export interface ActivityStatsResponse {
  totalActivities: number;
  totalDuration: number; // in minutes
  totalDistance: number; // in kilometers
  totalCalories: number;
  averageRating: number;
  activitiesByCategory: { [key in ActivityCategory]?: number };
  weeklyStats: WeeklyActivityStats[];
  personalBests: PersonalBest[];
}

export interface WeeklyActivityStats {
  weekStart: Date;
  weekEnd: Date;
  totalActivities: number;
  totalDuration: number;
  totalDistance: number;
  totalCalories: number;
}

export interface PersonalBest {
  activityId: number;
  activityName: string;
  metric: string; // 'distance', 'duration', 'pace', etc.
  value: number;
  unit: string;
  achievedAt: Date;
  userActivityId: number;
}

// Utility classes

export class ActivityUtils {
  static formatDuration(minutes: number): string {
    if (minutes < 60) {
      return `${minutes}m`;
    }
    
    const hours = Math.floor(minutes / 60);
    const remainingMinutes = minutes % 60;
    
    if (remainingMinutes === 0) {
      return `${hours}h`;
    }
    
    return `${hours}h ${remainingMinutes}m`;
  }

  static formatDistance(kilometers: number, unit: 'metric' | 'imperial' = 'metric'): string {
    if (unit === 'imperial') {
      const miles = kilometers * 0.621371;
      return `${miles.toFixed(2)} mi`;
    }
    
    if (kilometers < 1) {
      return `${(kilometers * 1000).toFixed(0)} m`;
    }
    
    return `${kilometers.toFixed(2)} km`;
  }

  static formatPace(minutesPerKm: number, unit: 'metric' | 'imperial' = 'metric'): string {
    if (unit === 'imperial') {
      const minutesPerMile = minutesPerKm * 1.60934;
      const minutes = Math.floor(minutesPerMile);
      const seconds = Math.round((minutesPerMile - minutes) * 60);
      return `${minutes}:${seconds.toString().padStart(2, '0')}/mi`;
    }
    
    const minutes = Math.floor(minutesPerKm);
    const seconds = Math.round((minutesPerKm - minutes) * 60);
    return `${minutes}:${seconds.toString().padStart(2, '0')}/km`;
  }

  static calculateCalories(metValue: number, weightKg: number, durationMinutes: number): number {
    // Calories = MET * weight(kg) * time(hours)
    return Math.round(metValue * weightKg * (durationMinutes / 60));
  }

  static calculateAverageSpeed(distance: number, durationMinutes: number): number {
    // Speed in km/h
    return distance / (durationMinutes / 60);
  }

  static getActivityCategoryIcon(category: ActivityCategory): string {
    const iconMap: { [key in ActivityCategory]: string } = {
      [ActivityCategory.Cardio]: 'favorite',
      [ActivityCategory.Strength]: 'fitness_center',
      [ActivityCategory.Flexibility]: 'self_improvement',
      [ActivityCategory.Sports]: 'sports_tennis',
      [ActivityCategory.Recreation]: 'outdoor_grill',
      [ActivityCategory.Outdoor]: 'terrain',
      [ActivityCategory.Water]: 'pool',
      [ActivityCategory.Winter]: 'ac_unit',
      [ActivityCategory.MartialArts]: 'sports_kabaddi',
      [ActivityCategory.Dance]: 'music_note',
      [ActivityCategory.Other]: 'directions_run'
    };
    
    return iconMap[category] || 'directions_run';
  }

  static getCategoryColor(category: ActivityCategory): string {
    const colorMap: { [key in ActivityCategory]: string } = {
      [ActivityCategory.Cardio]: '#e74c3c',
      [ActivityCategory.Strength]: '#3498db',
      [ActivityCategory.Flexibility]: '#9b59b6',
      [ActivityCategory.Sports]: '#f39c12',
      [ActivityCategory.Recreation]: '#27ae60',
      [ActivityCategory.Outdoor]: '#16a085',
      [ActivityCategory.Water]: '#2980b9',
      [ActivityCategory.Winter]: '#95a5a6',
      [ActivityCategory.MartialArts]: '#e67e22',
      [ActivityCategory.Dance]: '#f1c40f',
      [ActivityCategory.Other]: '#34495e'
    };
    
    return colorMap[category] || '#34495e';
  }

  static isEnduranceActivity(category: ActivityCategory): boolean {
    return [
      ActivityCategory.Cardio,
      ActivityCategory.Outdoor,
      ActivityCategory.Water
    ].includes(category);
  }

  static isStrengthActivity(category: ActivityCategory): boolean {
    return [
      ActivityCategory.Strength,
      ActivityCategory.MartialArts
    ].includes(category);
  }

  static shouldTrackHeartRate(category: ActivityCategory): boolean {
    return [
      ActivityCategory.Cardio,
      ActivityCategory.Sports,
      ActivityCategory.Outdoor,
      ActivityCategory.Water,
      ActivityCategory.Dance
    ].includes(category);
  }
}
