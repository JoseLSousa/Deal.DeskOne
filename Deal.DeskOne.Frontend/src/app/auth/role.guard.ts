import { CanMatchFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { KeycloakService } from './keycloak.service';

export function roleGuard(requiredRoles: string[]): CanMatchFn {
  return () => {
    const keycloakService = inject(KeycloakService);
    const router = inject(Router);

    const hasRequiredRole = requiredRoles.some((role) =>
      keycloakService.hasRole(role),
    );

    if (hasRequiredRole) {
      return true;
    }

    return router.parseUrl('/');
  };
}
