import {
  Component,
  ChangeDetectionStrategy,
  DestroyRef,
  OnInit,
  input,
  signal,
  inject,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { RequestHistory } from '../request-history.model';
import { RequestsService } from '../requests.service';

@Component({
  selector: 'app-request-history',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './request-history.html',
  styleUrl: './request-history.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RequestHistoryComponent implements OnInit {
  private readonly requestsService = inject(RequestsService);
  private readonly destroyRef = inject(DestroyRef);

  readonly requestId = input<string>('');
  readonly historicalEvents = signal<RequestHistory[]>([]);
  readonly loading = signal(false);

  ngOnInit(): void {
    const id = this.requestId();
    if (id) {
      this.loadHistory(id);
    }
  }

  loadHistory(requestId: string): void {
    this.loading.set(true);
    this.requestsService
      .getRequestHistory(requestId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (history) => {
          this.historicalEvents.set(history);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Erro ao carregar histórico:', err);
          this.loading.set(false);
        },
      });
  }

  getEventIcon(event: RequestHistory): string {
    // Map icons based on field name and new value
    const iconMap: Record<string, string> = {
      'Created': 'add_circle',
      'Approved': 'check_circle',
      'Rejected': 'cancel',
      'Updated': 'edit',
    };
    return iconMap[event.newValue] || 'info';
  }

  getEventColor(event: RequestHistory): string {
    // Map colors based on field name and new value
    const colorMap: Record<string, string> = {
      'Created': '#2196F3',
      'Approved': '#4CAF50',
      'Rejected': '#F44336',
      'Updated': '#FF9800',
    };
    return colorMap[event.newValue] || '#6b7280';
  }

  getEventDescription(event: RequestHistory): string {
    if (event.oldValue && event.newValue) {
      return `${event.fieldName}: "${event.oldValue}" → "${event.newValue}"`;
    }
    return `${event.fieldName}: ${event.newValue}`;
  }

  formatDate(dateString: string): string {
    try {
      const date = new Date(dateString);
      return new Intl.DateTimeFormat('pt-BR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      }).format(date);
    } catch {
      return dateString;
    }
  }
}

