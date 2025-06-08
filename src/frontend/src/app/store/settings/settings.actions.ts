import { createAction, props } from '@ngrx/store';

export const loadSettings = createAction('[Settings] Load Settings');
export const loadSettingsSuccess = createAction(
  '[Settings] Load Settings Success',
  props<{ settings: any }>()
);
export const loadSettingsFailure = createAction(
  '[Settings] Load Settings Failure',
  props<{ error: any }>()
);

export const updateSettings = createAction(
  '[Settings] Update Settings',
  props<{ settings: any }>()
);
export const updateSettingsSuccess = createAction(
  '[Settings] Update Settings Success',
  props<{ settings: any }>()
);
export const updateSettingsFailure = createAction(
  '[Settings] Update Settings Failure',
  props<{ error: any }>()
);

export const toggleTheme = createAction('[Settings] Toggle Theme');
export const toggleNotifications = createAction(
  '[Settings] Toggle Notifications',
  props<{ enabled: boolean }>()
);
export const updateLanguage = createAction(
  '[Settings] Update Language',
  props<{ language: string }>()
);
export const updateTimeZone = createAction(
  '[Settings] Update Time Zone',
  props<{ timeZone: string }>()
);
