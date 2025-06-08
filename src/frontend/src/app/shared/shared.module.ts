import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

// Angular Material Modules
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatChipsModule } from '@angular/material/chips';
import { MatBadgeModule } from '@angular/material/badge';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatDialogModule } from '@angular/material/dialog';
import { MatBottomSheetModule } from '@angular/material/bottom-sheet';

// Shared Components
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { UserAvatarComponent } from './components/user-avatar/user-avatar.component';
import { ProgressBarComponent } from './components/progress-bar/progress-bar.component';
import { BadgeComponent } from './components/badge/badge.component';
import { EmptyStateComponent } from './components/empty-state/empty-state.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';

// Shared Pipes
import { DurationPipe } from './pipes/duration.pipe';
import { DistancePipe } from './pipes/distance.pipe';
import { CaloriesPipe } from './pipes/calories.pipe';
import { TimeAgoPipe } from './pipes/time-ago.pipe';
import { SafeHtmlPipe } from './pipes/safe-html.pipe';

// Shared Directives
import { ClickOutsideDirective } from './directives/click-outside.directive';
import { InfiniteScrollDirective } from './directives/infinite-scroll.directive';
import { LazyLoadDirective } from './directives/lazy-load.directive';

/**
 * Shared Module - the common utilities and components for our fitness app.
 * Think of this as the shared equipment and facilities that all areas
 * of the gym can use: water fountains, restrooms, common tools, etc.
 */
@NgModule({
  declarations: [
    // Components
    LoadingSpinnerComponent,
    UserAvatarComponent,
    ProgressBarComponent,
    BadgeComponent,
    EmptyStateComponent,
    ConfirmDialogComponent,
    
    // Pipes
    DurationPipe,
    DistancePipe,
    CaloriesPipe,
    TimeAgoPipe,
    SafeHtmlPipe,
    
    // Directives
    ClickOutsideDirective,
    InfiniteScrollDirective,
    LazyLoadDirective
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    
    // Angular Material
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatBadgeModule,
    MatTooltipModule,
    MatDialogModule,
    MatBottomSheetModule
  ],
  exports: [
    // Re-export Angular modules for convenience
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    RouterModule,
    
    // Re-export Material modules
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatChipsModule,
    MatBadgeModule,
    MatTooltipModule,
    MatDialogModule,
    MatBottomSheetModule,
    
    // Export our components
    LoadingSpinnerComponent,
    UserAvatarComponent,
    ProgressBarComponent,
    BadgeComponent,
    EmptyStateComponent,
    ConfirmDialogComponent,
    
    // Export pipes
    DurationPipe,
    DistancePipe,
    CaloriesPipe,
    TimeAgoPipe,
    SafeHtmlPipe,
    
    // Export directives
    ClickOutsideDirective,
    InfiniteScrollDirective,
    LazyLoadDirective
  ]
})
export class SharedModule { }
