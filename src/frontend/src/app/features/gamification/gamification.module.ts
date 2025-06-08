import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { GamificationComponent } from './components/gamification/gamification.component';

@NgModule({
  declarations: [GamificationComponent],
  imports: [
    CommonModule,    RouterModule.forChild([
      { path: '', component: GamificationComponent }
    ]),
    SharedModule
  ]
})
export class GamificationModule { }
