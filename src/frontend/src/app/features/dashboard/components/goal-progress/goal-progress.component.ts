import { Component, OnInit } from '@angular/core';

interface Goal {
  id: number;
  name: string;
  target: number;
  current: number;
  unit: string;
  dueDate: Date;
}

@Component({
  selector: 'app-goal-progress',
  templateUrl: './goal-progress.component.html',
  styleUrls: ['./goal-progress.component.scss']
})
export class GoalProgressComponent implements OnInit {
  goals: Goal[] = [];

  constructor() { }

  ngOnInit(): void {
    // This will be replaced with actual API call
    this.goals = [
      {
        id: 1,
        name: 'Daily Steps',
        target: 10000,
        current: 8432,
        unit: 'steps',
        dueDate: new Date()
      },
      {
        id: 2,
        name: 'Weekly Running Distance',
        target: 20,
        current: 15.5,
        unit: 'km',
        dueDate: new Date(Date.now() + 7 * 24 * 60 * 60 * 1000)
      },
      {
        id: 3,
        name: 'Monthly Active Days',
        target: 20,
        current: 12,
        unit: 'days',
        dueDate: new Date(Date.now() + 30 * 24 * 60 * 60 * 1000)
      }
    ];
  }

  getProgressPercentage(goal: Goal): number {
    return Math.min(100, (goal.current / goal.target) * 100);
  }
}
