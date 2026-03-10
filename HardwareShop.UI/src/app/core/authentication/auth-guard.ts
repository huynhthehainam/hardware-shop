import { inject } from '@angular/core';
import { ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';

export const authGuard = (route?: ActivatedRouteSnapshot, state?: RouterStateSnapshot) => {
  const auth = inject(AuthService);

  if (auth.check()) {
    return true;
  }

  const redirectUrl = state?.url ? `${window.location.origin}${state.url}` : undefined;
  auth.login(redirectUrl).subscribe();
  return false;
};
