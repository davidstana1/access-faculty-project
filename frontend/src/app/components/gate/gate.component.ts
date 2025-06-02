import { Component, OnInit, OnDestroy } from '@angular/core';
import { interval, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AccessRequestService } from '../../services/access/access-request.service';
import { EmployeeService } from '../../services/employee/employee.service';
import { CommonModule, formatDate } from '@angular/common';

enum GateState {
  CLOSED = 'closed',
  OPEN = 'open',
  OPENING = 'opening',
  CLOSING = 'closing'
}

enum AccessDirection {
  ENTRY = 'Entry',
  EXIT = 'Exit'
}

interface AccessRequest {
  id: string;
  employeeId: number;
  employee?: any;
  timestamp: Date;
  direction: AccessDirection;
  method: string;
  vehicleNumber?: string;
  // isWithinSchedule: boolean;
}

@Component({
  selector: 'app-gate',
  templateUrl: './gate.component.html',
  styleUrls: ['./gate.component.css'],
  standalone: true,
  imports: [
    CommonModule
  ]
})
export class GateComponent implements OnInit{
  currentTime: Date = new Date();
  gateState: GateState = GateState.CLOSED;
  currentAccessRequest: AccessRequest | null = null;
  accessLogs: any[] = [];
  pendingRequests: AccessRequest[] = [];
  currentEmployee: any = null;
  
  // Constants for UI state
  GateState = GateState;
  AccessDirection = AccessDirection;
  
  private destroy$ = new Subject<void>();
  private loadingEmployee = false;

  constructor(
    private accessRequestService: AccessRequestService,
    private employeeService: EmployeeService,
  ) {}
  ngOnInit(): void {
    interval(1000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.currentTime = new Date());

    // Poling la fiecare 5 secunde pentru ultimul access request
    interval(5000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => this.loadLatestAccessRequest());

    // La inițializare și imediat după pornire
    this.loadLatestAccessRequest();

    this.loadTodayLogs();
  }

  loadLatestAccessRequest(): void {
    this.accessRequestService.getLatestAccessRequest()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (request) => {
          this.currentAccessRequest = request;

          if (request) {
            this.loadEmployeeDetails(request);
          } else {
            this.currentEmployee = null;
          }
        },
        error: (err) => {
          console.error('No latest access request or error:', err);
          this.currentAccessRequest = null;
          this.currentEmployee = null;
        }
      });
  }

  private loadEmployeeDetails(request: AccessRequest): void {
    this.loadingEmployee = true;
    
    this.employeeService.getEmployee(request.employeeId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (employee) => {
          request.employee = employee;
          this.currentEmployee = employee;
          this.loadingEmployee = false;
        },
        error: (error) => {
          console.error('Error loading employee details:', error);
          this.loadingEmployee = false;
          this.currentEmployee = null;
        }
      });
  }

  approveAccess(): void {
    if (!this.currentAccessRequest) return;
  
    this.gateState = GateState.OPENING;
  
    const directionValue = this.currentAccessRequest.direction === AccessDirection.ENTRY ? 0 : 1;
    const methodValue = this.currentAccessRequest.vehicleNumber ? 1 : 0;
  
    const logEntry = {
      employeeId: this.currentAccessRequest.employeeId,
      timestamp: new Date().toISOString(),
      direction: directionValue,
      method: methodValue,
      vehicleNumber: this.currentAccessRequest.vehicleNumber || null
    };
  
    this.employeeService.createAccessLog(logEntry)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.accessLogs.unshift(response);
  
          // 🔽 Trimite "true" către ESP
          this.accessRequestService.sendToEsp("true")
            .pipe(takeUntil(this.destroy$))
            .subscribe({
              next: () => console.log('Data sent to ESP'),
              error: (err) => console.error('Failed to send to ESP', err)
            });
  
          // ✅ Deschiderea porții
          setTimeout(() => {
            this.gateState = GateState.OPEN;
  
            setTimeout(() => {
              this.gateState = GateState.CLOSING;
  
              setTimeout(() => {
                this.gateState = GateState.CLOSED;
                this.removeCurrentRequest();
              }, 3000);
            }, 7000);
          }, 3000);
        },
        error: (error) => {
          console.error('Error logging access:', error);
          this.gateState = GateState.CLOSED;
          this.removeCurrentRequest();
        }
      });
  }
  

  denyAccess(): void {
    if (!this.currentAccessRequest) return;
    this.removeCurrentRequest();
  }

  private removeCurrentRequest(): void {
    if (!this.currentAccessRequest) return;

    this.accessRequestService.removeAccessRequest(this.currentAccessRequest.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          console.log('Access request removed successfully');
          this.currentAccessRequest = null;
          this.currentEmployee = null;
        },
        error: (error) => {
          console.error('Error removing access request:', error);
          this.currentAccessRequest = null;
          this.currentEmployee = null;
        }
      });
  }

  private loadTodayLogs(): void {
    const today = new Date();
    
    // Creează data fără a specifica timezone (va folosi ora locală)
    // Setează ora la 00:00:00 pentru începutul zilei
    const startOfDay = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 0, 0, 0);
    
    // Setează ora la 23:59:59 pentru sfârșitul zilei
    const endOfDay = new Date(today.getFullYear(), today.getMonth(), today.getDate(), 23, 59, 59, 999);
    
    console.log('Requesting logs between:', startOfDay.toISOString(), 'and', endOfDay.toISOString());
    
    // Modifică serviciul pentru a trimite data fără a converti la ISO (astfel backend-ul va face conversia corectă)
    this.employeeService.getAccessLogs(startOfDay, endOfDay)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (logs) => {
          console.log('Received logs count:', logs.length);
          this.accessLogs = logs;
        },
        error: (error) => {
          console.error('Error loading access logs:', error);
        }
      });
  }

  formatTime(date: Date): string {
    return formatDate(date, 'HH:mm:ss', 'en-US');
  }

  formatDateTime(value: string | Date | undefined | null): string {
    if (!value) {
      return 'N/A'; // sau returnează un string gol, sau alt fallback
    }
  
    try {
      const date = new Date(value);
      if (isNaN(date.getTime())) {
        return 'Invalid date';
      }
      return date.toLocaleString(); // sau folosește formatul dorit
    } catch (err) {
      return 'Invalid date';
    }
  }
  
}