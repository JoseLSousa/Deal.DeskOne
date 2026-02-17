import {
  Component,
  inject,
  ChangeDetectionStrategy,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatDialogRef } from '@angular/material/dialog';
import { signal } from '@angular/core';

import { Request, UpdateRequestCommand } from '../request.model';
import { RequestsService } from '../requests.service';

export const REQUEST_CATEGORIES = [
  { value: 0, label: 'Compra' },
  { value: 1, label: 'Acesso' },
  { value: 2, label: 'Reembolso' },
  { value: 3, label: 'TI' },
  { value: 4, label: 'Outro' },
];

export const REQUEST_PRIORITIES = [
  { value: 0, label: 'Baixa' },
  { value: 1, label: 'Média' },
  { value: 2, label: 'Alta' },
];

@Component({
  selector: 'app-edit-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './edit-modal.html',
  styleUrl: './edit-modal.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EditModal {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<EditModal>);
  private requestsService = inject(RequestsService);

  request = signal<Request | null>(null);
  isLoading = signal(false);

  categories = REQUEST_CATEGORIES;
  priorities = REQUEST_PRIORITIES;

  form = this.fb.group({
    title: ['', [Validators.required, Validators.minLength(3)]],
    description: ['', [Validators.required, Validators.minLength(10)]],
    category: ['', [Validators.required]],
    priority: ['', [Validators.required]],
  });

  titleControl = this.form.get('title');
  descriptionControl = this.form.get('description');
  categoryControl = this.form.get('category');
  priorityControl = this.form.get('priority');

  setRequest(request: Request): void {
    this.request.set(request);

    // Populate form with current values
    if (request.status === 0) {
      this.form.patchValue({
        title: request.title,
        description: request.description,
        category: String(request.category),
        priority: String(request.priority),
      });
    }
  }

  getStatusLabel(status: number): string {
    const statusLabels: Record<number, string> = {
      0: 'Pendente',
      1: 'Aprovado',
      2: 'Rejeitado',
    };
    return statusLabels[status] || 'Desconhecido';
  }

  onBackdropClick(): void {
    if (!this.isLoading()) {
      this.onCancel();
    }
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  onConfirm(): void {
    const req = this.request();
    if (!req || this.form.invalid || req.status !== 0) {
      return;
    }

    this.isLoading.set(true);

    const categoryValue = this.form.get('category')?.value;
    const priorityValue = this.form.get('priority')?.value;

    const command: UpdateRequestCommand = {
      title: this.form.get('title')?.value || undefined,
      description: this.form.get('description')?.value || undefined,
      category: categoryValue ? parseInt(categoryValue as string, 10) : undefined,
      priority: priorityValue ? parseInt(priorityValue as string, 10) : undefined,
    };

    this.requestsService.updateRequest(req.id, command).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.dialogRef.close(command);
      },
      error: () => {
        this.isLoading.set(false);
      },
    });
  }
}
