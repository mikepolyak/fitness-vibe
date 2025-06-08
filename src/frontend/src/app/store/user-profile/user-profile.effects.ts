import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import * as UserProfileActions from './user-profile.actions';

@Injectable()
export class UserProfileEffects {
  loadUserProfile$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserProfileActions.loadUserProfile),
      mergeMap(() =>
        // TODO: Replace with actual service call
        of({ profile: {} }).pipe(
          map(response => UserProfileActions.loadUserProfileSuccess({ profile: response.profile })),
          catchError(error => of(UserProfileActions.loadUserProfileFailure({ error })))
        )
      )
    )
  );

  updateUserProfile$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserProfileActions.updateUserProfile),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ profile: action.profile }).pipe(
          map(response => UserProfileActions.updateUserProfileSuccess({ profile: response.profile })),
          catchError(error => of(UserProfileActions.updateUserProfileFailure({ error })))
        )
      )
    )
  );

  updateUserPreferences$ = createEffect(() =>
    this.actions$.pipe(
      ofType(UserProfileActions.updateUserPreferences),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ preferences: action.preferences }).pipe(
          map(response => UserProfileActions.updateUserPreferencesSuccess({ preferences: response.preferences })),
          catchError(error => of(UserProfileActions.updateUserPreferencesFailure({ error })))
        )
      )
    )
  );

  constructor(private actions$: Actions) {}
}
