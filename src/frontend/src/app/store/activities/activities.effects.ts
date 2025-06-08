import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { of, timer, EMPTY } from 'rxjs';
import { 
  map, 
  exhaustMap, 
  catchError, 
  tap, 
  switchMap, 
  withLatestFrom,
  takeUntil,
  startWith,
  mergeMap
} from 'rxjs/operators';

import { NotificationService } from '../../core/services/notification.service';
import { ActivitiesService } from '../../core/services/activities.service';
import { AppState } from '../app.state';
import * as ActivitiesActions from './activities.actions';
import * as ActivitiesSelectors from './activities.selectors';

/**
 * Activities Effects - the dedicated fitness coordinators who handle all the behind-the-scenes
 * work for activity tracking. Think of them as personal trainers who manage workout scheduling,
 * progress tracking, challenge enrollment, and performance analytics.
 */
@Injectable()
export class ActivitiesEffects {

  constructor(
    private actions$: Actions,
    private store: Store<AppState>,
    private activitiesService: ActivitiesService,
    private notificationService: NotificationService
  ) {}

  /**
   * Load Activities Catalog Effect - fetches all available exercise types
   * Like getting the complete gym class schedule and equipment catalog
   */
  loadActivities$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.loadActivities),
      switchMap(() =>
        this.activitiesService.getActivities().pipe(
          map(activities => ActivitiesActions.loadActivitiesSuccess({ activities })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load activities catalog. Please try again.'
            );
            return of(ActivitiesActions.loadActivitiesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load User Activities Effect - fetches user's workout history
   * Like pulling up someone's complete exercise log from their membership record
   */
  loadUserActivities$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.loadUserActivities),
      switchMap(({ userId, page = 1, limit = 20, filters }) =>
        this.activitiesService.getUserActivities(userId, page, limit, filters).pipe(
          map(response => ActivitiesActions.loadUserActivitiesSuccess({
            activities: response.activities,
            totalCount: response.totalCount,
            hasMore: response.hasMore
          })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load your activities. Please try again.'
            );
            return of(ActivitiesActions.loadUserActivitiesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Activity By ID Effect - fetches detailed info for a specific workout
   * Like pulling up the complete record for a particular training session
   */
  loadActivityById$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.loadActivityById),
      switchMap(({ activityId }) =>
        this.activitiesService.getActivityById(activityId).pipe(
          map(activity => ActivitiesActions.loadActivityByIdSuccess({ activity })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load activity details. Please try again.'
            );
            return of(ActivitiesActions.loadActivityByIdFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Create Activity Effect - logs a new workout session
   * Like officially recording a completed training session in the logbook
   */
  createActivity$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.createActivity),
      exhaustMap(({ activity }) =>
        this.activitiesService.createActivity(activity).pipe(
          map(newActivity => {
            this.notificationService.showSuccess(
              `Great workout! Your ${newActivity.activity.name} has been logged. 💪`
            );
            return ActivitiesActions.createActivitySuccess({ activity: newActivity });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to save your activity. Please try again.'
            );
            return of(ActivitiesActions.createActivityFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Update Activity Effect - modifies an existing workout record
   * Like updating details about a past training session
   */
  updateActivity$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.updateActivity),
      exhaustMap(({ activityId, updates }) =>
        this.activitiesService.updateActivity(activityId, updates).pipe(
          map(activity => {
            this.notificationService.showSuccess('Activity updated successfully!');
            return ActivitiesActions.updateActivitySuccess({ activity });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to update activity. Please try again.'
            );
            return of(ActivitiesActions.updateActivityFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Delete Activity Effect - removes a workout from history
   * Like removing a training session from the official records
   */
  deleteActivity$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.deleteActivity),
      exhaustMap(({ activityId }) =>
        this.activitiesService.deleteActivity(activityId).pipe(
          map(() => {
            this.notificationService.showSuccess('Activity deleted successfully');
            return ActivitiesActions.deleteActivitySuccess({ activityId });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to delete activity. Please try again.'
            );
            return of(ActivitiesActions.deleteActivityFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Activity Templates Effect - fetches user's workout templates
   * Like getting someone's collection of favorite workout routines
   */
  loadActivityTemplates$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.loadActivityTemplates),
      switchMap(() =>
        this.activitiesService.getActivityTemplates().pipe(
          map(templates => ActivitiesActions.loadActivityTemplatesSuccess({ templates })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load activity templates. Please try again.'
            );
            return of(ActivitiesActions.loadActivityTemplatesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Create Activity Template Effect - saves a new workout template
   * Like creating a custom workout routine for future use
   */
  createActivityTemplate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.createActivityTemplate),
      exhaustMap(({ template }) =>
        this.activitiesService.createActivityTemplate(template).pipe(
          map(newTemplate => {
            this.notificationService.showSuccess(
              'Workout template created! You can now use it for quick workout setup.'
            );
            return ActivitiesActions.createActivityTemplateSuccess({ template: newTemplate });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to create template. Please try again.'
            );
            return of(ActivitiesActions.createActivityTemplateFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Use Activity Template Effect - increments template usage count
   * Like tracking how often someone uses their favorite workout routine
   */
  useActivityTemplate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.useActivityTemplate),
      exhaustMap(({ templateId }) =>
        this.activitiesService.useActivityTemplate(templateId).pipe(
          map(template => ActivitiesActions.useActivityTemplateSuccess({ template })),
          catchError(() => EMPTY) // Silent failure for usage tracking
        )
      )
    )
  );

  /**
   * Load Activity Stats Effect - fetches performance analytics
   * Like generating a comprehensive fitness report card
   */
  loadActivityStats$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.loadActivityStats),
      switchMap(({ userId, period = 'month' }) =>
        this.activitiesService.getActivityStats(userId, period).pipe(
          map(stats => ActivitiesActions.loadActivityStatsSuccess({ stats })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load activity statistics. Please try again.'
            );
            return of(ActivitiesActions.loadActivityStatsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Personal Bests Effect - fetches user's achievement records
   * Like checking someone's trophy case for their best performances
   */
  loadPersonalBests$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.loadPersonalBests),
      switchMap(() =>
        this.activitiesService.getPersonalBests().pipe(
          map(personalBests => ActivitiesActions.loadPersonalBestsSuccess({ personalBests })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load personal bests. Please try again.'
            );
            return of(ActivitiesActions.loadPersonalBestsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Challenges Effect - fetches available and joined challenges
   * Like browsing the community events and competitions board
   */
  loadChallenges$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.loadChallenges),
      switchMap(({ type = 'available', page = 1 }) =>
        this.activitiesService.getChallenges(type, page).pipe(
          map(response => ActivitiesActions.loadChallengesSuccess({
            challenges: response.challenges,
            hasMore: response.hasMore
          })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load challenges. Please try again.'
            );
            return of(ActivitiesActions.loadChallengesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load User Challenges Effect - fetches challenges user has joined
   * Like checking which competitions you're currently enrolled in
   */
  loadUserChallenges$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.loadUserChallenges),
      switchMap(() =>
        this.activitiesService.getUserChallenges().pipe(
          map(userChallenges => ActivitiesActions.loadUserChallengesSuccess({ userChallenges })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load your challenges. Please try again.'
            );
            return of(ActivitiesActions.loadUserChallengesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Join Challenge Effect - enrolls user in a fitness challenge
   * Like signing up for a group fitness competition
   */
  joinChallenge$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.joinChallenge),
      exhaustMap(({ challengeId }) =>
        this.activitiesService.joinChallenge(challengeId).pipe(
          map(userChallenge => {
            this.notificationService.showSuccess(
              `You've joined the challenge! Time to show what you're made of! 🏆`
            );
            return ActivitiesActions.joinChallengeSuccess({ userChallenge });
          }),
          catchError(error => {
            this.notificationService.showError(
              error.message || 'Failed to join challenge. Please try again.'
            );
            return of(ActivitiesActions.joinChallengeFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Leave Challenge Effect - withdraws user from a challenge
   * Like dropping out of a competition before it ends
   */
  leaveChallenge$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.leaveChallenge),
      exhaustMap(({ challengeId }) =>
        this.activitiesService.leaveChallenge(challengeId).pipe(
          map(() => {
            this.notificationService.showInfo('You have left the challenge');
            return ActivitiesActions.leaveChallengeSuccess({ challengeId });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to leave challenge. Please try again.'
            );
            return of(ActivitiesActions.leaveChallengeFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Start Live Activity Effect - begins live workout tracking
   * Like starting a training session with a personal trainer watching
   */
  startLiveActivity$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.startLiveActivity),
      exhaustMap(({ activityId, location }) =>
        this.activitiesService.startLiveActivity(activityId, location).pipe(
          map(liveActivity => {
            this.notificationService.showSuccess(
              'Live tracking started! Let\'s crush this workout! 💪'
            );
            return ActivitiesActions.startLiveActivitySuccess({ liveActivity });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to start live tracking. Please try again.'
            );
            return of(ActivitiesActions.startLiveActivityFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Live Activity Timer Effect - provides regular updates during live tracking
   * Like having a digital coach that updates your metrics every few seconds
   */
  liveActivityTimer$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.startLiveActivitySuccess),
      switchMap(() =>
        timer(0, 5000).pipe( // Update every 5 seconds
          withLatestFrom(this.store.select(ActivitiesSelectors.selectLiveActivity)),
          map(([tick, liveActivity]) => {
            if (!liveActivity.isActive) return EMPTY;
            
            // Calculate current duration
            const duration = this.calculateLiveDuration(liveActivity);
            
            // In a real app, you might get GPS coordinates, heart rate from wearables, etc.
            return ActivitiesActions.updateLiveActivity({ 
              duration: Math.floor(duration / (1000 * 60)) // Convert to minutes
            });
          }),
          takeUntil(
            this.actions$.pipe(
              ofType(ActivitiesActions.stopLiveActivitySuccess, ActivitiesActions.startLiveActivityFailure)
            )
          )
        )
      ),
      mergeMap(action => action === EMPTY ? EMPTY : of(action))
    )
  );

  /**
   * Stop Live Activity Effect - completes and saves live workout session
   * Like finishing with a trainer and getting your session officially recorded
   */
  stopLiveActivity$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.stopLiveActivity),
      withLatestFrom(this.store.select(ActivitiesSelectors.selectLiveActivity)),
      exhaustMap(([{ notes, rating, photos }, liveActivity]) =>
        this.activitiesService.stopLiveActivity(
          liveActivity.activity!.id,
          {
            notes,
            rating,
            photos,
            finalMetrics: liveActivity.liveMetrics
          }
        ).pipe(
          map(activity => {
            this.notificationService.showSuccess(
              `Awesome workout! You earned ${activity.experiencePointsEarned} XP! 🌟`
            );
            return ActivitiesActions.stopLiveActivitySuccess({ activity });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to save your workout. Please try again.'
            );
            return of(ActivitiesActions.stopLiveActivityFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Refresh Activities Effect - reloads all activity-related data
   * Like doing a complete refresh of someone's fitness dashboard
   */
  refreshActivities$ = createEffect(() =>
    this.actions$.pipe(
      ofType(ActivitiesActions.refreshActivities),
      mergeMap(() => [
        ActivitiesActions.loadUserActivities({}),
        ActivitiesActions.loadActivityTemplates(),
        ActivitiesActions.loadChallenges({}),
        ActivitiesActions.loadUserChallenges(),
        ActivitiesActions.loadActivityStats({}),
        ActivitiesActions.loadPersonalBests()
      ])
    )
  );

  /**
   * Auto-refresh Stats Effect - periodically updates statistics
   * Like having your fitness dashboard automatically update throughout the day
   */
  autoRefreshStats$ = createEffect(() =>
    timer(0, 5 * 60 * 1000).pipe( // Every 5 minutes
      startWith(0),
      withLatestFrom(this.store.select(ActivitiesSelectors.selectIsLiveActivityActive)),
      switchMap(([, isLiveActive]) => {
        // Only auto-refresh if not in live activity to avoid interruptions
        if (isLiveActive) return EMPTY;
        
        return of(ActivitiesActions.loadActivityStats({}));
      })
    )
  );

  /**
   * Helper method to calculate live activity duration accounting for pauses
   */
  private calculateLiveDuration(liveActivity: any): number {
    if (!liveActivity.startTime) return 0;
    
    const now = new Date();
    const startTime = new Date(liveActivity.startTime);
    const totalElapsed = now.getTime() - startTime.getTime();
    const netDuration = totalElapsed - liveActivity.totalPausedDuration;
    
    // If currently paused, subtract the current pause time
    if (liveActivity.isPaused && liveActivity.pausedTime) {
      const currentPauseDuration = now.getTime() - new Date(liveActivity.pausedTime).getTime();
      return Math.max(0, netDuration - currentPauseDuration);
    }
    
    return Math.max(0, netDuration);
  }
}
