import { HttpInterceptorFn, HttpRequest, HttpHandlerFn, HttpErrorResponse } from '@angular/common/http';
import { inject, Injector } from '@angular/core';
import { catchError, from, switchMap, throwError } from 'rxjs';
import { TokenService } from '../auth/token.service';
import { AuthService } from '../auth/auth.service';

let isRefreshing = false;

export const jwtInterceptor: HttpInterceptorFn = (req: HttpRequest<unknown>, next: HttpHandlerFn) => {
  const tokenService = inject(TokenService);
  const injector = inject(Injector); // Lazy inject to avoid circular dep
  
  // Skip auth endpoints
  if (req.url.includes('/auth/login') || 
      req.url.includes('/auth/register') || 
      req.url.includes('/auth/refresh')) {
    return next(req);
  }
  
  const accessToken = tokenService.getAccessToken();
  
  if (accessToken) {
    // Check if token needs refresh before making request
    if (tokenService.isTokenExpired(accessToken) && !isRefreshing) {
      const authService = injector.get(AuthService); // Get lazily
      isRefreshing = true;
      return from(authService.refreshToken()).pipe(
        switchMap((success) => {
          isRefreshing = false;
          if (success) {
            const newToken = tokenService.getAccessToken();
            const clonedReq = req.clone({
              setHeaders: { Authorization: `Bearer ${newToken}` }
            });
            return next(clonedReq);
          }
          return throwError(() => new Error('Token refresh failed'));
        }),
        catchError((error) => {
          isRefreshing = false;
          return throwError(() => error);
        })
      );
    }
    
    const clonedReq = req.clone({
      setHeaders: { Authorization: `Bearer ${accessToken}` }
    });
    
    return next(clonedReq).pipe(
      catchError((error: HttpErrorResponse) => {
        // Handle 401 - try to refresh token
        if (error.status === 401 && !isRefreshing) {
          const authService = injector.get(AuthService); // Get lazily
          isRefreshing = true;
          return from(authService.refreshToken()).pipe(
            switchMap((success) => {
              isRefreshing = false;
              if (success) {
                const newToken = tokenService.getAccessToken();
                const retryReq = req.clone({
                  setHeaders: { Authorization: `Bearer ${newToken}` }
                });
                return next(retryReq);
              }
              return throwError(() => error);
            }),
            catchError((refreshError) => {
              isRefreshing = false;
              return throwError(() => refreshError);
            })
          );
        }
        return throwError(() => error);
      })
    );
  }
  
  return next(req);
};
