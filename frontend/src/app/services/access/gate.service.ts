import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class GateService {
  private apiUrl = `http://localhost:5203/api/gate`;

  constructor(private http: HttpClient) { }

  /**
   * Get current gate status
   */
  getGateStatus(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/status`)
      .pipe(
        catchError(this.handleError<any>('getGateStatus'))
      );
  }

  /**
   * Update gate state manually
   */
  updateGateState(state: string): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/state`, { state })
      .pipe(
        catchError(this.handleError<any>('updateGateState'))
      );
  }

  /**
   * Command the gate to open
   */
  openGate(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/open`, {})
      .pipe(
        catchError(this.handleError<any>('openGate'))
      );
  }

  /**
   * Command the gate to close
   */
  closeGate(): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/close`, {})
      .pipe(
        catchError(this.handleError<any>('closeGate'))
      );
  }

  /**
   * Handle Http operation that failed.
   * Let the app continue.
   * @param operation - name of the operation that failed
   * @param result - optional value to return as the observable result
   */
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
      
      // Let the app keep running by returning an empty result.
      return of(result as T);
    };
  }
}