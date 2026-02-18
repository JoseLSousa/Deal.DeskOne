import { Injectable, computed, signal } from '@angular/core';
import Keycloak, { KeycloakProfile } from 'keycloak-js';

import { KEYCLOAK_CONFIG } from './keycloak.config';

@Injectable({ providedIn: 'root' })
export class KeycloakService {
  private readonly keycloak = new Keycloak(KEYCLOAK_CONFIG);
  private readonly authenticated = signal(false);
  private readonly profile = signal<KeycloakProfile | null>(null);
  private readonly roles = signal<string[]>([]);
  private initPromise: Promise<void> | null = null;
  private refreshIntervalId: number | null = null;

  readonly isAuthenticated = computed(() => this.authenticated());
  readonly userName = computed(() => {
    const profile = this.profile();
    return profile?.username ?? profile?.email ?? null;
  });
  readonly userRoles = computed(() => this.roles());

  init(): Promise<void> {
    if (!this.initPromise) {
      this.initPromise = this.initialize();
    }

    return this.initPromise;
  }

  async ensureAuthenticated(): Promise<boolean> {
    await this.init();

    if (this.keycloak.authenticated) {
      this.authenticated.set(true);
      return true;
    }

    await this.login();
    return false;
  }

  async login(): Promise<void> {
    await this.keycloak.login({ redirectUri: globalThis.location.origin });
  }

  async logout(): Promise<void> {
    await this.keycloak.logout({ redirectUri: globalThis.location.origin });
  }

  hasRole(role: string): boolean {
    return this.roles().includes(role);
  }

  async getToken(): Promise<string | null> {
    await this.init();

    if (!this.keycloak.authenticated) {
      return null;
    }

    try {
      await this.keycloak.updateToken(30);
    } catch {
      this.authenticated.set(false);
      return null;
    }

    return this.keycloak.token ?? null;
  }

  private async initialize(): Promise<void> {
    this.keycloak.onAuthSuccess = () => {
      this.authenticated.set(true);
      void this.loadUserProfile();
    };

    this.keycloak.onAuthLogout = () => {
      this.authenticated.set(false);
      this.profile.set(null);
      this.roles.set([]);
    };

    this.keycloak.onAuthRefreshError = () => {
      this.authenticated.set(false);
    };

    const authenticated = await this.keycloak.init({
      onLoad: 'check-sso',
      pkceMethod: 'S256',
      checkLoginIframe: false
    });

    this.authenticated.set(authenticated);

    if (authenticated) {
      await this.loadUserProfile();
    }

    this.startTokenRefresh();
  }

  private async loadUserProfile(): Promise<void> {
    try {
      const profile = await this.keycloak.loadUserProfile();
      this.profile.set(profile);
      this.extractRoles();
    } catch {
      this.profile.set(null);
      this.roles.set([]);
    }
  }

  private extractRoles(): void {
    const tokenParsed = this.keycloak.tokenParsed as Record<string, unknown> | undefined;
    
    if (!tokenParsed) {
      this.roles.set([]);
      return;
    }

    const realmAccess = (tokenParsed['realm_access'] as Record<string, unknown>) ?? {};
    const realmRoles = (realmAccess['roles'] as string[]) ?? [];

    this.roles.set(realmRoles);
  }

  private startTokenRefresh(): void {
    if (this.refreshIntervalId !== null) {
      return;
    }

    this.refreshIntervalId = globalThis.setInterval(() => {
      void this.refreshToken();
    }, 30000);
  }

  private async refreshToken(): Promise<void> {
    if (!this.keycloak.authenticated) {
      return;
    }

    try {
      await this.keycloak.updateToken(30);
      this.authenticated.set(true);
    } catch {
      this.authenticated.set(false);
    }
  }
}
