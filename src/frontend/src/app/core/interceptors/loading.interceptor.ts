import { Injectable } from '@angular/core';
import { 
  HttpRequest, 
  HttpHandler, 
  HttpEvent, 
  HttpInterceptor, 
  HttpResponse 
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap, finalize } from 'rxjs/operators';

import { LoadingService } from '../services/loading.service';

/**
 * Loading Interceptor - the activity monitor for our fitness app.
 * Think of this as the staff member who tracks when equipment is in use,
 * showing loading indicators to let users know something is happening.
 */
@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  private totalRequests = 0;

  constructor(private loadingService: LoadingService) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    // Skip loading indicator for certain requests
    if (this.shouldSkipLoading(request)) {
      return next.handle(request);
    }

    // Increment total requests and show loading
    this.totalRequests++;
    this.loadingService.setLoading(true);

    return next.handle(request).pipe(
      tap((event: HttpEvent<unknown>) => {
        // Handle successful responses
        if (event instanceof HttpResponse) {
          this.handleRequestComplete();
        }
      }),
      finalize(() => {
        // Always called when request completes (success or error)
        this.handleRequestComplete();
      })
    );
  }

  /**
   * Determine if loading indicator should be skipped for this request.
   * Like knowing which gym activities don't need progress tracking.
   */
  private shouldSkipLoading(request: HttpRequest<any>): boolean {
    // Skip for requests that should be silent
    if (request.headers.get('X-Skip-Loading') === 'true') {
      return true;
    }

    // Skip for certain URL patterns
    const skipPatterns = [
      '/heartbeat',
      '/ping',
      '/health',
      '/analytics',
      '/tracking'
    ];

    return skipPatterns.some(pattern => request.url.includes(pattern));
  }

  /**
   * Handle request completion by updating loading state.
   * Like updating the equipment status board when someone finishes.
   */
  private handleRequestComplete(): void {
    this.totalRequests--;
    
    if (this.totalRequests === 0) {
      this.loadingService.setLoading(false);
    }
  }
}
