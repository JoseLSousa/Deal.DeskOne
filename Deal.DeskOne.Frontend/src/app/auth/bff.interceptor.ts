import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { from, mergeMap } from 'rxjs';

import { KeycloakService } from './keycloak.service';

export const bffInterceptor: HttpInterceptorFn = (req, next) => {
  if (req.headers.has('Authorization')) {
    return next(req);
  }

  const auth = inject(KeycloakService);

  return from(auth.getToken()).pipe(
    mergeMap((token) => {
      if (!token) {
        return next(req);
      }

      const authRequest = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });

      return next(authRequest);
    })
  );
};
