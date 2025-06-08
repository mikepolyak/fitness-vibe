import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { OnboardingComponent } from './components/onboarding/onboarding.component';

@NgModule({
  declarations: [OnboardingComponent],
  imports: [
    CommonModule,    RouterModule.forChild([
      { path: '', component: OnboardingComponent }
    ]),
    SharedModule
  ]
})
export class OnboardingModule { }
