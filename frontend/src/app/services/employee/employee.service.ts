import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  cnp: string;
  badgeNumber: string;
  photoUrl: string;
  divisionId: number;
  divisionName: string;
  bluetoothSecurityCode: string;
  vehicleNumber: string;
  isAccessEnabled: boolean;
  approvalDate: Date;
}

export interface CreateEmployeeDto {
  firstName: string;
  lastName: string;
  cnp: string;
  badgeNumber: string;
  photoUrl: string;
  divisionId: number;
  bluetoothSecurityCode: string;
  vehicleNumber: string;
}

export interface UpdateEmployeeDto {
  id: number;
  firstName: string;
  lastName: string;
  cnp: string;
  badgeNumber: string;
  photoUrl: string;
  divisionId: number;
  bluetoothSecurityCode: string;
  vehicleNumber: string;
  isAccessEnabled: boolean;
}

export interface AccessLog {
  id: number;
  employeeId: number;
  employeeName: string;
  timestamp: Date;
  direction: string;
  method: string;
  vehicleNumber: string;
}

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private apiUrl = `http://localhost:5000/api/employee`;
  private apiUrlAccessLog = `http://localhost:5000/api/AccessLog`;

  constructor(private http: HttpClient) { }

  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(this.apiUrl);
  }

  getEmployee(id: number): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/${id}`);
  }

  createEmployee(employee: CreateEmployeeDto): Observable<Employee> {
    return this.http.post<Employee>(this.apiUrl, employee);
  }

  updateEmployee(id: number, employee: UpdateEmployeeDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, employee);
  }

  deleteEmployee(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  toggleAccess(id: number, employee: UpdateEmployeeDto): Observable<void> {
    employee.isAccessEnabled = !employee.isAccessEnabled;
    return this.updateEmployee(id, employee);
  }

  getEmployeeAccessLogs(employeeId: number): Observable<AccessLog[]> {
    return this.http.get<AccessLog[]>(`${this.apiUrlAccessLog}/employee/${employeeId}`);
  }

  getAccessLogs(startDate: Date, endDate: Date): Observable<AccessLog[]> {
    const params = {
      startDate: startDate.toISOString(),
      endDate: endDate.toISOString()
    };
    
    return this.http.get<AccessLog[]>(`${this.apiUrlAccessLog}`, { params });
  }

  createAccessLog(logEntry: any): Observable<any> {
    // Convert timestamp to ISO string if it's a Date object
    if (logEntry.timestamp instanceof Date) {
      logEntry = {
        ...logEntry,
        timestamp: logEntry.timestamp.toISOString()
      };
    }
    return this.http.post<any>(`${this.apiUrlAccessLog}`, logEntry);
  }

  editEmployee(id: number, employee: UpdateEmployeeDto): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}`, employee);
  }
}