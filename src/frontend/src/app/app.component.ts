import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Store } from '@ngrx/store';
import { Observable } from 'rxjs';
import { AppState } from './store/app.state';
import { selectIsAuthenticated, selectCurrentUser } from './store/auth/auth.selectors';
import { loadCurrentUser } from './store/auth/auth.actions';

/**
 * Root application component - the main container for our fitness journey app.
 * Think of this as the main stage where all the fitness magic happens.
 * It sets up the overall layout and coordinates between different features.
 */
@Component({
  selector: 'fv-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent implements OnInit {
  title = 'FitnessVibe';
  
  // Observable streams for reactive UI updates
  isAuthenticated$: Observable<boolean>;
  currentUser$: Observable<any>;

  constructor(private store: Store<AppState>) {
    // Set up our reactive data streams - like connecting to live fitness data feeds
    this.isAuthenticated$ = this.store.select(selectIsAuthenticated);
    this.currentUser$ = this.store.select(selectCurrentUser);
  }

  ngOnInit(): void {
    // On app startup, check if user is already logged in
    // Think of this as automatically logging into your fitness tracker when you open the app
    this.store.dispatch(loadCurrentUser());
  }
}
