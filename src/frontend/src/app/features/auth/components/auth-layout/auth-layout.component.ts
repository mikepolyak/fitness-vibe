import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { Observable } from 'rxjs';
import { filter, map } from 'rxjs/operators';

/**
 * Auth Layout Component - The welcoming entrance to our fitness community.
 * 
 * Think of this as the beautiful lobby of a premium fitness club:
 * - Inspiring visuals that motivate people to join
 * - Clean, modern design that builds trust
 * - Clear navigation between different services
 * - Consistent branding and messaging
 */
@Component({
  selector: 'app-auth-layout',
  templateUrl: './auth-layout.component.html',
  styleUrls: ['./auth-layout.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AuthLayoutComponent implements OnInit {
  
  // Observable for the current page title and description
  pageData$: Observable<{ title: string; description: string }>;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute
  ) {
    // Listen to route changes to update page metadata
    this.pageData$ = this.router.events.pipe(
      filter(event => event instanceof NavigationEnd),
      map(() => {
        let route = this.activatedRoute;
        while (route.firstChild) {
          route = route.firstChild;
        }
        return route.snapshot.data as { title: string; description: string };
      })
    );
  }

  ngOnInit(): void {
    // Any initialization logic for the auth layout
  }

  /**
   * Navigation helper methods
   */
  navigateToLogin(): void {
    this.router.navigate(['/auth/login']);
  }

  navigateToRegister(): void {
    this.router.navigate(['/auth/register']);
  }

  navigateToHome(): void {
    this.router.navigate(['/']);
  }
}
