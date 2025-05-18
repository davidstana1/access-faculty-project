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
export class GateComponent implements OnInit, OnDestroy {
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
    // Update clock every second
    interval(1000)
      .pipe(takeUntil(this.destroy$))
      .subscribe(() => {
        this.currentTime = new Date();
      });

    // Subscribe to access requests
    this.accessRequestService.getAccessRequests()
      .pipe(takeUntil(this.destroy$))
      .subscribe(requests => {
        this.handleNewAccessRequests(requests);
      });
    
    // Load initial access logs for today
    this.loadTodayLogs();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private handleNewAccessRequests(requests: AccessRequest[]): void {
    if (!requests || requests.length === 0) return;
    
    // Add new requests to the pending queue
    for (const request of requests) {
      // Check if this request is already in our queue
      if (!this.pendingRequests.some(r => r.id === request.id)) {
        this.pendingRequests.push(request);
        
        // If we don't have employee details yet, fetch them
        if (!request.employee && !this.loadingEmployee) {
          this.loadEmployeeDetails(request);
        }
      }
    }
    
    // If we don't have a current request being processed, take the first one
    if (!this.currentAccessRequest && this.pendingRequests.length > 0) {
      this.processNextRequest();
    }
  }

  private loadEmployeeDetails(request: AccessRequest): void {
    this.loadingEmployee = true;
    
    this.employeeService.getEmployee(request.employeeId)
    .pipe(takeUntil(this.destroy$))
    .subscribe({
      next: (employee) => {
        request.employee = employee;
        this.loadingEmployee = false;
        
        // If this is the current request, update the current employee
        if (this.currentAccessRequest?.id === request.id) {
          this.currentEmployee = employee;
        }
      },
      error: (error) => {
        console.error('Error loading employee details:', error);
        this.loadingEmployee = false;
      }
    });
  }

  private processNextRequest(): void {
    if (this.pendingRequests.length === 0) {
      this.currentAccessRequest = null;
      this.currentEmployee = null;
      return;
    }
    
    // Take the first request from the queue
    this.currentAccessRequest = this.pendingRequests[0];
    
    // If we already have employee details, set the current employee
    if (this.currentAccessRequest.employee) {
      this.currentEmployee = this.currentAccessRequest.employee;
    } else if (!this.loadingEmployee) {
      // Otherwise load them
      this.loadEmployeeDetails(this.currentAccessRequest);
    }
  }

  approveAccess(): void {
    if (!this.currentAccessRequest) return;
    
    // Begin opening the gate
    this.gateState = GateState.OPENING;
    const directionValue = this.currentAccessRequest.direction === AccessDirection.ENTRY ? 0 : 1;
  
    // Determină metoda de acces bazată pe vehicleNumber
    // Dacă există vehicleNumber, atunci este Vehicle (1), altfel Direct (0)
    const methodValue = this.currentAccessRequest.vehicleNumber ? 1 : 0;
    
    // Create a log entry for this access event
    const logEntry = {
      employeeId: this.currentAccessRequest.employeeId,
      timestamp: new Date().toISOString(),
      direction: directionValue,  // 0 pentru Entry, 1 pentru Exit
      method: methodValue,        // 0 pentru Direct, 1 pentru Vehicle
      vehicleNumber: this.currentAccessRequest.vehicleNumber || null
    };
    
    // Record the access
    this.employeeService.createAccessLog(logEntry)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          // Add to our local log
          this.accessLogs.unshift(response);
          
          // Remove this request from pending queue
          this.pendingRequests.shift();
          
          // Simulate gate fully opening after 3 seconds
          setTimeout(() => {
            this.gateState = GateState.OPEN;
            
            // Simulate gate closing after person/car passes (7 seconds)
            setTimeout(() => {
              this.gateState = GateState.CLOSING;
              
              // Simulate gate fully closed after another 3 seconds
              setTimeout(() => {
                this.gateState = GateState.CLOSED;
                
                // Process the next request, if any
                this.processNextRequest();
              }, 3000);
            }, 7000);
          }, 3000);
        },
        error: (error) => {
          console.error('Error logging access:', error);
          this.gateState = GateState.CLOSED;
          
          // Still remove the request since we tried to process it
          this.pendingRequests.shift();
          this.processNextRequest();
        }
      });

    this.accessRequestService.removeAccessRequest(this.currentAccessRequest.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          console.log('Access request removed successfully');
        },
        error: (error) => {
          console.error('Error removing access request:', error);
        }
      });
  }

  denyAccess(): void {
    if (!this.currentAccessRequest) return;
    
    // No need to create a log entry for denied access
    // Just remove from queue and process next
    this.pendingRequests.shift();
    this.processNextRequest();
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