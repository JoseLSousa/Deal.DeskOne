import { inject } from '@angular/core';
import { CanMatchFn } from '@angular/router';

import { KeycloakService } from './keycloak.service';

export const authGuard: CanMatchFn = async () => {
  const auth = inject(KeycloakService);
  return auth.ensureAuthenticated();
};
