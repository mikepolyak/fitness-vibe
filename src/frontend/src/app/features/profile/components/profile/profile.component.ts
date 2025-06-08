import { Component, OnInit } from '@angular/core';

interface ProfileTab {
  id: string;
  label: string;
  icon: string;
}

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  activeTab: string = 'overview';
  
  tabs: ProfileTab[] = [
    { id: 'overview', label: 'Overview', icon: 'person' },
    { id: 'activities', label: 'Activities', icon: 'fitness_center' },
    { id: 'achievements', label: 'Achievements', icon: 'emoji_events' },
    { id: 'stats', label: 'Statistics', icon: 'bar_chart' }
  ];

  userProfile = {
    name: 'John Doe',
    level: 15,
    points: 2750,
    joinedDate: new Date(2025, 0, 1),
    activities: 48,
    achievements: 12,
    streakDays: 7
  };

  constructor() { }

  ngOnInit(): void {
    // Initialize profile data
  }

  setActiveTab(tabId: string): void {
    this.activeTab = tabId;
  }

  isTabActive(tabId: string): boolean {
    return this.activeTab === tabId;
  }
}
