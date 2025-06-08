import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { GoalsComponent } from './components/goals/goals.component';

@NgModule({
  declarations: [GoalsComponent],
  imports: [
    CommonModule,    RouterModule.forChild([
      { path: '', component: GoalsComponent }
    ]),
    SharedModule
  ]
})
export class GoalsModule { }
