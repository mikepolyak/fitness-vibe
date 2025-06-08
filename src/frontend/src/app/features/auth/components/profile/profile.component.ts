import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router } from '@angular/router';

/**
 * Profile Component - User profile management center.
 * 
 * Think of this as the member services desk where users can:
 * - Update their personal information
 * - Modify their fitness goals and preferences
 * - Manage their privacy settings
 * - View their account status
 */
@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ProfileComponent implements OnInit {

  constructor(private router: Router) { }

  ngOnInit(): void {
    // Profile component implementation will be expanded in later phases
  }

  navigateToLogin(): void {
    this.router.navigate(['/auth/login']);
  }
}
