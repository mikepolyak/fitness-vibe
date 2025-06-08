import { Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import * as NutritionActions from './nutrition.actions';

@Injectable()
export class NutritionEffects {
  loadNutritionLog$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NutritionActions.loadNutritionLog),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ entries: [] }).pipe(
          map(response => NutritionActions.loadNutritionLogSuccess({ entries: response.entries })),
          catchError(error => of(NutritionActions.loadNutritionLogFailure({ error })))
        )
      )
    )
  );

  addNutritionEntry$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NutritionActions.addNutritionEntry),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ entry: action.entry }).pipe(
          map(response => NutritionActions.addNutritionEntrySuccess({ entry: response.entry })),
          catchError(error => of(NutritionActions.addNutritionEntryFailure({ error })))
        )
      )
    )
  );

  updateNutritionEntry$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NutritionActions.updateNutritionEntry),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ entry: { id: action.id, ...action.entry } }).pipe(
          map(response => NutritionActions.updateNutritionEntrySuccess({ entry: response.entry })),
          catchError(error => of(NutritionActions.updateNutritionEntryFailure({ error })))
        )
      )
    )
  );

  deleteNutritionEntry$ = createEffect(() =>
    this.actions$.pipe(
      ofType(NutritionActions.deleteNutritionEntry),
      mergeMap(action =>
        // TODO: Replace with actual service call
        of({ id: action.id }).pipe(
          map(() => NutritionActions.deleteNutritionEntrySuccess({ id: action.id })),
          catchError(error => of(NutritionActions.deleteNutritionEntryFailure({ error })))
        )
      )
    )
  );

  constructor(private actions$: Actions) {}
}
