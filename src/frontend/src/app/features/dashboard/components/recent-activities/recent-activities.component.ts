import { Component, OnInit } from '@angular/core';

interface Activity {
  id: number;
  type: 'running' | 'cycling' | 'swimming' | 'walking' | 'gym';
  title: string;
  duration: number;
  distance?: number;
  calories: number;
  date: Date;
}

@Component({
  selector: 'app-recent-activities',
  templateUrl: './recent-activities.component.html',
  styleUrls: ['./recent-activities.component.scss']
})
export class RecentActivitiesComponent implements OnInit {
  recentActivities: Activity[] = [];

  constructor() { }

  ngOnInit(): void {
    // This will be replaced with actual API call
    this.recentActivities = [
      {
        id: 1,
        type: 'running',
        title: 'Morning Run',
        duration: 45,
        distance: 5.2,
        calories: 420,
        date: new Date(Date.now() - 2 * 60 * 60 * 1000)
      },
      {
        id: 2,
        type: 'gym',
        title: 'Strength Training',
        duration: 60,
        calories: 300,
        date: new Date(Date.now() - 24 * 60 * 60 * 1000)
      },
      {
        id: 3,
        type: 'cycling',
        title: 'Evening Ride',
        duration: 30,
        distance: 8.5,
        calories: 250,
        date: new Date(Date.now() - 2 * 24 * 60 * 60 * 1000)
      }
    ];
  }

  getActivityIcon(type: Activity['type']): string {
    const icons = {
      running: 'fas fa-running',
      cycling: 'fas fa-bicycle',
      swimming: 'fas fa-swimming-pool',
      walking: 'fas fa-walking',
      gym: 'fas fa-dumbbell'
    };
    return icons[type] || 'fas fa-question';
  }
}
