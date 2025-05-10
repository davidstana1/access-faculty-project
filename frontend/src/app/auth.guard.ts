import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from './services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated) {
    return true;
  }

  // Salvăm URL-ul către care a încercat utilizatorul să navigheze pentru a-l redirecționa după autentificare
  router.navigate(['/login']);
  return false;
};