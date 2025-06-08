import { createAction, props } from '@ngrx/store';

export const loadNutritionLog = createAction(
  '[Nutrition] Load Nutrition Log',
  props<{ date: Date }>()
);
export const loadNutritionLogSuccess = createAction(
  '[Nutrition] Load Nutrition Log Success',
  props<{ entries: any[] }>()
);
export const loadNutritionLogFailure = createAction(
  '[Nutrition] Load Nutrition Log Failure',
  props<{ error: any }>()
);

export const addNutritionEntry = createAction(
  '[Nutrition] Add Entry',
  props<{ entry: any }>()
);
export const addNutritionEntrySuccess = createAction(
  '[Nutrition] Add Entry Success',
  props<{ entry: any }>()
);
export const addNutritionEntryFailure = createAction(
  '[Nutrition] Add Entry Failure',
  props<{ error: any }>()
);

export const updateNutritionEntry = createAction(
  '[Nutrition] Update Entry',
  props<{ id: string; entry: any }>()
);
export const updateNutritionEntrySuccess = createAction(
  '[Nutrition] Update Entry Success',
  props<{ entry: any }>()
);
export const updateNutritionEntryFailure = createAction(
  '[Nutrition] Update Entry Failure',
  props<{ error: any }>()
);

export const deleteNutritionEntry = createAction(
  '[Nutrition] Delete Entry',
  props<{ id: string }>()
);
export const deleteNutritionEntrySuccess = createAction(
  '[Nutrition] Delete Entry Success',
  props<{ id: string }>()
);
export const deleteNutritionEntryFailure = createAction(
  '[Nutrition] Delete Entry Failure',
  props<{ error: any }>()
);
