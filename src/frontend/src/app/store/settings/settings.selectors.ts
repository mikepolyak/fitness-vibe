import { createFeatureSelector, createSelector } from '@ngrx/store';
import { SettingsState } from './settings.reducer';

export const selectSettingsState = createFeatureSelector<SettingsState>('settings');

export const selectTheme = createSelector(
  selectSettingsState,
  state => state.theme
);

export const selectNotificationsEnabled = createSelector(
  selectSettingsState,
  state => state.notificationsEnabled
);

export const selectLanguage = createSelector(
  selectSettingsState,
  state => state.language
);

export const selectTimeZone = createSelector(
  selectSettingsState,
  state => state.timeZone
);

export const selectSettingsLoading = createSelector(
  selectSettingsState,
  state => state.loading
);

export const selectSettingsError = createSelector(
  selectSettingsState,
  state => state.error
);
