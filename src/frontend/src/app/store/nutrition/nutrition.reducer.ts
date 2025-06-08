import { createReducer, on } from '@ngrx/store';
import * as NutritionActions from './nutrition.actions';

export interface NutritionState {
  entries: any[];
  currentDate: Date;
  loading: boolean;
  error: any;
}

export const initialState: NutritionState = {
  entries: [],
  currentDate: new Date(),
  loading: false,
  error: null
};

export const nutritionReducer = createReducer(
  initialState,
  on(NutritionActions.loadNutritionLog, (state, { date }) => ({
    ...state,
    currentDate: date,
    loading: true
  })),
  on(NutritionActions.loadNutritionLogSuccess, (state, { entries }) => ({
    ...state,
    entries,
    loading: false,
    error: null
  })),
  on(NutritionActions.loadNutritionLogFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(NutritionActions.addNutritionEntry, state => ({
    ...state,
    loading: true
  })),
  on(NutritionActions.addNutritionEntrySuccess, (state, { entry }) => ({
    ...state,
    entries: [...state.entries, entry],
    loading: false,
    error: null
  })),
  on(NutritionActions.addNutritionEntryFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(NutritionActions.updateNutritionEntry, state => ({
    ...state,
    loading: true
  })),
  on(NutritionActions.updateNutritionEntrySuccess, (state, { entry }) => ({
    ...state,
    entries: state.entries.map(e => e.id === entry.id ? entry : e),
    loading: false,
    error: null
  })),
  on(NutritionActions.updateNutritionEntryFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(NutritionActions.deleteNutritionEntry, state => ({
    ...state,
    loading: true
  })),
  on(NutritionActions.deleteNutritionEntrySuccess, (state, { id }) => ({
    ...state,
    entries: state.entries.filter(e => e.id !== id),
    loading: false,
    error: null
  })),
  on(NutritionActions.deleteNutritionEntryFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  }))
);
