import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { 
  Activity, 
  UserActivity, 
  ActivityTemplate, 
  Challenge, 
  UserChallenge,
  CreateActivityRequest,
  UpdateActivityRequest,
  ActivityStatsResponse,
  PersonalBest,
  CreateChallengeRequest
} from '../../shared/models/activity.model';

/**
 * Activities Service - the API communication specialist for all fitness activity operations.
 * Think of this as the liaison between our app and the fitness data servers -
 * handling workout logging, template management, challenge participation, and analytics.
 */
@Injectable({
  providedIn: 'root'
})
export class ActivitiesService {
  private readonly baseUrl = `${environment.apiUrl}/api`;

  constructor(private http: HttpClient) {}

  // ============================================================================
  // ACTIVITIES CATALOG MANAGEMENT
  // ============================================================================

  /**
   * Get all available activity types - like browsing the gym's class catalog
   */
  getActivities(): Observable<Activity[]> {
    return this.http.get<Activity[]>(`${this.baseUrl}/activities`);
  }

  /**
   * Get a specific activity type by ID
   */
  getActivity(activityId: number): Observable<Activity> {
    return this.http.get<Activity>(`${this.baseUrl}/activities/${activityId}`);
  }

  // ============================================================================
  // USER ACTIVITIES (WORKOUT HISTORY) MANAGEMENT
  // ============================================================================

  /**
   * Get user's activity history with pagination and filtering
   * Like reviewing someone's complete workout log with search capabilities
   */
  getUserActivities(
    userId?: number, 
    page: number = 1, 
    limit: number = 20, 
    filters?: any
  ): Observable<{
    activities: UserActivity[];
    totalCount: number;
    hasMore: boolean;
  }> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());

    if (userId) {
      params = params.set('userId', userId.toString());
    }

    if (filters) {
      if (filters.activityType) {
        filters.activityType.forEach((type: string) => {
          params = params.append('activityType', type);
        });
      }
      if (filters.category) {
        filters.category.forEach((category: string) => {
          params = params.append('category', category);
        });
      }
      if (filters.startDate) {
        params = params.set('startDate', filters.startDate.toISOString());
      }
      if (filters.endDate) {
        params = params.set('endDate', filters.endDate.toISOString());
      }
      if (filters.minDuration) {
        params = params.set('minDuration', filters.minDuration.toString());
      }
      if (filters.maxDuration) {
        params = params.set('maxDuration', filters.maxDuration.toString());
      }
    }

    return this.http.get<{
      activities: UserActivity[];
      totalCount: number;
      hasMore: boolean;
    }>(`${this.baseUrl}/user-activities`, { params });
  }

  /**
   * Get a specific user activity by ID - like pulling up details for one workout session
   */
  getActivityById(activityId: number): Observable<UserActivity> {
    return this.http.get<UserActivity>(`${this.baseUrl}/user-activities/${activityId}`);
  }

  /**
   * Create a new activity record - like logging a completed workout
   */
  createActivity(activity: CreateActivityRequest): Observable<UserActivity> {
    return this.http.post<UserActivity>(`${this.baseUrl}/user-activities`, activity);
  }

  /**
   * Update an existing activity - like modifying details of a past workout
   */
  updateActivity(activityId: number, updates: UpdateActivityRequest): Observable<UserActivity> {
    return this.http.put<UserActivity>(`${this.baseUrl}/user-activities/${activityId}`, updates);
  }

  /**
   * Delete an activity record - like removing a workout from history
   */
  deleteActivity(activityId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/user-activities/${activityId}`);
  }

  // ============================================================================
  // ACTIVITY TEMPLATES MANAGEMENT
  // ============================================================================

  /**
   * Get user's activity templates - like fetching saved workout routines
   */
  getActivityTemplates(): Observable<ActivityTemplate[]> {
    return this.http.get<ActivityTemplate[]>(`${this.baseUrl}/activity-templates`);
  }

  /**
   * Create a new activity template - like saving a favorite workout routine
   */
  createActivityTemplate(template: Omit<ActivityTemplate, 'id' | 'userId' | 'useCount' | 'createdAt' | 'updatedAt'>): Observable<ActivityTemplate> {
    return this.http.post<ActivityTemplate>(`${this.baseUrl}/activity-templates`, template);
  }

  /**
   * Update an activity template
   */
  updateActivityTemplate(templateId: number, updates: Partial<ActivityTemplate>): Observable<ActivityTemplate> {
    return this.http.put<ActivityTemplate>(`${this.baseUrl}/activity-templates/${templateId}`, updates);
  }

  /**
   * Delete an activity template
   */
  deleteActivityTemplate(templateId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/activity-templates/${templateId}`);
  }

  /**
   * Use an activity template - increments usage count
   */
  useActivityTemplate(templateId: number): Observable<ActivityTemplate> {
    return this.http.post<ActivityTemplate>(`${this.baseUrl}/activity-templates/${templateId}/use`, {});
  }

  // ============================================================================
  // ACTIVITY STATISTICS & ANALYTICS
  // ============================================================================

  /**
   * Get activity statistics for a user - like generating a fitness report card
   */
  getActivityStats(
    userId?: number, 
    period: 'week' | 'month' | 'quarter' | 'year' = 'month'
  ): Observable<ActivityStatsResponse> {
    let params = new HttpParams().set('period', period);
    
    if (userId) {
      params = params.set('userId', userId.toString());
    }

    return this.http.get<ActivityStatsResponse>(`${this.baseUrl}/activities/stats`, { params });
  }

  /**
   * Get user's personal best records - like checking trophy achievements
   */
  getPersonalBests(): Observable<PersonalBest[]> {
    return this.http.get<PersonalBest[]>(`${this.baseUrl}/activities/personal-bests`);
  }

  /**
   * Get activity trends over time
   */
  getActivityTrends(
    period: 'week' | 'month' | 'quarter' | 'year' = 'month',
    activityType?: string,
    category?: string
  ): Observable<any> {
    let params = new HttpParams().set('period', period);
    
    if (activityType) {
      params = params.set('activityType', activityType);
    }
    if (category) {
      params = params.set('category', category);
    }

    return this.http.get(`${this.baseUrl}/activities/trends`, { params });
  }

  // ============================================================================
  // CHALLENGES MANAGEMENT
  // ============================================================================

  /**
   * Get available challenges - like browsing fitness competitions and events
   */
  getChallenges(
    type: 'available' | 'joined' | 'completed' = 'available',
    page: number = 1,
    limit: number = 10
  ): Observable<{
    challenges: Challenge[];
    hasMore: boolean;
  }> {
    const params = new HttpParams()
      .set('type', type)
      .set('page', page.toString())
      .set('limit', limit.toString());

    return this.http.get<{
      challenges: Challenge[];
      hasMore: boolean;
    }>(`${this.baseUrl}/challenges`, { params });
  }

  /**
   * Get a specific challenge by ID
   */
  getChallenge(challengeId: number): Observable<Challenge> {
    return this.http.get<Challenge>(`${this.baseUrl}/challenges/${challengeId}`);
  }

  /**
   * Create a new challenge - like organizing a fitness competition
   */
  createChallenge(challenge: CreateChallengeRequest): Observable<Challenge> {
    return this.http.post<Challenge>(`${this.baseUrl}/challenges`, challenge);
  }

  /**
   * Get challenges the user has joined
   */
  getUserChallenges(): Observable<UserChallenge[]> {
    return this.http.get<UserChallenge[]>(`${this.baseUrl}/user-challenges`);
  }

  /**
   * Join a challenge - like signing up for a fitness competition
   */
  joinChallenge(challengeId: number): Observable<UserChallenge> {
    return this.http.post<UserChallenge>(`${this.baseUrl}/challenges/${challengeId}/join`, {});
  }

  /**
   * Leave a challenge - like withdrawing from a competition
   */
  leaveChallenge(challengeId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/challenges/${challengeId}/leave`);
  }

  /**
   * Get challenge leaderboard
   */
  getChallengeLeaderboard(challengeId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/challenges/${challengeId}/leaderboard`);
  }

  // ============================================================================
  // LIVE ACTIVITY TRACKING
  // ============================================================================

  /**
   * Start live activity tracking - like beginning a workout with real-time monitoring
   */
  startLiveActivity(activityId: number, location?: string): Observable<UserActivity> {
    return this.http.post<UserActivity>(`${this.baseUrl}/live-activities/start`, {
      activityId,
      location,
      startedAt: new Date().toISOString()
    });
  }

  /**
   * Update live activity metrics during workout
   */
  updateLiveActivityMetrics(
    liveActivityId: number, 
    metrics: {
      duration?: number;
      distance?: number;
      heartRate?: number;
      pace?: number;
      routePoints?: any[];
    }
  ): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/live-activities/${liveActivityId}/metrics`, metrics);
  }

  /**
   * Stop live activity tracking and save final workout
   */
  stopLiveActivity(
    liveActivityId: number,
    completionData: {
      notes?: string;
      rating?: number;
      photos?: string[];
      finalMetrics: any;
    }
  ): Observable<UserActivity> {
    return this.http.post<UserActivity>(`${this.baseUrl}/live-activities/${liveActivityId}/stop`, {
      ...completionData,
      completedAt: new Date().toISOString()
    });
  }

  // ============================================================================
  // GPS AND ROUTE TRACKING
  // ============================================================================

  /**
   * Upload GPS route data for an activity
   */
  uploadActivityRoute(activityId: number, routeData: any): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/user-activities/${activityId}/route`, routeData);
  }

  /**
   * Get route data for an activity
   */
  getActivityRoute(activityId: number): Observable<any> {
    return this.http.get(`${this.baseUrl}/user-activities/${activityId}/route`);
  }

  // ============================================================================
  // SOCIAL AND SHARING
  // ============================================================================

  /**
   * Get public activities feed - like viewing community workout posts
   */
  getPublicActivitiesFeed(page: number = 1, limit: number = 20): Observable<{
    activities: UserActivity[];
    hasMore: boolean;
  }> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());

    return this.http.get<{
      activities: UserActivity[];
      hasMore: boolean;
    }>(`${this.baseUrl}/activities/feed`, { params });
  }

  /**
   * Like an activity post
   */
  likeActivity(activityId: number): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/user-activities/${activityId}/like`, {});
  }

  /**
   * Unlike an activity post
   */
  unlikeActivity(activityId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/user-activities/${activityId}/like`);
  }

  /**
   * Comment on an activity
   */
  commentOnActivity(activityId: number, comment: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/user-activities/${activityId}/comments`, { comment });
  }

  /**
   * Get comments for an activity
   */
  getActivityComments(activityId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.baseUrl}/user-activities/${activityId}/comments`);
  }

  // ============================================================================
  // DATA EXPORT AND IMPORT
  // ============================================================================

  /**
   * Export user activities to various formats
   */
  exportActivities(
    format: 'csv' | 'json' | 'gpx' = 'csv',
    filters?: any
  ): Observable<Blob> {
    let params = new HttpParams().set('format', format);
    
    if (filters) {
      Object.keys(filters).forEach(key => {
        if (filters[key] !== null && filters[key] !== undefined) {
          params = params.set(key, filters[key].toString());
        }
      });
    }

    return this.http.get(`${this.baseUrl}/activities/export`, {
      params,
      responseType: 'blob'
    });
  }

  /**
   * Import activities from external sources
   */
  importActivities(file: File, source: 'strava' | 'garmin' | 'fitbit' | 'csv'): Observable<{
    imported: number;
    skipped: number;
    errors: string[];
  }> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('source', source);

    return this.http.post<{
      imported: number;
      skipped: number;
      errors: string[];
    }>(`${this.baseUrl}/activities/import`, formData);
  }

  // ============================================================================
  // WEARABLE DEVICE INTEGRATION
  // ============================================================================

  /**
   * Sync activities from connected wearable devices
   */
  syncWearableData(deviceType: 'fitbit' | 'garmin' | 'apple' | 'polar'): Observable<{
    synced: number;
    lastSync: Date;
  }> {
    return this.http.post<{
      synced: number;
      lastSync: Date;
    }>(`${this.baseUrl}/activities/sync/${deviceType}`, {});
  }

  /**
   * Get wearable device sync status
   */
  getWearableSyncStatus(): Observable<{
    devices: Array<{
      type: string;
      connected: boolean;
      lastSync: Date | null;
    }>;
  }> {
    return this.http.get<{
      devices: Array<{
        type: string;
        connected: boolean;
        lastSync: Date | null;
      }>;
    }>(`${this.baseUrl}/activities/sync/status`);
  }
}
