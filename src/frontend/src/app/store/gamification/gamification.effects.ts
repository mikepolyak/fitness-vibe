import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { of, timer, merge } from 'rxjs';
import { 
  map, 
  exhaustMap, 
  catchError, 
  tap, 
  switchMap, 
  withLatestFrom,
  mergeMap,
  startWith,
  filter,
  delay
} from 'rxjs/operators';

import { NotificationService } from '../../core/services/notification.service';
import { GamificationService } from '../../core/services/gamification.service';
import { AppState } from '../app.state';
import * as GamificationActions from './gamification.actions';
import * as GamificationSelectors from './gamification.selectors';
import * as AuthSelectors from '../auth/auth.selectors';

/**
 * Gamification Effects - the dedicated game masters who handle all the behind-the-scenes
 * work for rewards, achievements, and motivational systems. Think of them as arcade operators
 * who manage points, prizes, leaderboards, and special events to keep players engaged!
 */
@Injectable()
export class GamificationEffects {

  constructor(
    private actions$: Actions,
    private store: Store<AppState>,
    private gamificationService: GamificationService,
    private notificationService: NotificationService
  ) {}

  /**
   * Earn Experience Points Effect - awards XP for various fitness activities
   * Like getting points in an arcade game for completing challenges
   */
  earnExperiencePoints$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.earnExperiencePoints),
      exhaustMap(({ points, source, sourceId, description }) =>
        this.gamificationService.earnExperiencePoints(points, source, sourceId, description).pipe(
          map(response => {
            // Show a subtle XP notification
            if (response.leveledUp) {
              // Level up is handled by its own effect/notification
              return GamificationActions.earnExperiencePointsSuccess({
                pointsEarned: response.pointsEarned,
                totalPoints: response.totalPoints,
                newLevel: response.newLevel,
                leveledUp: response.leveledUp,
                levelUpRewards: response.levelUpRewards
              });
            } else {
              // Just show XP earned
              this.notificationService.showSuccess(
                `+${response.pointsEarned} XP earned! 🌟`
              );
              return GamificationActions.earnExperiencePointsSuccess({
                pointsEarned: response.pointsEarned,
                totalPoints: response.totalPoints,
                leveledUp: false
              });
            }
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to award experience points. Please try again.'
            );
            return of(GamificationActions.earnExperiencePointsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Check Level Up Effect - verifies if user should level up based on XP
   * Like checking if a player has enough points to advance to the next level
   */
  checkLevelUp$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.checkLevelUp),
      switchMap(({ currentXP }) =>
        this.gamificationService.checkLevelUp(currentXP).pipe(
          map(response => {
            if (response.leveledUp) {
              this.notificationService.showSuccess(
                `🎉 Level ${response.newLevel} achieved! You're getting stronger! 💪`,
                8000
              );
              return GamificationActions.levelUpSuccess({
                newLevel: response.newLevel,
                previousLevel: response.previousLevel,
                rewards: response.rewards
              });
            }
            // No level up, return empty action
            return { type: '[Gamification] No Level Up Needed' };
          }),
          catchError(error => of(GamificationActions.earnExperiencePointsFailure({ 
            error: error.message 
          })))
        )
      )
    )
  );

  /**
   * Load Available Badges Effect - fetches all possible badges to earn
   * Like browsing the prize catalog in an arcade
   */
  loadAvailableBadges$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadAvailableBadges),
      switchMap(() =>
        this.gamificationService.getAvailableBadges().pipe(
          map(badges => GamificationActions.loadAvailableBadgesSuccess({ badges })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load available badges. Please try again.'
            );
            return of(GamificationActions.loadAvailableBadgesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load User Badges Effect - fetches badges user has already earned
   * Like checking your trophy collection
   */
  loadUserBadges$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadUserBadges),
      switchMap(() =>
        this.gamificationService.getUserBadges().pipe(
          map(userBadges => GamificationActions.loadUserBadgesSuccess({ userBadges })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load your badges. Please try again.'
            );
            return of(GamificationActions.loadUserBadgesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Earn Badge Effect - awards a new badge to the user
   * Like winning a special prize for accomplishing something remarkable
   */
  earnBadge$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.earnBadge),
      exhaustMap(({ badgeId, context, activityId, goalId }) =>
        this.gamificationService.earnBadge(badgeId, context, activityId, goalId).pipe(
          map(response => {
            this.notificationService.showSuccess(
              `🏆 Badge earned: "${response.userBadge.badge.name}"! +${response.experiencePointsBonus} XP bonus!`,
              6000
            );
            return GamificationActions.earnBadgeSuccess({
              userBadge: response.userBadge,
              experiencePointsBonus: response.experiencePointsBonus
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to award badge. Please try again.'
            );
            return of(GamificationActions.earnBadgeFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Check Badge Eligibility Effect - automatically checks if user earned any badges
   * Like an automated prize checker that runs after every game
   */
  checkBadgeEligibility$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.checkBadgeEligibility),
      switchMap(({ triggerType, triggerData }) =>
        this.gamificationService.checkBadgeEligibility(triggerType, triggerData).pipe(
          map(eligibleBadges => {
            // Automatically earn all eligible badges
            if (eligibleBadges.length > 0) {
              eligibleBadges.forEach(({ badgeId }) => {
                this.store.dispatch(GamificationActions.earnBadge({ 
                  badgeId, 
                  context: JSON.stringify(triggerData) 
                }));
              });
            }
            
            return GamificationActions.checkBadgeEligibilitySuccess({ eligibleBadges });
          }),
          catchError(error => of(GamificationActions.earnBadgeFailure({ 
            error: error.message 
          })))
        )
      )
    )
  );

  /**
   * Load Active Streaks Effect - fetches user's current activity streaks
   * Like checking your consistency records and combo meters
   */
  loadActiveStreaks$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadActiveStreaks),
      switchMap(() =>
        this.gamificationService.getActiveStreaks().pipe(
          map(streaks => GamificationActions.loadActiveStreaksSuccess({ streaks })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load your streaks. Please try again.'
            );
            return of(GamificationActions.loadActiveStreaksFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Update Streak Effect - updates streak counters based on activities
   * Like updating your combo counter when you perform consecutive actions
   */
  updateStreak$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.updateStreak),
      exhaustMap(({ streakType, increment, activityDate }) =>
        this.gamificationService.updateStreak(streakType, increment, activityDate).pipe(
          map(response => {
            if (response.streak.milestoneReached) {
              this.notificationService.showSuccess(
                `🔥 ${response.streak.currentCount} day streak! Milestone reached! +${response.streak.milestoneReached.reward.xp} XP!`,
                5000
              );
            } else if (response.streak.wasExtended) {
              this.notificationService.showSuccess(
                `🔥 Streak extended! ${response.streak.currentCount} days and counting!`
              );
            }
            
            return GamificationActions.updateStreakSuccess({ streak: response.streak });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to update streak. Please try again.'
            );
            return of(GamificationActions.updateStreakFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Break Streak Effect - handles when a streak is broken
   * Like resetting a combo meter but with encouragement to start again
   */
  breakStreak$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.breakStreak),
      exhaustMap(({ streakId, reason }) =>
        this.gamificationService.breakStreak(streakId, reason).pipe(
          map(response => {
            this.notificationService.showInfo(
              response.encouragementMessage
            );
            return GamificationActions.breakStreakSuccess({
              streakId,
              finalCount: response.finalCount,
              encouragementMessage: response.encouragementMessage
            });
          }),
          catchError(error => of(GamificationActions.updateStreakFailure({ 
            error: error.message 
          })))
        )
      )
    )
  );

  /**
   * Load Achievements Effect - fetches all possible achievements and progress
   * Like viewing the quest log in an RPG game
   */
  loadAchievements$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadAchievements),
      switchMap(() =>
        this.gamificationService.getAchievements().pipe(
          map(achievements => GamificationActions.loadAchievementsSuccess({ achievements })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load achievements. Please try again.'
            );
            return of(GamificationActions.loadAchievementsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Unlock Achievement Effect - awards an achievement to the user
   * Like completing a major quest and getting the reward
   */
  unlockAchievement$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.unlockAchievement),
      exhaustMap(({ achievementId, context }) =>
        this.gamificationService.unlockAchievement(achievementId, context).pipe(
          map(response => {
            this.notificationService.showSuccess(
              `🎯 Achievement unlocked: "${response.achievement.title}"! Amazing work!`,
              7000
            );
            return GamificationActions.unlockAchievementSuccess({ achievement: response.achievement });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to unlock achievement. Please try again.'
            );
            return of(GamificationActions.unlockAchievementFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Leaderboards Effect - fetches competitive rankings
   * Like checking the high score table in an arcade game
   */
  loadLeaderboards$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadLeaderboards),
      switchMap(({ type, scope, period }) =>
        this.gamificationService.getLeaderboards(type, scope, period).pipe(
          map(response => GamificationActions.loadLeaderboardsSuccess({
            leaderboardType: type,
            scope,
            period,
            entries: response.entries,
            userEntry: response.userEntry
          })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load leaderboards. Please try again.'
            );
            return of(GamificationActions.loadLeaderboardsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Virtual Currency Effect - fetches user's current coin balance and history
   * Like checking your arcade tokens or game credits
   */
  loadVirtualCurrency$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadVirtualCurrency),
      switchMap(() =>
        this.gamificationService.getVirtualCurrency().pipe(
          map(currency => GamificationActions.loadVirtualCurrencySuccess({ currency })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load virtual currency. Please try again.'
            );
            return of(GamificationActions.loadVirtualCurrencyFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Earn Virtual Currency Effect - awards coins for activities
   * Like getting tokens for playing games in an arcade
   */
  earnVirtualCurrency$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.earnVirtualCurrency),
      exhaustMap(({ amount, source, description }) =>
        this.gamificationService.earnVirtualCurrency(amount, source, description).pipe(
          map(response => {
            this.notificationService.showSuccess(
              `💰 +${response.amountEarned} coins earned! Balance: ${response.newBalance}`
            );
            return GamificationActions.earnVirtualCurrencySuccess({
              amountEarned: response.amountEarned,
              newBalance: response.newBalance,
              transactionId: response.transactionId
            });
          }),
          catchError(error => of(GamificationActions.loadVirtualCurrencyFailure({ 
            error: error.message 
          })))
        )
      )
    )
  );

  /**
   * Spend Virtual Currency Effect - handles purchases in the rewards store
   * Like buying prizes with your arcade tokens
   */
  spendVirtualCurrency$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.spendVirtualCurrency),
      exhaustMap(({ amount, itemId, itemType, itemName }) =>
        this.gamificationService.spendVirtualCurrency(amount, itemId, itemType, itemName).pipe(
          map(response => {
            this.notificationService.showSuccess(
              `🛍️ "${response.purchasedItem.name}" purchased! Enjoy your new item!`
            );
            return GamificationActions.spendVirtualCurrencySuccess({
              amountSpent: response.amountSpent,
              newBalance: response.newBalance,
              purchasedItem: response.purchasedItem,
              transactionId: response.transactionId
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              error.message || 'Purchase failed. Please try again.'
            );
            return of(GamificationActions.spendVirtualCurrencyFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Rewards Store Effect - fetches available items for purchase
   * Like browsing the prize counter at an arcade
   */
  loadRewardsStore$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadRewardsStore),
      switchMap(() =>
        this.gamificationService.getRewardsStore().pipe(
          map(response => GamificationActions.loadRewardsStoreSuccess({
            storeItems: response.storeItems,
            featuredItems: response.featuredItems,
            userOwnedItems: response.userOwnedItems
          })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load rewards store. Please try again.'
            );
            return of(GamificationActions.loadRewardsStoreFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Purchase Store Item Effect - handles buying items from the store
   * Like exchanging tokens for a specific prize
   */
  purchaseStoreItem$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.purchaseStoreItem),
      exhaustMap(({ itemId, paymentMethod }) =>
        this.gamificationService.purchaseStoreItem(itemId, paymentMethod).pipe(
          map(response => {
            this.notificationService.showSuccess(
              `🎁 "${response.item.name}" is now yours! Check your collection!`
            );
            return GamificationActions.purchaseStoreItemSuccess({
              item: response.item,
              transactionId: response.transactionId,
              newCurrencyBalance: response.newCurrencyBalance
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              error.message || 'Purchase failed. Please try again.'
            );
            return of(GamificationActions.purchaseStoreItemFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Daily Challenges Effect - fetches today's challenges and weekly challenge
   * Like getting your daily quests in an RPG
   */
  loadDailyChallenges$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadDailyChallenges),
      switchMap(() =>
        this.gamificationService.getDailyChallenges().pipe(
          map(response => GamificationActions.loadDailyChallengesSuccess({
            dailyChallenges: response.dailyChallenges,
            weeklyChallenge: response.weeklyChallenge
          })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load daily challenges. Please try again.'
            );
            return of(GamificationActions.loadDailyChallengesFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Complete Daily Challenge Effect - marks a challenge as completed and awards rewards
   * Like turning in a completed quest for rewards
   */
  completeDailyChallenge$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.completeDailyChallenge),
      exhaustMap(({ challengeId }) =>
        this.gamificationService.completeDailyChallenge(challengeId).pipe(
          map(response => {
            this.notificationService.showSuccess(
              `💫 Daily challenge completed! Earned ${response.reward.xp} XP and ${response.reward.virtualCurrency} coins!`,
              4000
            );
            return GamificationActions.completeDailyChallengeSuccess({
              challengeId,
              reward: response.reward,
              newChallenges: response.newChallenges
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to complete challenge. Please try again.'
            );
            return of(GamificationActions.completeDailyChallengeFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Seasonal Events Effect - fetches active seasonal events and competitions
   * Like checking for special holiday events in a game
   */
  loadSeasonalEvents$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadSeasonalEvents),
      switchMap(() =>
        this.gamificationService.getSeasonalEvents().pipe(
          map(activeEvents => GamificationActions.loadSeasonalEventsSuccess({ activeEvents })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load seasonal events. Please try again.'
            );
            return of(GamificationActions.loadSeasonalEventsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Participate in Seasonal Event Effect - enrolls user in a special event
   * Like signing up for a tournament or special competition
   */
  participateInSeasonalEvent$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.participateInSeasonalEvent),
      exhaustMap(({ eventId }) =>
        this.gamificationService.participateInSeasonalEvent(eventId).pipe(
          map(response => {
            this.notificationService.showSuccess(
              '🎊 You\'ve joined the seasonal event! Good luck and have fun!'
            );
            return GamificationActions.participateInSeasonalEventSuccess({
              eventId,
              participationReward: response.participationReward
            });
          }),
          catchError(error => {
            this.notificationService.showError(
              'Failed to join seasonal event. Please try again.'
            );
            return of(GamificationActions.loadSeasonalEventsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Load Gamification Analytics Effect - fetches comprehensive gamification statistics
   * Like viewing your player stats and achievements summary
   */
  loadGamificationAnalytics$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.loadGamificationAnalytics),
      switchMap(() =>
        this.gamificationService.getGamificationAnalytics().pipe(
          map(analytics => GamificationActions.loadGamificationAnalyticsSuccess({ analytics })),
          catchError(error => {
            this.notificationService.showError(
              'Failed to load analytics. Please try again.'
            );
            return of(GamificationActions.loadGamificationAnalyticsFailure({ error: error.message }));
          })
        )
      )
    )
  );

  /**
   * Refresh Gamification Data Effect - reloads all gamification data
   * Like refreshing your entire game profile and stats
   */
  refreshGamificationData$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.refreshGamificationData),
      mergeMap(() => [
        GamificationActions.loadAvailableBadges(),
        GamificationActions.loadUserBadges(),
        GamificationActions.loadActiveStreaks(),
        GamificationActions.loadAchievements(),
        GamificationActions.loadVirtualCurrency(),
        GamificationActions.loadRewardsStore(),
        GamificationActions.loadDailyChallenges(),
        GamificationActions.loadSeasonalEvents(),
        GamificationActions.loadGamificationAnalytics()
      ])
    )
  );

  /**
   * Auto-hide Reward Notification Effect - automatically hides notifications after a delay
   * Like making popup messages disappear after the player has seen them
   */
  autoHideNotification$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.showRewardNotification),
      filter(({ autoHide }) => autoHide === true),
      switchMap(({ duration = 5000 }) =>
        timer(duration).pipe(
          map(() => GamificationActions.hideRewardNotification())
        )
      )
    )
  );

  /**
   * Daily Streak Check Effect - automatically checks and updates daily streaks
   * Like a daily login bonus system that runs automatically
   */
  dailyStreakCheck$ = createEffect(() =>
    timer(0, 24 * 60 * 60 * 1000).pipe( // Check once per day
      startWith(0),
      withLatestFrom(this.store.select(AuthSelectors.selectIsAuthenticated)),
      filter(([, isAuthenticated]) => isAuthenticated),
      switchMap(() => 
        this.gamificationService.checkDailyStreaks().pipe(
          mergeMap(streakUpdates => 
            streakUpdates.map(update => 
              GamificationActions.updateStreakSuccess({ streak: update })
            )
          ),
          catchError(() => of({ type: '[Gamification] Daily Streak Check Failed' }))
        )
      )
    )
  );

  /**
   * Level Up Celebration Effect - triggers special effects and rewards for level ups
   * Like playing fanfare and showing special animations when leveling up
   */
  levelUpCelebration$ = createEffect(() =>
    this.actions$.pipe(
      ofType(GamificationActions.levelUpSuccess),
      tap(({ newLevel, rewards }) => {
        // Could trigger special UI animations or sounds here
        console.log(`🎉 LEVEL UP! Welcome to level ${newLevel}!`);
        
        // Dispatch any level-up rewards
        if (rewards) {
          rewards.forEach(reward => {
            if (reward.type === 'badge' && reward.value) {
              this.store.dispatch(GamificationActions.earnBadge({ 
                badgeId: reward.value.id 
              }));
            } else if (reward.type === 'virtual_currency' && reward.value) {
              this.store.dispatch(GamificationActions.earnVirtualCurrency({
                amount: reward.value,
                source: 'level_up',
                description: `Level ${newLevel} bonus`
              }));
            }
          });
        }
      })
    ),
    { dispatch: false }
  );

  /**
   * Auto-refresh Leaderboards Effect - periodically updates competitive rankings
   * Like refreshing the high score table to show current standings
   */
  autoRefreshLeaderboards$ = createEffect(() =>
    timer(0, 15 * 60 * 1000).pipe( // Every 15 minutes
      startWith(0),
      withLatestFrom(this.store.select(GamificationSelectors.selectCurrentView)),
      filter(([, currentView]) => currentView === 'leaderboards'),
      switchMap(() => merge(
        of(GamificationActions.loadLeaderboards({ 
          type: 'xp', scope: 'friends', period: 'weekly' 
        })),
        of(GamificationActions.loadLeaderboards({ 
          type: 'activities', scope: 'global', period: 'monthly' 
        }))
      ))
    )
  );

  /**
   * Auto-badge Detection Effect - automatically checks for badge eligibility after activities
   * Like an automated system that checks if you earned any new achievements
   */
  autoBadgeDetection$ = createEffect(() =>
    merge(
      // Check after activity completion
      this.actions$.pipe(
        ofType('[Activities] Create Activity Success'),
        map((action: any) => ({
          triggerType: 'activity_completed' as const,
          triggerData: action.activity
        }))
      ),
      // Check after goal achievement  
      this.actions$.pipe(
        ofType('[Goals] Complete Goal Success'),
        map((action: any) => ({
          triggerType: 'goal_achieved' as const,
          triggerData: action.goal
        }))
      ),
      // Check after streak milestone
      this.actions$.pipe(
        ofType(GamificationActions.updateStreakSuccess),
        filter((action) => !!action.streak.milestoneReached),
        map((action) => ({
          triggerType: 'streak_reached' as const,
          triggerData: action.streak
        }))
      )
    ).pipe(
      map(({ triggerType, triggerData }) =>
        GamificationActions.checkBadgeEligibility({ triggerType, triggerData })
      )
    )
  );
}
