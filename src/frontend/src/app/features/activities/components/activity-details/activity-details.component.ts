import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { Observable, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { 
  Activity, 
  UserActivity, 
  ActivityType, 
  ActivityCategory, 
  ActivityUtils 
} from '../../../../shared/models/activity.model';

interface WeatherInfo {
  temperature: number;
  condition: string;
  humidity: number;
}

interface RouteInfo {
  startPoint: string;
  endPoint: string;
  coordinates: [number, number][];
}

@Component({
  selector: 'app-activity-details',
  templateUrl: './activity-details.component.html',
  styleUrls: ['./activity-details.component.scss']
})
export class ActivityDetailsComponent implements OnInit {
  activity: UserActivity | null = null;
  private weatherSubject = new BehaviorSubject<WeatherInfo | null>(null);
  private routeSubject = new BehaviorSubject<RouteInfo | null>(null);
  
  weatherInfo$ = this.weatherSubject.asObservable();
  routeInfo$ = this.routeSubject.asObservable();

  constructor(
    private route: ActivatedRoute,
    private router: Router
  ) { }

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadActivity(parseInt(id));
    }
  }

  loadActivity(id: number) {
    // This will be replaced with actual API call
    const startTime = new Date();
    const endTime = new Date(startTime.getTime() + 45 * 60000); // 45 minutes later

    this.activity = {
      id: 1,
      userId: 1,
      activityId: 1,
      activity: {
        id: 1,
        name: 'Morning Run',
        description: 'Great morning run around the park. Weather was perfect and felt strong throughout.',
        type: ActivityType.Outdoor,
        category: ActivityCategory.Cardio,
        metValue: 7.0,
        isActive: true
      },
      startedAt: startTime,
      completedAt: endTime,
      duration: 45,
      location: 'Central Park',
      distance: 5.2,
      caloriesBurned: 420,
      averageHeartRate: 145,
      maxHeartRate: 165,
      averagePace: 8.5,
      elevationGain: 32,
      userRating: 5,
      notes: 'Felt strong throughout the run. Perfect weather conditions.',
      photos: [],
      isPublic: true,
      experiencePointsEarned: 100,
      routeData: JSON.stringify({
        startPoint: 'Central Park West',
        endPoint: 'Central Park East',
        coordinates: [[40.785091, -73.968285], [40.797091, -73.958285]]
      }),
      weatherConditions: JSON.stringify({
        temperature: 18,
        condition: 'Partly Cloudy',
        humidity: 65
      }),
      createdAt: new Date()
    };

    // Parse JSON data and update subjects
    if (this.activity.weatherConditions) {
      this.weatherSubject.next(JSON.parse(this.activity.weatherConditions));
    }
    if (this.activity.routeData) {
      this.routeSubject.next(JSON.parse(this.activity.routeData));
    }
  }

  getActivityIcon(category: ActivityCategory): string {
    return ActivityUtils.getActivityCategoryIcon(category);
  }

  goBack() {
    this.router.navigate(['/activities']);
  }

  editActivity() {
    if (this.activity) {
      this.router.navigate(['/activities', this.activity.id, 'edit']);
    }
  }

  shareActivity() {
    // This will be implemented later
    console.log('Share activity');
  }

  deleteActivity() {
    if (this.activity) {
      if (confirm('Are you sure you want to delete this activity?')) {
        // This will be replaced with actual API call
        console.log('Delete activity:', this.activity.id);
        this.router.navigate(['/activities']);
      }
    }
  }
}
