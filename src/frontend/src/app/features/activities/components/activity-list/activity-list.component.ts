import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

interface Activity {
  id: number;
  type: 'running' | 'cycling' | 'swimming' | 'walking' | 'gym';
  title: string;
  description?: string;
  duration: number;
  distance?: number;
  calories: number;
  date: Date;
  intensity: 'low' | 'medium' | 'high';
  status: 'completed' | 'in-progress' | 'planned';
}

@Component({
  selector: 'app-activity-list',
  templateUrl: './activity-list.component.html',
  styleUrls: ['./activity-list.component.scss']
})
export class ActivityListComponent implements OnInit {
  activities: Activity[] = [];
  filteredActivities: Activity[] = [];
  selectedFilter: string = 'all';
  selectedType: string = 'all';

  constructor(private router: Router) { }

  ngOnInit() {
    // This will be replaced with actual API call
    this.activities = [
      {
        id: 1,
        type: 'running',
        title: 'Morning Run',
        description: 'Great morning run around the park',
        duration: 45,
        distance: 5.2,
        calories: 420,
        date: new Date(),
        intensity: 'medium',
        status: 'completed'
      },
      {
        id: 2,
        type: 'gym',
        title: 'Upper Body Workout',
        description: 'Focus on chest and shoulders',
        duration: 60,
        calories: 300,
        date: new Date(),
        intensity: 'high',
        status: 'in-progress'
      },
      {
        id: 3,
        type: 'cycling',
        title: 'Evening Ride',
        duration: 30,
        distance: 8.5,
        calories: 250,
        date: new Date(Date.now() + 24 * 60 * 60 * 1000),
        intensity: 'low',
        status: 'planned'
      }
    ];

    this.filterActivities();
  }

  filterActivities() {
    this.filteredActivities = this.activities.filter(activity => {
      const statusMatch = this.selectedFilter === 'all' || activity.status === this.selectedFilter;
      const typeMatch = this.selectedType === 'all' || activity.type === this.selectedType;
      return statusMatch && typeMatch;
    });
  }

  getActivityIcon(type: string): string {
    const icons: { [key: string]: string } = {
      running: 'fas fa-running',
      cycling: 'fas fa-bicycle',
      swimming: 'fas fa-swimming-pool',
      walking: 'fas fa-walking',
      gym: 'fas fa-dumbbell'
    };
    return icons[type] || 'fas fa-question';
  }

  onAddActivity() {
    this.router.navigate(['/activities/new']);
  }

  viewActivity(id: number) {
    this.router.navigate(['/activities', id]);
  }

  editActivity(id: number) {
    this.router.navigate(['/activities', id, 'edit']);
  }

  toggleStatus(activity: Activity) {
    if (activity.status === 'completed') {
      activity.status = 'in-progress';
    } else {
      activity.status = 'completed';
    }
    // This will be replaced with actual API call
  }
}
