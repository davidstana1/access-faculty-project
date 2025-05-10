import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

interface LoginResponse {
  token: string;
  refreshToken: string;
  expiration: Date;
  user: User;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = `http://localhost:5203/api/auth`; // API endpoint-ul pentru login
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    this.checkForSavedAuth();
  }

  // Actualizat pentru a se potrivi cu LoginComponent
  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, { email, password }).pipe(
      tap(response => {
        // Salvăm token-urile și user-ul în localStorage
        localStorage.setItem('access_token', response.token);
        localStorage.setItem('refresh_token', response.refreshToken);
        localStorage.setItem('token_expiration', response.expiration.toString());
        localStorage.setItem('user', JSON.stringify(response.user));

        // Actualizăm currentUserSubject
        this.currentUserSubject.next(response.user);

        // Navigăm către pagina principală după logare
        this.router.navigate(['/dashboard']);
      })
    );
  }

  logout() {
    // Ștergem datele de autentificare din localStorage
    localStorage.removeItem('access_token');
    localStorage.removeItem('refresh_token');
    localStorage.removeItem('token_expiration');
    localStorage.removeItem('user');
    this.currentUserSubject.next(null); // Resetăm utilizatorul curent
    this.router.navigate(['/login']); // Redirecționăm către login
  }

  checkForSavedAuth() {
    const token = localStorage.getItem('access_token');
    const refreshToken = localStorage.getItem('refresh_token');
    const userJson = localStorage.getItem('user');
    
    if (token && refreshToken && userJson) {
      const user = JSON.parse(userJson);
      this.currentUserSubject.next(user);

      // Verificăm dacă token-ul a expirat
      const expiration = localStorage.getItem('token_expiration');
      if (expiration) {
        const expiresAt = new Date(expiration);
        if (expiresAt > new Date()) {
          const timeLeft = expiresAt.getTime() - new Date().getTime();
          this.autoLogout(timeLeft); // Setăm un timer pentru logout automat
        } else {
          this.logout(); // Token expirat
        }
      }
    }
  }

  autoLogout(expirationDuration: number) {
    setTimeout(() => {
      this.logout(); // Logout automat
    }, expirationDuration);
  }

  // Alte metode pentru gestionarea autentificării
  get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  get isAuthenticated(): boolean {
    return !!this.currentUserValue && !!this.getToken();
  }

  getToken(): string | null {
    return localStorage.getItem('access_token');
  }
}