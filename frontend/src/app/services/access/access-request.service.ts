import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AccessRequestService {
  private apiUrl = `http://localhost:5203/api/accessRequests`; //needs to be done

  constructor(private http: HttpClient) { }

  getAccessRequests(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/pending`)
  }

  createAccessRequest(request: any): Observable<any> {
    return this.http.post<any>(this.apiUrl, request)
  }

  removeAccessRequest(id: string): Observable<any> {
    return this.http.delete<any>(`${this.apiUrl}/${id}`)
  }

}