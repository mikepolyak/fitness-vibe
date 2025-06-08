import { createReducer, on } from '@ngrx/store';
import * as SettingsActions from './settings.actions';

export interface SettingsState {
  theme: 'light' | 'dark';
  notificationsEnabled: boolean;
  language: string;
  timeZone: string;
  loading: boolean;
  error: any;
}

export const initialState: SettingsState = {
  theme: 'light',
  notificationsEnabled: true,
  language: 'en',
  timeZone: Intl.DateTimeFormat().resolvedOptions().timeZone,
  loading: false,
  error: null
};

export const settingsReducer = createReducer(
  initialState,
  on(SettingsActions.loadSettings, state => ({
    ...state,
    loading: true
  })),
  on(SettingsActions.loadSettingsSuccess, (state, { settings }) => ({
    ...state,
    ...settings,
    loading: false,
    error: null
  })),
  on(SettingsActions.loadSettingsFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),  on(SettingsActions.toggleTheme, state => ({
    ...state,
    theme: state.theme === 'light' ? 'dark' as const : 'light' as const
  })),
  on(SettingsActions.toggleNotifications, (state, { enabled }) => ({
    ...state,
    notificationsEnabled: enabled
  })),
  on(SettingsActions.updateLanguage, (state, { language }) => ({
    ...state,
    language
  })),
  on(SettingsActions.updateTimeZone, (state, { timeZone }) => ({
    ...state,
    timeZone
  }))
);
