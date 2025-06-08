import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { AnalyticsComponent } from './components/analytics/analytics.component';

@NgModule({
  declarations: [AnalyticsComponent],
  imports: [
    CommonModule,    RouterModule.forChild([
      { path: '', component: AnalyticsComponent }
    ]),
    SharedModule
  ]
})
export class AnalyticsModule { }
