import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EmployeeService, Employee, AccessLog } from '../../../services/employee/employee.service';
import { AuthService } from '../../../services/auth.service';
import { catchError, finalize } from 'rxjs/operators';
import { of } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-employee-access-logs',
  templateUrl: './employee-access-logs.component.html',
  styleUrl: './employee-access-logs.component.css',
  standalone: true,
  imports: [CommonModule]
})
export class EmployeeAccessLogsComponent implements OnInit {
  employeeId: number | undefined;
  employee: Employee | undefined;
  accessLogs: AccessLog[] = [];
  loading = true;
  error = '';
  
  constructor(
    private route: ActivatedRoute,
    private employeeService: EmployeeService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.employeeId = +(this.route.snapshot.paramMap.get('id')!);
    this.loadEmployeeData();
    this.loadAccessLogs();
  }

  loadEmployeeData(): void {
    if (this.employeeId !== undefined) {
      this.employeeService.getEmployee(this.employeeId)
        .pipe(
          catchError(error => {
            this.error = 'Failed to load employee data';
            return of(null);
          })
        )
        .subscribe(employee => {
          if (employee) {
            this.employee = employee;
          }
        });
    } else {
      this.error = 'Invalid employee ID';
    }
  }

  loadAccessLogs(): void {
    this.loading = true;
    if (this.employeeId !== undefined) {
      this.employeeService.getEmployeeAccessLogs(this.employeeId)
        .pipe(
          catchError(error => {
            this.error = 'Failed to load access logs';
            return of([]);
          }),
          finalize(() => {
            this.loading = false;
          })
        )
        .subscribe(logs => {
          this.accessLogs = logs;
        });
    } else {
      this.error = 'Invalid employee ID';
      this.loading = false;
    }
  }

  formatDateTime(date: Date): string {
    return new Date(date).toLocaleString();
  }

  getDirectionClass(direction: string): string {
    return direction === 'Entry' ? 'direction-entry' : 'direction-exit';
  }

  // getScheduleClass(isWithinSchedule: boolean): string {
  //   return isWithinSchedule ? 'schedule-ok' : 'schedule-outside';
  // }

  isHr(): boolean {
    return this.authService.currentUserValue?.roles?.includes('HR') ?? false;
  }

  isGatePersonnel(): boolean {
    return this.authService.currentUserValue?.roles?.includes('GatePersonnel') ?? false;
  }

  isManager(): boolean {
    return this.authService.currentUserValue?.roles?.includes('Manager') ?? false;
  }

  goBack(): void {
    window.history.back();
  }
}
