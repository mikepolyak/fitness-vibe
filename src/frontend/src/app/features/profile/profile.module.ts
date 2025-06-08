import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../../shared/shared.module';
import { ProfileComponent } from './components/profile/profile.component';

@NgModule({
  declarations: [ProfileComponent],
  imports: [
    CommonModule,    RouterModule.forChild([
      { path: '', component: ProfileComponent }
    ]),
    SharedModule
  ]
})
export class ProfileModule { }
