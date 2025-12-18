import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);
  const alertify = inject(AlertifyService);

  if (authService.loggedIn() && authService.decodedToken?.role === 'Admin') {
    return true;
  }

  alertify.error('You shall not pass!!!!');
  router.navigate(['/home']);
  return false;
};
