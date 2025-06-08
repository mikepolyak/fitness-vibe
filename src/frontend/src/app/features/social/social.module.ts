import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { SocialComponent } from './components/social/social.component';

@NgModule({
  declarations: [SocialComponent],
  imports: [
    CommonModule,    RouterModule.forChild([
      { path: '', component: SocialComponent }
    ]),
    FormsModule,
    SharedModule
  ]
})
export class SocialModule { }
