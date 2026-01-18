import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard: CanActivateFn = async () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  // Wait for auth initialization
  while (!authService.initialized()) {
    await new Promise(resolve => setTimeout(resolve, 50));
  }
  
  if (authService.isAuthenticated()) {
    return true;
  }
  
  return router.createUrlTree(['/auth/login']);
};

export const guestGuard: CanActivateFn = async () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  
  // Wait for auth initialization
  while (!authService.initialized()) {
    await new Promise(resolve => setTimeout(resolve, 50));
  }
  
  if (!authService.isAuthenticated()) {
    return true;
  }
  
  return router.createUrlTree(['/dashboard']);
};
