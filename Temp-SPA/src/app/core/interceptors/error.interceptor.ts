import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';

/**
 * Global error handling interceptor
 * Catches HTTP errors and provides consistent error handling across the application
 */
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(
    private router: Router,
    private alertify: AlertifyService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400:
              // Bad Request - validation errors
              if (error.error?.errors) {
                // Model state errors from API
                const modalStateErrors = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key]);
                  }
                }
                this.alertify.error(modalStateErrors.flat().join('\n'));
              } else if (typeof error.error === 'object') {
                // Generic object error
                this.alertify.error('Bad Request');
              } else {
                // String error message
                this.alertify.error(error.error);
              }
              break;

            case 401:
              // Unauthorized - redirect to login
              this.alertify.error('Unauthorized');
              this.router.navigate(['/login']);
              break;

            case 403:
              // Forbidden - user doesn't have permission
              this.alertify.error('You do not have permission to access this resource');
              break;

            case 404:
              // Not Found
              this.router.navigate(['/not-found']);
              break;

            case 500:
              // Server Error
              if (error.error) {
                this.alertify.error('Server error occurred');
              }
              break;

            case 0:
              // Network error or CORS issue
              this.alertify.error('Network error - unable to connect to server');
              break;

            default:
              // Unexpected error
              this.alertify.error('An unexpected error occurred');
              console.error('HTTP Error:', error);
              break;
          }
        }
        return throwError(() => error);
      })
    );
  }
}
