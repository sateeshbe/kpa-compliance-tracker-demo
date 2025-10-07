// src/app/services/upload.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UploadTaskDto } from '../models/upload-task-dto';
import { environment } from '../../environments/environment';
import { UploadResponse } from '../models/upload-response-dto';


@Injectable({ providedIn: 'root' })
export class UploadService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiBaseUrl; // e.g., http://localhost:5073

  uploadTasks(payload: UploadTaskDto[]): Observable<UploadResponse> {
    return this.http.post<UploadResponse>(`${this.baseUrl}/api/upload`, payload);
  }
}
