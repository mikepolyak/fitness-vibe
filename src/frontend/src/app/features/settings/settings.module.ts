import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { SettingsComponent } from './components/settings/settings.component';

@NgModule({
  declarations: [SettingsComponent],
  imports: [
    CommonModule,    RouterModule.forChild([
      { path: '', component: SettingsComponent }
    ]),
    FormsModule,
    SharedModule
  ]
})
export class SettingsModule { }
