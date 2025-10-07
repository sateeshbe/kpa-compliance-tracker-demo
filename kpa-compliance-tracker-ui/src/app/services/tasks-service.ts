import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { ComplianceTask } from '../models/compliance-task.model';



@Injectable({ providedIn: 'root' })
export class TasksService {
  private http = inject(HttpClient);
  private baseUrl = environment.apiBaseUrl; 

  list(): Observable<ComplianceTask[]> {
    return this.http.get<ComplianceTask[]>(`${this.baseUrl}/api/tasks`);
  }

  update(id: string, task: ComplianceTask): Observable<ComplianceTask> {
    return this.http.put<ComplianceTask>(`${this.baseUrl}/api/tasks/${id}`, task);
  }
}
