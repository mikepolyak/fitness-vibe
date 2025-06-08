/**
 * User Model - represents a fitness enthusiast in our app ecosystem.
 * Think of this as a digital membership profile that tracks someone's
 * entire fitness journey, from beginner to fitness expert.
 */
export interface User {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  dateOfBirth?: Date;
  gender?: Gender;
  avatarUrl?: string;
  
  // Fitness profile
  fitnessLevel: FitnessLevel;
  primaryGoal: FitnessGoal;
  preferences: UserPreferences;
  
  // Gamification
  experiencePoints: number;
  level: number;
  lastActiveDate: Date;
  
  // Account status
  isEmailVerified: boolean;
  isActive: boolean;
  
  // Timestamps
  createdAt: Date;
  updatedAt?: Date;
}

/**
 * User Preferences - personal settings that shape the fitness experience.
 * Like customizing your gym experience with your favorite equipment,
 * music preferences, and training schedule.
 */
export interface UserPreferences {
  timeZone: string;
  allowNotifications: boolean;
  shareActivitiesPublicly: boolean;
  receiveMotivationalMessages: boolean;
  allowFriendRequests: boolean;
  quietHourStart: number; // 24-hour format
  quietHourEnd: number;   // 24-hour format
  preferredUnits: 'metric' | 'imperial';
  enableAudioCues: boolean;
  shareToSocialMedia: boolean;
}

/**
 * User Goal - a specific target the user is working toward.
 * Think of goals like personal training milestones - specific,
 * measurable, and time-bound objectives.
 */
export interface UserGoal {
  id: number;
  userId: number;
  title: string;
  description?: string;
  type: GoalType;
  frequency: GoalFrequency;
  targetValue: number;
  currentValue: number;
  unit: string;
  startDate: Date;
  endDate: Date;
  status: GoalStatus;
  isAdaptive: boolean;
  
  // Computed properties
  progressPercentage?: number;
  timeRemaining?: number; // in days
  isOverdue?: boolean;
}

/**
 * User Badge - an achievement earned through fitness activities.
 * Like merit badges or trophies that celebrate milestones
 * and motivate continued progress.
 */
export interface UserBadge {
  id: number;
  userId: number;
  badgeId: number;
  badge: Badge;
  earnedAt: Date;
  earnedContext?: string; // JSON string with achievement details
  isVisible: boolean;
}

/**
 * Badge - a specific achievement that can be earned.
 * Think of badges like different types of awards at a fitness competition -
 * each with its own criteria and prestige level.
 */
export interface Badge {
  id: number;
  name: string;
  description: string;
  iconUrl: string;
  category: BadgeCategory;
  rarity: BadgeRarity;
  points: number; // XP awarded when earned
  criteria: string; // JSON criteria for earning
  isActive: boolean;
}

// Enums matching backend domain

export enum Gender {
  NotSpecified = 'NotSpecified',
  Male = 'Male',
  Female = 'Female',
  Other = 'Other'
}

export enum FitnessLevel {
  Beginner = 'Beginner',
  Intermediate = 'Intermediate',
  Advanced = 'Advanced',
  Expert = 'Expert'
}

export enum FitnessGoal {
  LoseWeight = 'LoseWeight',
  BuildMuscle = 'BuildMuscle',
  ImproveEndurance = 'ImproveEndurance',
  StayActive = 'StayActive',
  CompeteInEvents = 'CompeteInEvents',
  Rehabilitation = 'Rehabilitation'
}

export enum GoalType {
  Steps = 'Steps',
  Distance = 'Distance',
  Duration = 'Duration',
  Frequency = 'Frequency',
  Weight = 'Weight',
  Calories = 'Calories',
  Custom = 'Custom'
}

export enum GoalFrequency {
  Daily = 'Daily',
  Weekly = 'Weekly',
  Monthly = 'Monthly',
  Quarterly = 'Quarterly',
  Yearly = 'Yearly',
  OneTime = 'OneTime'
}

export enum GoalStatus {
  Active = 'Active',
  Completed = 'Completed',
  Expired = 'Expired',
  Abandoned = 'Abandoned'
}

export enum BadgeCategory {
  Activity = 'Activity',
  Streak = 'Streak',
  Social = 'Social',
  Challenge = 'Challenge',
  Milestone = 'Milestone',
  Special = 'Special',
  Achievement = 'Achievement'
}

export enum BadgeRarity {
  Common = 'Common',
  Uncommon = 'Uncommon',
  Rare = 'Rare',
  Epic = 'Epic',
  Legendary = 'Legendary'
}

// Utility types for forms and API requests

export interface CreateUserRequest {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  fitnessLevel?: FitnessLevel;
  primaryGoal?: FitnessGoal;
}

export interface UpdateUserProfileRequest {
  firstName?: string;
  lastName?: string;
  dateOfBirth?: Date;
  gender?: Gender;
  avatarUrl?: string;
}

export interface UpdateUserPreferencesRequest {
  timeZone?: string;
  allowNotifications?: boolean;
  shareActivitiesPublicly?: boolean;
  receiveMotivationalMessages?: boolean;
  allowFriendRequests?: boolean;
  quietHourStart?: number;
  quietHourEnd?: number;
  preferredUnits?: 'metric' | 'imperial';
  enableAudioCues?: boolean;
  shareToSocialMedia?: boolean;
}

export interface CreateGoalRequest {
  title: string;
  description?: string;
  type: GoalType;
  frequency: GoalFrequency;
  targetValue: number;
  unit: string;
  startDate: Date;
  endDate: Date;
  isAdaptive?: boolean;
}

// API Response types

export interface AuthResponse {
  user: User;
  token: string;
  refreshToken: string;
}

export interface TokenRefreshResponse {
  token: string;
  refreshToken: string;
}

// Level progression utilities

export interface LevelProgress {
  currentLevel: number;
  currentXP: number;
  nextLevelXP: number;
  totalXPForCurrentLevel: number;
  progressPercentage: number;
  xpToNextLevel: number;
}

export class UserUtils {
  static getDisplayName(user: User): string {
    return `${user.firstName} ${user.lastName}`;
  }

  static getAge(user: User): number | null {
    if (!user.dateOfBirth) return null;
    
    const today = new Date();
    const birthDate = new Date(user.dateOfBirth);
    let age = today.getFullYear() - birthDate.getFullYear();
    const monthDiff = today.getMonth() - birthDate.getMonth();
    
    if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < birthDate.getDate())) {
      age--;
    }
    
    return age;
  }

  static calculateLevelProgress(user: User): LevelProgress {
    const currentLevel = user.level;
    const totalXP = user.experiencePoints;
    
    // Calculate total XP required for current level
    let totalXPForCurrentLevel = 0;
    for (let i = 1; i < currentLevel; i++) {
      totalXPForCurrentLevel += i * 100;
    }
    
    // XP within current level
    const currentXP = totalXP - totalXPForCurrentLevel;
    const nextLevelXP = currentLevel * 100;
    const progressPercentage = (currentXP / nextLevelXP) * 100;
    const xpToNextLevel = nextLevelXP - currentXP;
    
    return {
      currentLevel,
      currentXP,
      nextLevelXP,
      totalXPForCurrentLevel,
      progressPercentage: Math.min(100, Math.max(0, progressPercentage)),
      xpToNextLevel: Math.max(0, xpToNextLevel)
    };
  }

  static getGoalProgressPercentage(goal: UserGoal): number {
    if (goal.targetValue === 0) return 0;
    return Math.min(100, (goal.currentValue / goal.targetValue) * 100);
  }

  static isGoalOverdue(goal: UserGoal): boolean {
    return new Date() > new Date(goal.endDate) && goal.status === GoalStatus.Active;
  }

  static getTimeRemainingInDays(goal: UserGoal): number {
    const now = new Date();
    const endDate = new Date(goal.endDate);
    const diffTime = endDate.getTime() - now.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }
}
