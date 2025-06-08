import { Component, OnInit } from '@angular/core';

interface SettingsSection {
  id: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  activeSection: string = 'account';
  
  sections: SettingsSection[] = [
    { id: 'account', label: 'Account Settings', icon: 'person' },
    { id: 'preferences', label: 'Preferences', icon: 'tune' },
    { id: 'notifications', label: 'Notifications', icon: 'notifications' },
    { id: 'privacy', label: 'Privacy & Security', icon: 'security' },
    { id: 'appearance', label: 'Appearance', icon: 'palette' },
    { id: 'integrations', label: 'Integrations', icon: 'link' }
  ];

  settings = {
    account: {
      email: 'john.doe@example.com',
      phone: '+1 (555) 123-4567',
      language: 'English',
      timezone: 'UTC-5'
    },
    preferences: {
      measurementUnit: 'Metric',
      workoutReminders: true,
      weeklyReports: true,
      challengeInvites: true
    },
    notifications: {
      email: true,
      push: true,
      achievements: true,
      friendActivity: true,
      challenges: true
    },
    privacy: {
      profileVisibility: 'Friends',
      activitySharing: 'Public',
      showStats: true,
      allowTagging: true
    },
    appearance: {
      theme: 'Light',
      colorScheme: 'Default',
      compactMode: false
    },
    integrations: {
      googleFit: false,
      appleHealth: false,
      strava: false,
      fitbit: false
    }
  };

  constructor() { }

  ngOnInit(): void {
    // Initialize settings data
  }

  setActiveSection(sectionId: string): void {
    this.activeSection = sectionId;
  }

  isSectionActive(sectionId: string): boolean {
    return this.activeSection === sectionId;
  }

  saveSettings(): void {
    // Implement settings save logic
    console.log('Saving settings:', this.settings);
  }
}
