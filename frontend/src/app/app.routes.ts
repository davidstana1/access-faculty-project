import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login.component';
import { authGuard } from './auth.guard';
import { EmployeeListComponent } from './components/employee/employee-list/employee-list.component';
import { EmployeeAccessLogsComponent } from './components/employee/employee-access-logs/employee-access-logs.component';

export const routes: Routes = [
  { 
    path: '', 
    redirectTo: 'dashboard', 
    pathMatch: 'full' 
  },
  { 
    path: 'login',
    component: LoginComponent 
  },
  { path: 'employees',
    component: EmployeeListComponent,
     canActivate: [authGuard] 
  },
  {
    path: 'employees/:id/access-logs',
    component: EmployeeAccessLogsComponent,
    canActivate: [authGuard]
  },
  { 
    path: '**',
    redirectTo: 'login' 
  } // unknown routes redirect to login
];