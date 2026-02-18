import {
  Component,
  ChangeDetectionStrategy,
  inject,
  computed
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';

import { KeycloakService } from '../../auth/keycloak.service';

@Component({
  selector: 'app-header',
  imports: [CommonModule, MatButtonModule, MatMenuModule, MatIconModule],
  templateUrl: './header.html',
  styleUrl: './header.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Header {
  private readonly keycloakService = inject(KeycloakService);

  readonly isAuthenticated = this.keycloakService.isAuthenticated;
  readonly userName = this.keycloakService.userName;

  readonly userInitials = computed(() => {
    const name = this.userName();
    if (!name) return '';
    return name
      .split(' ')
      .map((word) => word.charAt(0).toUpperCase())
      .slice(0, 2)
      .join('');
  });

  logout(): void {
    this.keycloakService.logout();
  }
}
