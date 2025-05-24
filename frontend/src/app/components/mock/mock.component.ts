import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { EmployeeService } from '../../services/employee/employee.service';

@Component({
  selector: 'app-mock-tests',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="mock-testing-panel">
      <h2>Mock Testing Panel</h2>
      
      <div class="card">
        <h3>Generate Access Request</h3>
        <div class="form-group">
          <label for="employeeId">Employee Name:</label>
          <select id="employeeId" [(ngModel)]="mockRequest.employeeId">
            <option *ngFor="let employee of testEmployees" [value]="employee.id">
              {{ employee.firstName }} {{ employee.lastName }}
            </option>
          </select>
        </div>
        
        <div class="form-group">
          <label for="direction">Direction:</label>
          <select id="direction" [(ngModel)]="mockRequest.direction">
            <option value="Entry">Entry</option>
            <option value="Exit">Exit</option>
          </select>
        </div>
        
        <div class="form-group">
          <label for="method">Method:</label>
          <select id="method" [(ngModel)]="mockRequest.method">
            <option value="Direct">Direct</option>
            <option value="Vehicle">Vehicle</option>
          </select>
        </div>
        
        <div class="form-group">
          <label for="vehicleNumber">Vehicle Number (optional):</label>
          <input type="text" id="vehicleNumber" [(ngModel)]="mockRequest.vehicleNumber">
        </div>
        
        <button (click)="sendMockRequest()">Send Access Request</button>
        
        <div *ngIf="lastResponse" class="response-box">
          <h4>Last Response:</h4>
          <pre>{{ lastResponse | json }}</pre>
        </div>
      </div>
      
      <div class="card">
        <h3>Gate Control</h3>
        <div class="gate-status">
          <strong>Current Status:</strong> {{ gateStatus?.state || 'Unknown' }}
          <div *ngIf="gateStatus" class="status-details">
            <div>Operational: {{ gateStatus.isOperational ? 'Yes' : 'No' }}</div>
            <div>Last Operation: {{ gateStatus.lastOperation }}</div>
            <div>Last Operation Time: {{ formatTime(gateStatus.lastOperationTime) }}</div>
          </div>
        </div>
        
        <div class="gate-controls">
          <button (click)="getGateStatus()">Refresh Status</button>
          <button (click)="openGate()" [disabled]="gateStatus?.state === 'open' || gateStatus?.state === 'opening'">Open Gate</button>
          <button (click)="closeGate()" [disabled]="gateStatus?.state === 'closed' || gateStatus?.state === 'closing'">Close Gate</button>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .mock-testing-panel {
      padding: 20px;
      max-width: 800px;
      margin: 0 auto;
    }
    
    .card {
      background: #f5f5f5;
      border-radius: 8px;
      padding: 20px;
      margin-bottom: 20px;
      box-shadow: 0 2px 4px rgba(0,0,0,0.1);
    }
    
    .form-group {
      margin-bottom: 15px;
    }
    
    label {
      display: block;
      margin-bottom: 5px;
      font-weight: bold;
    }
    
    input, select {
      width: 100%;
      padding: 8px;
      border: 1px solid #ddd;
      border-radius: 4px;
    }
    
    button {
      background: #4a90e2;
      color: white;
      border: none;
      padding: 10px 15px;
      border-radius: 4px;
      cursor: pointer;
      margin-right: 10px;
      margin-top: 10px;
    }
    
    button:disabled {
      background: #cccccc;
      cursor: not-allowed;
    }
    
    .response-box {
      margin-top: 20px;
      background: #e9f7ff;
      padding: 10px;
      border-radius: 4px;
      border-left: 4px solid #4a90e2;
    }
    
    pre {
      white-space: pre-wrap;
      word-wrap: break-word;
    }
    
    .gate-status {
      padding: 15px;
      background: #f0f0f0;
      border-radius: 4px;
      margin-bottom: 15px;
    }
    
    .status-details {
      margin-top: 10px;
      font-size: 0.9em;
      color: #555;
    }
    
    .gate-controls {
      display: flex;
      flex-wrap: wrap;
    }
  `]
})
export class MockTestsComponent implements OnInit {
  mockRequest = {
    employeeId: 1,
    direction: 'Entry',
    method: 'Vehicle',
    vehicleNumber: ''
  };
  
  testEmployees: any[] = [];
  lastResponse: any = null;
  gateStatus: any = null;
  
  constructor(
    private http: HttpClient,
    private employeeService: EmployeeService,
  ) {}
  
  ngOnInit(): void {
    this.getEmployees();
    this.getGateStatus();
  }
  
  getEmployees(): void {
    this.employeeService.getEmployees().subscribe({
      next: (data) => {
        this.testEmployees = data;
      },
      error: (error) => {
        console.error('Error loading employees', error);
      }
    });
  }

  sendMockRequest(): void {
    this.http.post('http://localhost:5203/api/accessRequests', this.mockRequest)
      .subscribe({
        next: (response) => {
          this.lastResponse = response;
          console.log('Mock request sent successfully', response);
        },
        error: (error) => {
          this.lastResponse = error;
          console.error('Error sending mock request', error);
        }
      });
  }
  
  getGateStatus(): void {
    this.http.get('http://localhost:5203/api/gate/status')
      .subscribe({
        next: (response: any) => {
          this.gateStatus = response;
        },
        error: (error) => {
          console.error('Error getting gate status', error);
        }
      });
  }
  
  openGate(): void {
    this.http.post('http://localhost:5203/api/gate/open', {})
      .subscribe({
        next: (response: any) => {
          this.lastResponse = response;
          this.getGateStatus();
        },
        error: (error) => {
          console.error('Error opening gate', error);
        }
      });
  }
  
  closeGate(): void {
    this.http.post('http://localhost:5203/api/gate/close', {})
      .subscribe({
        next: (response: any) => {
          this.lastResponse = response;
          this.getGateStatus();
        },
        error: (error) => {
          console.error('Error closing gate', error);
        }
      });
  }
  
  formatTime(timeStr: string): string {
    if (!timeStr) return '';
    const date = new Date(timeStr);
    return date.toLocaleString();
  }
}