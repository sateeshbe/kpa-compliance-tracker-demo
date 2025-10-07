import { Component, OnInit, inject, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { TasksService } from '../../services/tasks-service';
import { ComplianceTask } from '../../models/compliance-task.model';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './tasks-component.html',
})
export class TasksComponent implements OnInit {
  private tasksSvc = inject(TasksService);
  private destroyRef = inject(DestroyRef);

  rows: ComplianceTask[] = [];
  isLoading = false;
  savingId: string | null = null;
  errorMsg = '';

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading = true;
    this.errorMsg = '';

    this.tasksSvc.list()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: r => {
          this.rows = r;
          this.isLoading = false;
        },
        error: () => {
          this.errorMsg = 'Failed to load tasks.';
          this.isLoading = false;
        }
      });
  }

  save(row: ComplianceTask) {
    this.savingId = row.id;
    this.errorMsg = '';

    this.tasksSvc.update(row.id, row)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          // Optionally refresh updatedAt locally or show a toast
          this.savingId = null;
        },
        error: () => {
          this.errorMsg = 'Failed to save task.';
          this.savingId = null;
        }
      });
  }
}
