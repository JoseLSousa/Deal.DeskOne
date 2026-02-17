import { Component, DestroyRef, OnInit, computed, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { MatDialog } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { finalize } from 'rxjs/operators';

import { Request } from './request.model';
import { RequestsService } from './requests.service';
import { RequestModal } from './request-modal/request-modal';
import { RejectModal } from './reject-modal/reject-modal';
import { EditModal } from './edit-modal/edit-modal';
import { KeycloakService } from '../../auth/keycloak.service';

interface RequestView extends Request {
  categoryLabel: string;
  priorityLabel: string;
  statusLabel: string;
}


@Component({
  selector: 'app-dashboard',
  imports: [
    MatButtonModule,
    MatCardModule,
    MatChipsModule,
    MatIconModule,
    MatMenuModule,
    MatTableModule,
    CommonModule,
    RequestModal,
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard implements OnInit {
  readonly displayedColumns: string[] = [
    'title',
    'description',
    'category',
    'priority',
    'status',
    'createdAt',
    'actions',
  ];

  private readonly requestsService = inject(RequestsService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly keycloakService = inject(KeycloakService);
  private readonly dialog = inject(MatDialog);

  readonly requests = signal<Request[]>([]);
  readonly loading = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly searchTerm = signal('');
  readonly categoryFilter = signal<number | null>(null);
  readonly priorityFilter = signal<number | null>(null);
  readonly statusFilter = signal<number | null>(null);
  readonly showModal = signal(false);

  readonly isManager = computed(() => this.keycloakService.hasRole('Manager'));
  readonly isUser = computed(() => this.keycloakService.hasRole('User'));

  private readonly categoryLabels = new Map<number, string>([
    [0, 'Compra'],
    [1, 'Acesso'],
    [2, 'Reembolso'],
    [3, 'TI'],
    [4, 'Outro'],
  ]);

  private readonly priorityLabels = new Map<number, string>([
    [0, 'Baixa'],
    [1, 'Média'],
    [2, 'Alta'],
  ]);

  private readonly statusLabels = new Map<number, string>([
    [0, 'Pendente'],
    [1, 'Aprovado'],
    [2, 'Rejeitado'],
  ]);

  readonly filteredRequests = computed<Request[]>(() => {
    const term = this.searchTerm().trim().toLowerCase();
    return this.requests().filter((request) => {
      const matchesTerm = term
        ? `${request.title} ${request.description} ${request.id}`
            .toLowerCase()
            .includes(term)
        : true;
      const matchesCategory = this.categoryFilter() === null
        ? true
        : request.category === this.categoryFilter();
      const matchesPriority = this.priorityFilter() === null
        ? true
        : request.priority === this.priorityFilter();
      const matchesStatus = this.statusFilter() === null
        ? true
        : request.status === this.statusFilter();
      return matchesTerm && matchesCategory && matchesPriority && matchesStatus;
    });
  });

  readonly viewRows = computed<RequestView[]>(() =>
    this.filteredRequests().map((request) => ({
      ...request,
      categoryLabel: this.labelFor(this.categoryLabels, request.category),
      priorityLabel: this.labelFor(this.priorityLabels, request.priority),
      statusLabel: this.labelFor(this.statusLabels, request.status),
    })),
  );

  readonly totalCount = computed(() => this.filteredRequests().length);
  readonly totalAllCount = computed(() => this.requests().length);
  readonly highPriorityCount = computed(
    () => this.filteredRequests().filter((request) => request.priority === 2).length,
  );
  readonly pendingCount = computed(
    () => this.filteredRequests().filter((request) => request.status === 0).length,
  );

  readonly hasActiveFilters = computed(
    () =>
      this.searchTerm().trim().length > 0 ||
      this.categoryFilter() !== null ||
      this.priorityFilter() !== null ||
      this.statusFilter() !== null,
  );

  readonly chipLabel = computed(() =>
    this.hasActiveFilters()
      ? `Filtrado: ${this.totalCount()} de ${this.totalAllCount()}`
      : 'Online',
  );

  ngOnInit(): void {
    this.loadRequests();
  }

  updateSearch(term: string): void {
    this.searchTerm.set(term);
  }

  updateCategory(value: string): void {
    this.categoryFilter.set(this.parseFilterValue(value));
  }

  updatePriority(value: string): void {
    this.priorityFilter.set(this.parseFilterValue(value));
  }

  updateStatusFilter(value: string): void {
    this.statusFilter.set(this.parseFilterValue(value));
  }

  clearFilters(): void {
    this.searchTerm.set('');
    this.categoryFilter.set(null);
    this.priorityFilter.set(null);
    this.statusFilter.set(null);
  }

  openCreateModal(): void {
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  onRequestCreated(): void {
    this.loadRequests();
  }

  approveRequest(request: Request): void {
    if (!this.canApproveOrReject(request)) {
      return;
    }

    const userName = this.keycloakService.userName();
    if (!userName) {
      this.errorMessage.set('Usuário não identificado.');
      return;
    }

    this.requestsService
      .approveRequest(request.id, { approvedBy: userName })
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.requests.update((items) =>
            items.map((item) => (item.id === request.id ? { ...item, status: 1 } : item)),
          );
        },
        error: () => this.errorMessage.set('Falha ao aprovar solicitação.'),
      });
  }

  rejectRequest(request: Request): void {
    if (!this.canApproveOrReject(request)) {
      return;
    }

    const dialogRef = this.dialog.open(RejectModal);

    dialogRef.afterClosed().subscribe((reason: string | null) => {
      if (!reason) {
        return;
      }

      this.requestsService
        .rejectRequest(request.id, { reason })
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            this.requests.update((items) =>
              items.map((item) => (item.id === request.id ? { ...item, status: 2 } : item)),
            );
          },
          error: () => this.errorMessage.set('Falha ao rejeitar solicitação.'),
        });
    });
  }

  editRequest(request: Request): void {
    if (request.status !== 0) {
      this.errorMessage.set('Apenas solicitações com status "Pendente" podem ser editadas.');
      return;
    }

    const dialogRef = this.dialog.open(EditModal);
    const modalComponent = dialogRef.componentInstance;
    modalComponent.setRequest(request);

    dialogRef.afterClosed().subscribe((updatedData) => {
      if (!updatedData) {
        return;
      }

      this.requests.update((items) =>
        items.map((item) =>
          item.id === request.id
            ? {
                ...item,
                title: updatedData.title ?? item.title,
                description: updatedData.description ?? item.description,
                category: updatedData.category ?? item.category,
                priority: updatedData.priority ?? item.priority,
              }
            : item,
        ),
      );
    });
  }

  deleteRequest(request: Request): void {
    if (!confirm(`Tem certeza que deseja deletar a solicitação "${request.title}"?`)) {
      return;
    }

    this.requestsService
      .deleteRequest(request.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.requests.update((items) => items.filter((item) => item.id !== request.id));
          this.errorMessage.set('Solicitação deletada com sucesso.');
        },
        error: () => this.errorMessage.set('Falha ao deletar solicitação.'),
      });
  }

  private canApproveOrReject(request: Request): boolean {
    return request.status === 0;
  }

  private loadRequests(): void {
    this.loading.set(true);
    this.errorMessage.set(null);

    this.requestsService
      .getRequests()
      .pipe(
        takeUntilDestroyed(this.destroyRef),
        finalize(() => this.loading.set(false)),
      )
      .subscribe({
        next: (items) => this.requests.set(items),
        error: () => this.errorMessage.set('Falha ao carregar solicitacoes.'),
      });
  }

  private parseFilterValue(value: string): number | null {
    if (!value) {
      return null;
    }
    const parsed = Number(value);
    return Number.isNaN(parsed) ? null : parsed;
  }

  private labelFor(labels: Map<number, string>, value: number): string {
    return labels.get(value) ?? 'Nao definido';
  }
}
