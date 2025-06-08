import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './components/dashboard/dashboard.component';
import { ActivitySummaryComponent } from './components/activity-summary/activity-summary.component';
import { GoalProgressComponent } from './components/goal-progress/goal-progress.component';
import { BadgeShowcaseComponent } from './components/badge-showcase/badge-showcase.component';
import { RecentActivitiesComponent } from './components/recent-activities/recent-activities.component';

@NgModule({
  declarations: [
    DashboardComponent,
    ActivitySummaryComponent,
    GoalProgressComponent,
    BadgeShowcaseComponent,
    RecentActivitiesComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,
    DashboardRoutingModule
  ]
})
export class DashboardModule { }
