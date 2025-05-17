import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Employee, EmployeeService } from '../../../services/employee/employee.service';
import { AuthService, User } from '../../../services/auth.service';
import { DivisionService } from '../../../services/divisions/division.service';
import { RouterModule } from '@angular/router';

interface Division {
  id: string;
  name: string;
}

@Component({
  selector: 'app-employee-list',
  templateUrl: './employee-list.component.html',
  styleUrls: ['./employee-list.component.css'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule,RouterModule]
})
export class EmployeeListComponent implements OnInit {
  employees: Employee[] = [];
  divisions: Division[] = [];
  loading: boolean = true;
  currentUserValue: User | null = null;
  showAddEmployeeModal: boolean = false;
  showDeleteEmployeeModal: boolean = false;
  employeeToDelete: Employee | null = null;
  employeeForm: FormGroup;
  submitting: boolean = false;

  constructor(
    private employeeService: EmployeeService,
    private authService: AuthService,
    private divisionService: DivisionService,
    private fb: FormBuilder
  ) {
    this.employeeForm = this.fb.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      cnp: ['', [Validators.required, Validators.minLength(13), Validators.maxLength(13)]],
      badgeNumber: ['', [Validators.required]],
      divisionId: ['', [Validators.required]],
      bluetoothSecurityCode: ['', [Validators.required]],
      vehicleNumber: [''],
      photoUrl: ['']
    });
  }

  ngOnInit(): void {
    this.loadEmployees();
    this.loadDivisions();
    this.currentUserValue = this.authService.currentUserValue;
    this.authService.currentUser$.subscribe(user => {
      this.currentUserValue = user;
    });
  }

  loadEmployees(): void {
    this.loading = true;
    this.employeeService.getEmployees().subscribe({
      next: (data) => {
        this.employees = data;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading employees', error);
        this.loading = false;
      }
    });
  }

  loadDivisions(): void {
    this.divisionService.getDivisions().subscribe({
      next: (data: Division[]) => {
        this.divisions = data;
      }
    });
  }

  isHr(user: User | null): boolean {
    if (!user) return false;
    return this.authService.isHr(user);
  }

  isManager(user: User | null): boolean {
    if (!user) return false;
    return this.authService.isManager(user);
  }

  openAddEmployeeModal(): void {
    this.showAddEmployeeModal = true;
    this.employeeForm.reset();
  }

  closeAddEmployeeModal(event: Event): void {
    event.preventDefault();
    this.showAddEmployeeModal = false;
  }

  openDeleteEmployeeModal(employee: Employee): void {
    this.showDeleteEmployeeModal = true;
    this.employeeToDelete = employee;
  }

  closeDeleteEmployeeModal(event: Event): void {
    event.preventDefault();
    this.showDeleteEmployeeModal = false;
  }

  confirmDeleteEmployee(): void {
    if (!this.employeeToDelete) return;
  
    this.employeeService.deleteEmployee(this.employeeToDelete.id).subscribe({
      next: () => {
        this.showDeleteEmployeeModal = false;
        this.employeeToDelete = null;
        this.loadEmployees();
      },
      error: (error) => {
        console.error('Error deleting employee', error);
      }
    });
  }

  onSubmit(): void {
    if (this.employeeForm.invalid) {
      Object.keys(this.employeeForm.controls).forEach(key => {
        const control = this.employeeForm.get(key);
        control?.markAsTouched();
      });
      return;
    }

    this.submitting = true;
    
    const employeeData = this.employeeForm.value;
    
    this.employeeService.createEmployee(employeeData).subscribe({
      next: (newEmployee) => {
        this.submitting = false;
        this.showAddEmployeeModal = false;
        this.loadEmployees();
      },
      error: (error) => {
        console.error('Error creating employee', error);
        this.submitting = false;
      }
    });
  }
}