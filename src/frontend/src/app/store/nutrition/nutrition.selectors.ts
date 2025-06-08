import { createFeatureSelector, createSelector } from '@ngrx/store';
import { NutritionState } from './nutrition.reducer';

export const selectNutritionState = createFeatureSelector<NutritionState>('nutrition');

export const selectNutritionEntries = createSelector(
  selectNutritionState,
  state => state.entries
);

export const selectCurrentDate = createSelector(
  selectNutritionState,
  state => state.currentDate
);

export const selectNutritionLoading = createSelector(
  selectNutritionState,
  state => state.loading
);

export const selectNutritionError = createSelector(
  selectNutritionState,
  state => state.error
);

export const selectDailyNutritionSummary = createSelector(
  selectNutritionEntries,
  entries => ({
    totalCalories: entries.reduce((sum, entry) => sum + (entry.calories || 0), 0),
    totalProtein: entries.reduce((sum, entry) => sum + (entry.protein || 0), 0),
    totalCarbs: entries.reduce((sum, entry) => sum + (entry.carbs || 0), 0),
    totalFat: entries.reduce((sum, entry) => sum + (entry.fat || 0), 0)
  })
);
