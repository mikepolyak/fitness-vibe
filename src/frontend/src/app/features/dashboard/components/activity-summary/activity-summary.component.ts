import { Component, OnInit } from '@angular/core';

interface ActivitySummary {
  steps: number;
  calories: number;
  activeMinutes: number;
  distance: number;
}

@Component({
  selector: 'app-activity-summary',
  templateUrl: './activity-summary.component.html',
  styleUrls: ['./activity-summary.component.scss']
})
export class ActivitySummaryComponent implements OnInit {
  summary: ActivitySummary = {
    steps: 0,
    calories: 0,
    activeMinutes: 0,
    distance: 0
  };

  constructor() { }

  ngOnInit(): void {
    // This will be replaced with actual API call
    this.summary = {
      steps: 8432,
      calories: 420,
      activeMinutes: 45,
      distance: 5.2
    };
  }
}
