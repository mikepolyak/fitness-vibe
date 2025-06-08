import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { of, timer } from 'rxjs';
import { 
  map, 
  exhaustMap, 
  catchError, 
  tap, 
  switchMap, 
  withLatestFrom,
  mergeMap,
  startWith,
  filter
} from 'rxjs/operators';

import { NotificationService } from '../../core/services/notification.service';
import { GoalsService } from '../../core/services/goals.service';
import { AppState } from '../app.state';
import * as GoalsActions from './goals.actions';
import * as GoalsSelectors from './goals.selectors';
import * as AuthSelectors from '../auth/auth.selectors';

/**
 * Goals Effects - the dedicated goal achievement coordinators who handle all the behind-the-scenes
 * work for goal management. Think of them as personal success coaches who track progress,
 * provide motivation, suggest adjustments, and celebrate achievements.
 */
@Injectable()
export class GoalsEffects {

  constructor(
    private actions$: Actions,
    private store: Store<AppState>,
    private goalsService: GoalsService,
    private notificationService: NotificationService
  ) {}

  /**
   * Load Goals Effect - fetches user's fitness goals
   * Like pulling up someone's complete list of fitness aspirations and commitments
   */
  loadGoals$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.loadGoals),
      switchMap(({ userId, status, type }) =>
        this.goalsService.getGoals(userId, status, type).pipe(
          map(goals => GoalsActions.loadGoalsSuccess({ goals })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load your goals. Please try again.'
            );
            return of(GoalsActions.loadGoalsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Goal By ID Effect - fetches detailed info for a specific goal
   * Like examining the complete details of one particular fitness target
   */
  loadGoalById$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.loadGoalById),
      switchMap(({ goalId }) =>
        this.goalsService.getGoalById(goalId).pipe(
          map(goal => GoalsActions.loadGoalByIdSuccess({ goal })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load goal details. Please try again.'
            );
            return of(GoalsActions.loadGoalByIdFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Create Goal Effect - sets up a new fitness goal
   * Like making a formal commitment to a new fitness challenge or target
   */
  createGoal$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.createGoal),
      exhaustMap(({ goal }) =>
        this.goalsService.createGoal(goal).pipe(
          map(newGoal => {
            this.notificationService.showSuccess(
              `New goal "${newGoal.title}" created! Time to make it happen! 🎯`
            );
            return GoalsActions.createGoalSuccess({ goal: newGoal });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to create your goal. Please try again.'
            );
            return of(GoalsActions.createGoalFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Update Goal Effect - modifies an existing goal
   * Like adjusting your fitness targets based on progress or changing circumstances
   */
  updateGoal$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.updateGoal),
      exhaustMap(({ goalId, updates }) =>
        this.goalsService.updateGoal(goalId, updates).pipe(
          map(goal => {
            this.notificationService.showSuccess('Goal updated successfully!');
            return GoalsActions.updateGoalSuccess({ goal });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to update your goal. Please try again.'
            );
            return of(GoalsActions.updateGoalFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Delete Goal Effect - removes a goal
   * Like deciding to abandon a fitness target that's no longer relevant
   */
  deleteGoal$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.deleteGoal),
      exhaustMap(({ goalId }) =>
        this.goalsService.deleteGoal(goalId).pipe(
          map(() => {
            this.notificationService.showSuccess('Goal deleted successfully');
            return GoalsActions.deleteGoalSuccess({ goalId });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to delete goal. Please try again.'
            );
            return of(GoalsActions.deleteGoalFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Update Goal Progress Effect - tracks progress toward a goal
   * Like logging advancement toward your fitness target after each workout
   */
  updateGoalProgress$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.updateGoalProgress),
      exhaustMap(({ goalId, progressValue, activityId }) =>
        this.goalsService.updateGoalProgress(goalId, progressValue, activityId).pipe(
          map(goal => {
            // Check if goal was completed with this update
            if (goal.progressPercentage === 100 && goal.status === 'Completed') {
              this.notificationService.showSuccess(
                `🎉 Congratulations! You've completed "${goal.title}"! Amazing work!`
              );
            } else if (goal.progressPercentage && goal.progressPercentage >= 75) {
              this.notificationService.showSuccess(
                `Great progress on "${goal.title}"! You're ${Math.round(goal.progressPercentage)}% there! 💪`
              );
            }
            
            return GoalsActions.updateGoalProgressSuccess({ goal });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to update goal progress. Please try again.'
            );
            return of(GoalsActions.updateGoalProgressFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Complete Goal Effect - manually marks a goal as completed
   * Like officially celebrating the achievement of a fitness milestone
   */
  completeGoal$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.completeGoal),
      exhaustMap(({ goalId }) =>
        this.goalsService.completeGoal(goalId).pipe(
          map(response => {
            this.notificationService.showSuccess(
              `🎉 Goal completed! You earned ${response.experiencePointsEarned} XP! Keep crushing it!`
            );
            return GoalsActions.completeGoalSuccess({ 
              goal: response.goal, 
              experiencePointsEarned: response.experiencePointsEarned 
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to complete goal. Please try again.'
            );
            return of(GoalsActions.completeGoalFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Abandon Goal Effect - marks a goal as abandoned
   * Like officially stepping back from a fitness target that's no longer feasible
   */
  abandonGoal$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.abandonGoal),
      exhaustMap(({ goalId, reason }) =>
        this.goalsService.abandonGoal(goalId, reason).pipe(
          map(() => {
            this.notificationService.showInfo(
              'Goal marked as abandoned. Remember, every setback is a setup for a comeback! 💪'
            );
            return GoalsActions.abandonGoalSuccess({ goalId });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to abandon goal. Please try again.'
            );
            return of(GoalsActions.abandonGoalFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Goal Suggestions Effect - gets AI-powered goal recommendations
   * Like having a personal trainer suggest your next fitness challenges
   */
  loadGoalSuggestions$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.loadGoalSuggestions),
      withLatestFrom(this.store.select(AuthSelectors.selectCurrentUser)),
      switchMap(([{ basedOnActivity, userLevel }, user]) =>
        this.goalsService.getGoalSuggestions(
          basedOnActivity, 
          userLevel || user?.level
        ).pipe(
          map(suggestions => GoalsActions.loadGoalSuggestionsSuccess({ suggestions })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load goal suggestions. Please try again.'
            );
            return of(GoalsActions.loadGoalSuggestionsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Request Goal Adjustment Effect - AI-powered goal difficulty adjustments
   * Like having a smart trainer automatically adjust your targets based on performance
   */
  requestGoalAdjustment$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.requestGoalAdjustment),
      exhaustMap(({ goalId, reason }) =>
        this.goalsService.requestGoalAdjustment(goalId, reason).pipe(
          map(response => {
            this.notificationService.showSuccess(
              `Goal adjusted! ${response.adjustmentReason}`
            );
            return GoalsActions.requestGoalAdjustmentSuccess({ 
              goal: response.goal,
              adjustmentReason: response.adjustmentReason
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to adjust goal. Please try again.'
            );
            return of(GoalsActions.requestGoalAdjustmentFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Goal Templates Effect - fetches preset goal configurations
   * Like browsing a catalog of proven fitness challenge templates
   */
  loadGoalTemplates$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.loadGoalTemplates),
      switchMap(() =>
        this.goalsService.getGoalTemplates().pipe(
          map(templates => GoalsActions.loadGoalTemplatesSuccess({ templates })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load goal templates. Please try again.'
            );
            return of(GoalsActions.loadGoalTemplatesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Create Goal From Template Effect - instantiates a goal from a template
   * Like choosing a preset workout plan and customizing it for yourself
   */
  createGoalFromTemplate$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.createGoalFromTemplate),
      exhaustMap(({ templateId, customizations }) =>
        this.goalsService.createGoalFromTemplate(templateId, customizations).pipe(
          map(goal => {
            this.notificationService.showSuccess(
              `Goal "${goal.title}" created from template! Let's achieve it! 🎯`
            );
            return GoalsActions.createGoalSuccess({ goal });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to create goal from template. Please try again.'
            );
            return of(GoalsActions.createGoalFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Goal Analytics Effect - fetches goal performance analytics
   * Like generating a comprehensive report card for your goal achievement history
   */
  loadGoalAnalytics$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.loadGoalAnalytics),
      switchMap(({ period = 'month' }) =>
        this.goalsService.getGoalAnalytics(period).pipe(
          map(analytics => GoalsActions.loadGoalAnalyticsSuccess({ analytics })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load goal analytics. Please try again.'
            );
            return of(GoalsActions.loadGoalAnalyticsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Goal Milestones Effect - fetches milestone tracking for a goal
   * Like checking the checkpoint achievements for a long-term fitness journey
   */
  loadGoalMilestones$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.loadGoalMilestones),
      switchMap(({ goalId }) =>
        this.goalsService.getGoalMilestones(goalId).pipe(
          map(milestones => GoalsActions.loadGoalMilestonesSuccess({ goalId, milestones })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load goal milestones. Please try again.'
            );
            return of(GoalsActions.loadGoalMilestonesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Achieve Milestone Effect - marks a goal milestone as achieved
   * Like celebrating reaching a significant checkpoint in your fitness journey
   */
  achieveMilestone$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.achieveMilestone),
      exhaustMap(({ goalId, milestoneId }) =>
        this.goalsService.achieveMilestone(goalId, milestoneId).pipe(
          map(response => {
            if (response.reward) {
              this.notificationService.showSuccess(
                `🎉 Milestone achieved! You earned: ${response.reward.value}${response.reward.type === 'xp' ? ' XP' : ''}!`
              );
            } else {
              this.notificationService.showSuccess('🎯 Milestone achieved! Great progress!');
            }
            
            return GoalsActions.achieveMilestoneSuccess({ 
              goalId, 
              milestoneId, 
              reward: response.reward 
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to record milestone achievement. Please try again.'
            );
            return of(GoalsActions.loadGoalMilestonesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Track Goal Progress (Bulk) Effect - updates multiple goals based on activities
   * Like automatically updating all relevant fitness goals after a comprehensive workout session
   */
  trackGoalProgress$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.trackGoalProgress),
      exhaustMap(({ activities }) =>
        this.goalsService.trackGoalProgress(activities).pipe(
          map(updatedGoals => {
            const completedGoals = updatedGoals.filter(g => 
              g.status === 'Completed' && g.progressPercentage === 100
            );
            
            if (completedGoals.length > 0) {
              this.notificationService.showSuccess(
                `🎉 Amazing! You completed ${completedGoals.length} goal${completedGoals.length > 1 ? 's' : ''} with this workout!`
              );
            }
            
            return GoalsActions.trackGoalProgressSuccess({ updatedGoals });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to update goal progress. Please try again.'
            );
            return of(GoalsActions.trackGoalProgressFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Share Goal Effect - shares a goal with friends or community
   * Like announcing your fitness commitment to get social support and accountability
   */
  shareGoal$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.shareGoal),
      exhaustMap(({ goalId, platform, message }) =>
        this.goalsService.shareGoal(goalId, platform, message).pipe(
          map(() => {
            this.notificationService.showSuccess(
              'Goal shared successfully! Your community will cheer you on! 📢'
            );
            return GoalsActions.shareGoalSuccess({ goalId });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to share goal. Please try again.'
            );
            return of(GoalsActions.shareGoalFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Shared Goals Effect - fetches goals shared by friends
   * Like browsing what fitness challenges your workout buddies are tackling
   */
  loadSharedGoals$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.loadSharedGoals),
      switchMap(({ friendsOnly = false, page = 1 }) =>
        this.goalsService.getSharedGoals(friendsOnly, page).pipe(
          map(sharedGoals => GoalsActions.loadSharedGoalsSuccess({ sharedGoals })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load shared goals. Please try again.'
            );
            return of(GoalsActions.loadSharedGoalsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Schedule Goal Reminder Effect - sets up reminders for goal activities
   * Like setting an alarm to remind you about your fitness commitments
   */
  scheduleGoalReminder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.scheduleGoalReminder),
      exhaustMap(({ goalId, reminderType, time }) =>
        this.goalsService.scheduleGoalReminder(goalId, reminderType, time).pipe(
          map(response => {
            this.notificationService.showSuccess(
              'Goal reminder scheduled! We\'ll help keep you on track! ⏰'
            );
            return GoalsActions.scheduleGoalReminderSuccess({ 
              goalId, 
              reminderId: response.reminderId 
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to schedule reminder. Please try again.'
            );
            return of(GoalsActions.scheduleGoalReminderFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Cancel Goal Reminder Effect - removes a scheduled reminder
   * Like turning off an alarm you no longer need
   */
  cancelGoalReminder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.cancelGoalReminder),
      exhaustMap(({ goalId, reminderId }) =>
        this.goalsService.cancelGoalReminder(goalId, reminderId).pipe(
          map(() => {
            this.notificationService.showSuccess('Reminder cancelled successfully');
            return GoalsActions.cancelGoalReminderSuccess({ goalId, reminderId });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to cancel reminder. Please try again.'
            );
            return of(GoalsActions.scheduleGoalReminderFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Refresh Goals Effect - reloads all goal-related data
   * Like doing a complete refresh of your goal dashboard
   */
  refreshGoals$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.refreshGoals),
      mergeMap(() => [
        GoalsActions.loadGoals({}),
        GoalsActions.loadGoalTemplates(),
        GoalsActions.loadGoalAnalytics({}),
        GoalsActions.loadGoalSuggestions({})
      ])
    )
  );

  /**
   * Auto-check Overdue Goals Effect - periodically checks for overdue goals
   * Like having a personal assistant who reminds you about missed deadlines
   */
  autoCheckOverdueGoals$ = createEffect(() =>
    timer(0, 60 * 60 * 1000).pipe( // Check every hour
      startWith(0),
      withLatestFrom(this.store.select(GoalsSelectors.selectOverdueGoals)),
      filter(([, overdueGoals]) => overdueGoals.length > 0),
      map(([, overdueGoals]) => {
        // Don't show notifications too frequently
        const now = new Date();
        const lastNotificationTime = localStorage.getItem('lastOverdueNotification');
        
        if (!lastNotificationTime || 
            (now.getTime() - parseInt(lastNotificationTime)) > 24 * 60 * 60 * 1000) {
          
          this.notificationService.showWarning(
            `You have ${overdueGoals.length} overdue goal${overdueGoals.length > 1 ? 's' : ''}. Time to get back on track! 💪`
          );
          
          localStorage.setItem('lastOverdueNotification', now.getTime().toString());
        }
        
        return { type: '[Goals] Overdue Check Completed' }; // Dummy action
      })
    ),
    { dispatch: false }
  );

  /**
   * Auto-suggest Goal Adjustments Effect - suggests adjustments for struggling goals
   * Like having an AI coach that notices when you're struggling and offers help
   */
  autoSuggestAdjustments$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GoalsActions.loadGoalsSuccess),
      withLatestFrom(this.store.select(GoalsSelectors.selectActiveGoals)),
      switchMap(([, activeGoals]) => {
        const strugglingGoals = activeGoals.filter(goal => {
          const progress = goal.progressPercentage || 0;
          const timeRemaining = new Date(goal.endDate).getTime() - Date.now();
          const totalTime = new Date(goal.endDate).getTime() - new Date(goal.startDate).getTime();
          const timeElapsed = totalTime - timeRemaining;
          const expectedProgress = (timeElapsed / totalTime) * 100;
          
          // Goal is struggling if actual progress is significantly behind expected progress
          return progress < (expectedProgress - 25) && expectedProgress > 25;
        });
        
        if (strugglingGoals.length > 0) {
          // Suggest adjustments for the most struggling goals
          return strugglingGoals.slice(0, 2).map(goal =>
            GoalsActions.requestGoalAdjustment({ 
              goalId: goal.id, 
              reason: 'auto_adaptive' 
            })
          );
        }
        
        return [];
      })
    )
  );
}
