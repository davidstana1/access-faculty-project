import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

export interface Division {
  id: string;
  name: string;
  description?: string;
}

@Injectable({
  providedIn: 'root'
})
export class DivisionService {
  // mock data
  private mockDivisions: Division[] = [
    { id: '1', name: 'IT Department' },
    { id: '12', name: 'Human Resources' },
    { id: '13', name: 'Finance' },
    { id: '4', name: 'Marketing' },
    { id: '5', name: 'Operations' },
    { id: '6', name: 'Sales' },
    { id: '7', name: 'Customer Support' }
  ];

  constructor() { }

  getDivisions(): Observable<Division[]> {
    return of(this.mockDivisions);
  }

  getDivision(id: string): Observable<Division | undefined> {
    const division = this.mockDivisions.find(d => d.id === id);
    return of(division);
  }
}