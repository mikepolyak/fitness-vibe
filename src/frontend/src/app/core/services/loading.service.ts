import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

/**
 * Loading Service - the progress indicator system for our fitness app.
 * Think of this as the digital display that shows when gym equipment
 * or services are currently in use or processing.
 */
@Injectable({
  providedIn: 'root'
})
export class LoadingService {
  private loadingSubject = new BehaviorSubject<boolean>(false);
  private loadingMessages = new BehaviorSubject<string>('');
  
  // Track multiple loading states by key
  private loadingStates = new Map<string, boolean>();

  constructor() {}

  /**
   * Observable for global loading state.
   * Like watching the main activity board to see if anything is happening.
   */
  get isLoading$(): Observable<boolean> {
    return this.loadingSubject.asObservable();
  }

  /**
   * Observable for loading messages.
   * Like listening to announcements about what's currently processing.
   */
  get loadingMessage$(): Observable<string> {
    return this.loadingMessages.asObservable();
  }

  /**
   * Get current loading state.
   * Like checking if the gym is currently busy.
   */
  get isLoading(): boolean {
    return this.loadingSubject.value;
  }

  /**
   * Set global loading state.
   * Like turning the main activity indicator on or off.
   */
  setLoading(loading: boolean, message: string = ''): void {
    this.loadingSubject.next(loading);
    this.loadingMessages.next(message);
  }

  /**
   * Set loading state for a specific operation.
   * Like marking a specific piece of equipment as in use.
   */
  setLoadingFor(key: string, loading: boolean, message: string = ''): void {
    if (loading) {
      this.loadingStates.set(key, true);
    } else {
      this.loadingStates.delete(key);
    }

    // Update global loading state based on any active loading operations
    const hasAnyLoading = this.loadingStates.size > 0;
    this.setLoading(hasAnyLoading, message);
  }

  /**
   * Check if a specific operation is loading.
   * Like checking if a particular machine is currently in use.
   */
  isLoadingFor(key: string): boolean {
    return this.loadingStates.has(key);
  }

  /**
   * Show loading with message for async operation.
   * Like announcing what activity is currently in progress.
   */
  showLoading(message: string = 'Loading...'): void {
    this.setLoading(true, message);
  }

  /**
   * Hide loading indicator.
   * Like clearing the activity board when everything is complete.
   */
  hideLoading(): void {
    this.setLoading(false, '');
  }

  /**
   * Execute an operation with loading indicator.
   * Like automatically managing the in-use sign for equipment.
   */
  withLoading<T>(
    operation: () => Observable<T>, 
    message: string = 'Processing...',
    key?: string
  ): Observable<T> {
    return new Observable<T>(observer => {
      // Start loading
      if (key) {
        this.setLoadingFor(key, true, message);
      } else {
        this.setLoading(true, message);
      }

      // Execute operation
      const subscription = operation().subscribe({
        next: (value) => observer.next(value),
        error: (error) => {
          // Stop loading on error
          if (key) {
            this.setLoadingFor(key, false);
          } else {
            this.setLoading(false);
          }
          observer.error(error);
        },
        complete: () => {
          // Stop loading on completion
          if (key) {
            this.setLoadingFor(key, false);
          } else {
            this.setLoading(false);
          }
          observer.complete();
        }
      });

      // Return cleanup function
      return () => {
        subscription.unsubscribe();
        if (key) {
          this.setLoadingFor(key, false);
        } else {
          this.setLoading(false);
        }
      };
    });
  }

  /**
   * Clear all loading states.
   * Like resetting all equipment status indicators.
   */
  clearAllLoading(): void {
    this.loadingStates.clear();
    this.setLoading(false, '');
  }

  /**
   * Get all currently loading operations.
   * Like getting a list of all equipment currently in use.
   */
  getActiveLoadingOperations(): string[] {
    return Array.from(this.loadingStates.keys());
  }

  /**
   * Show minimal loading indicator (for quick operations).
   * Like a brief "processing" indicator for fast actions.
   */
  showQuickLoading(duration: number = 1000): void {
    this.setLoading(true, 'Processing...');
    setTimeout(() => {
      this.setLoading(false);
    }, duration);
  }
}
