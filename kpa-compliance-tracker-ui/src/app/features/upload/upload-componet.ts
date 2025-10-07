import { Component, inject, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { UploadService } from '../../services/upload.service';
import { UploadTaskDto } from '../../models/upload-task-dto';

@Component({
  selector: 'app-upload',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './upload-component.html',
})
export class UploadComponent {
  private upload = inject(UploadService);
  private destroyRef = inject(DestroyRef);

  msg = '';
  s3Key: string | null = null;
  isBusy = false;

  async onFileChange(e: Event) {
    const file = (e.target as HTMLInputElement).files?.[0];
    if (!file) return;

    this.isBusy = true;
    this.msg = 'Validating…';
    this.s3Key = null;

    try {
      const text = await file.text();
      const payload = JSON.parse(text) as UploadTaskDto[];
      if (!Array.isArray(payload)) throw new Error('Root must be an array');

      this.upload.uploadTasks(payload)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: r => {
            this.msg = `Inserted ${r.inserted}, updated ${r.updated}.`;
            this.s3Key = r.s3Key || null;
            this.isBusy = false;
          },
          error: () => {
            this.msg = 'Upload failed. Check JSON format or server logs.';
            this.isBusy = false;
          }
        });
    } catch {
      this.msg = 'Invalid JSON file.';
      this.isBusy = false;
    }
  }
}
