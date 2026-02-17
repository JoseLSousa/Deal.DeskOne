import { Component, Input, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatCardModule } from '@angular/material/card';

import { RequestHistory, StatusLabels } from '../request-history.model';
import { RequestsService } from '../requests.service';

@Component({
  selector: 'app-request-history',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatListModule,
    MatDividerModule,
    MatCardModule
  ],
  template: `
    <mat-card class="history-card" *ngIf="historicalEvents().length > 0">
      <mat-card-header>
        <mat-card-title>Histórico de Mudanças</mat-card-title>
      </mat-card-header>

      <mat-card-content>
        <mat-list>
          @for (item of historicalEvents(); track item.id; let last = $last) {
            <mat-list-item class="history-item">
              <mat-icon 
                matListItemIcon 
                [color]="getStatusColor(item.toStatus)"
                class="status-icon">
                {{ getStatusIcon(item.toStatus) }}
              </mat-icon>

              <div matListItemTitle class="status-transition">
                {{ getStatusLabel(item.fromStatus) }} 
                <mat-icon class="arrow-icon">arrow_forward</mat-icon>
                {{ getStatusLabel(item.toStatus) }}
              </div>

              <div matListItemLine class="changed-by">
                <strong>Por:</strong> {{ item.changedBy | slice:0:8 }}...
              </div>

              <div matListItemLine class="changed-at">
                <strong>Data:</strong> {{ item.changedAt | date:'dd/MM/yyyy HH:mm:ss' }}
              </div>

              @if (item.comment) {
                <div matListItemLine class="comment">
                  <strong>Comentário:</strong> {{ item.comment }}
                </div>
              }
            </mat-list-item>

            @if (!last) {
              <mat-divider></mat-divider>
            }
          }
        </mat-list>
      </mat-card-content>
    </mat-card>

    @if (historicalEvents().length === 0) {
      <div class="no-history">
        <mat-icon>history</mat-icon>
        <p>Nenhuma mudança de status registrada</p>
      </div>
    }
  `,
  styles: [`
    .history-card {
      margin-top: 24px;
      background-color: #fafafa;
    }

    mat-card-header {
      background-color: #f5f5f5;
      padding: 16px;
      border-bottom: 1px solid #e0e0e0;
    }

    mat-card-title {
      margin: 0;
      font-size: 18px;
      font-weight: 600;
      color: #333;
    }

    mat-card-content {
      padding: 0;
    }

    mat-list {
      background-color: white;
    }

    .history-item {
      padding: 12px 16px;
    }

    .status-transition {
      display: flex;
      align-items: center;
      gap: 8px;
      font-weight: 600;
      color: #333;
    }

    .arrow-icon {
      font-size: 18px;
      width: 18px;
      height: 18px;
      color: #999;
    }

    .status-icon {
      margin-right: 12px;
    }

    .changed-by,
    .changed-at,
    .comment {
      font-size: 12px;
      color: #666;
      margin-top: 4px;
    }

    .comment {
      color: #555;
      font-style: italic;
      word-break: break-word;
    }

    .no-history {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      padding: 48px 16px;
      color: #999;
      background-color: white;
      border-radius: 4px;
      text-align: center;
    }

    .no-history mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      margin-bottom: 16px;
      opacity: 0.5;
    }

    .no-history p {
      margin: 0;
      font-size: 14px;
    }
  `]
})
export class RequestHistoryComponent implements OnInit {
  @Input() requestId: string = '';

  private readonly requestsService = inject(RequestsService);

  readonly historicalEvents = signal<RequestHistory[]>([]);
  readonly loading = signal(false);

  ngOnInit() {
    if (this.requestId) {
      this.loadHistory();
    }
  }

  private loadHistory() {
    this.loading.set(true);
    this.requestsService.getRequestHistory(this.requestId).subscribe({
      next: (history) => {
        this.historicalEvents.set(history);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Erro ao carregar histórico:', err);
        this.loading.set(false);
      }
    });
  }

  getStatusLabel(status: number): string {
    return StatusLabels[status] || 'Desconhecido';
  }

  getStatusColor(status: number): string {
    switch (status) {
      case 0: return 'warn';
      case 1: return 'accent';
      case 2: return 'primary';
      default: return 'primary';
    }
  }

  getStatusIcon(status: number): string {
    switch (status) {
      case 0: return 'hourglass_empty';
      case 1: return 'check_circle';
      case 2: return 'cancel';
      default: return 'info';
    }
  }
}
