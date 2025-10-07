import { Routes } from '@angular/router';
import { TasksComponent } from './features/tasks/tasks-component';
import { UploadComponent } from './features/upload/upload-componet';


export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'upload' },
  { path: 'upload', component: UploadComponent },
  { path: 'tasks', component: TasksComponent }
];
