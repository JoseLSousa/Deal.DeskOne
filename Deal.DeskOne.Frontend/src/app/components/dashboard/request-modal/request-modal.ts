import {
  Component,
  ChangeDetectionStrategy,
  input,
  output,
  signal,
  inject
} from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  Validators,
  ReactiveFormsModule
} from '@angular/forms';

import { RequestsService } from '../requests.service';
import { CreateRequestCommand } from '../request.model';

export const REQUEST_CATEGORIES = [
  { value: 0, label: 'Compra' },
  { value: 1, label: 'Acesso' },
  { value: 2, label: 'Reembolso' },
  { value: 3, label: 'TI' },
  { value: 4, label: 'Outro' }
];

export const REQUEST_PRIORITIES = [
  { value: 0, label: 'Baixa' },
  { value: 1, label: 'Média' },
  { value: 2, label: 'Alta' }
];

@Component({
  selector: 'app-request-modal',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './request-modal.html',
  styleUrl: './request-modal.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RequestModal {
  private readonly fb = inject(FormBuilder);
  private readonly requestsService = inject(RequestsService);

  isOpen = input<boolean>(true);
  close = output<void>();
  requestCreated = output<void>();

  isLoading = signal(false);

  form: FormGroup = this.fb.group({
    title: ['', [Validators.required]],
    description: ['', [Validators.required]],
    category: ['', [Validators.required]],
    priority: ['', [Validators.required]]
  });

  onSubmit(): void {
    if (this.form.invalid) {
      return;
    }

    this.isLoading.set(true);

    const command: CreateRequestCommand = {
      title: this.form.get('title')?.value,
      description: this.form.get('description')?.value,
      category: parseInt(this.form.get('category')?.value, 10),
      priority: parseInt(this.form.get('priority')?.value, 10)
    };

    this.requestsService.createRequest(command).subscribe({
      next: () => {
        this.isLoading.set(false);
        this.requestCreated.emit();
        this.close.emit();
      },
      error: () => {
        this.isLoading.set(false);
      }
    });
  }

  onBackdropClick(): void {
    if (!this.isLoading()) {
      this.close.emit();
    }
  }
}
