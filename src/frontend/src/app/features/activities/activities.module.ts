import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { MatNativeDateModule } from '@angular/material/core';

import { ActivityListComponent } from './components/activity-list/activity-list.component';
import { ActivityFormComponent } from './components/activity-form/activity-form.component';
import { ActivityDetailsComponent } from './components/activity-details/activity-details.component';

@NgModule({
  declarations: [
    ActivityListComponent,
    ActivityFormComponent,
    ActivityDetailsComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild([
      {
        path: '',
        component: ActivityListComponent
      },
      {
        path: 'new',
        component: ActivityFormComponent
      },
      {
        path: ':id',
        component: ActivityDetailsComponent
      },
      {
        path: ':id/edit',
        component: ActivityFormComponent
      }
    ]),
    SharedModule,
    MatButtonModule,
    MatCardModule,
    MatDatepickerModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonToggleModule,
    MatMenuModule,
    MatIconModule,
    MatNativeDateModule
  ]
})
export class ActivitiesModule { }
