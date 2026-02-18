import type { KeycloakConfig } from 'keycloak-js';

export const KEYCLOAK_CONFIG: KeycloakConfig = {
  url: 'http://localhost:8080',
  realm: 'DeskOneRealm',
  clientId: 'deskone-frontend'
};
