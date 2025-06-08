import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import * as SettingsActions from './settings.actions';

@Injectable()
export class SettingsEffects {
  loadSettings$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SettingsActions.loadSettings),
      mergeMap(() =>
        // TODO: Replace with actual service call
        of({
          settings: {
            theme: 'light',
            notificationsEnabled: true,
            language: 'en',
            timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone
          }
        }).pipe(
          map(response => SettingsActions.loadSettingsSuccess({ settings: response.settings })),
          catchError(error => of(SettingsActions.loadSettingsFailure({ error })))
        )
      )
    )
  );

  updateSettings$ = createEffect(() =>
    this.actions$.pipe(
      ofType(SettingsActions.updateSettings),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ settings: action.settings }).pipe(
          map(response => SettingsActions.updateSettingsSuccess({ settings: response.settings })),
          catchError(error => of(SettingsActions.updateSettingsFailure({ error })))
        )
      )
    )
  );

  constructor(private actions$: Actions) {}
}
