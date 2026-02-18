import {
  Component,
  inject,
  ChangeDetectionStrategy,
  ChangeDetectorRef,
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { CommonModule } from '@angular/common';
import { MatDialogModule, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-reject-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatInputModule,
    MatFormFieldModule,
    MatDialogModule,
  ],
  template: `
    <div class="reject-modal">
      <h2 mat-dialog-title>Rejeitar Solicitação</h2>
      <mat-dialog-content>
        <form [formGroup]="form">
          <mat-form-field class="full-width">
            <mat-label>Motivo da rejeição*</mat-label>
            <textarea
              matInput
              formControlName="reason"
              placeholder="Descreva o motivo da rejeição..."
              rows="4"></textarea>
            @if (reasonControl?.invalid && reasonControl?.touched) {
              <mat-error>O motivo é obrigatório</mat-error>
            }
          </mat-form-field>
        </form>
      </mat-dialog-content>
      <mat-dialog-actions align="end">
        <button mat-button type="button" (click)="onCancel()">Cancelar</button>
        <button
          mat-raised-button
          color="warn"
          type="button"
          [disabled]="form.invalid"
          (click)="onConfirm()">
          Rejeitar
        </button>
      </mat-dialog-actions>
    </div>
  `,
  styles: `
    .reject-modal {
      padding: 20px;
      min-width: 400px;
    }

    .full-width {
      width: 100%;
    }

    mat-dialog-actions {
      margin-top: 20px;
    }
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RejectModal {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<RejectModal>);
  private cdr = inject(ChangeDetectorRef);

  form = this.fb.group({
    reason: ['', [Validators.required, Validators.minLength(5)]],
  });

  reasonControl = this.form.get('reason');

  constructor() {
    // Marcar para check quando o formulário mudar (para OnPush)
    this.form.valueChanges.subscribe(() => {
      this.cdr.markForCheck();
    });
    this.form.statusChanges.subscribe(() => {
      this.cdr.markForCheck();
    });
  }

  onCancel(): void {
    this.dialogRef.close(null);
  }

  onConfirm(): void {
    if (this.form.valid) {
      const reason = this.form.get('reason')?.value || '';
      this.dialogRef.close(reason);
    }
  }
}
