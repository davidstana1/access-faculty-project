import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { catchError, of } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ]
})
export class LoginComponent {
  loginForm: FormGroup;
  isLoading = false;
  errorMessage: string | null = null;
  passwordVisible = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });
    
    if (this.authService.isAuthenticated) {
      this.router.navigate(['/dashboard']);
    }
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }

  onSubmit() {
    if (this.loginForm.invalid) {
      Object.keys(this.loginForm.controls).forEach(key => {
        const control = this.loginForm.get(key);
        control?.markAsTouched();
      });
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.login(this.email?.value, this.password?.value)
      .pipe(
        catchError(error => {
          console.error('Login error:', error);
          
          if (error.status === 401) {
            this.errorMessage = 'Credențiale invalide. Verificați email-ul și parola.';
          } else if (error.status === 0) {
            this.errorMessage = 'Serverul nu este disponibil. Încercați mai târziu.';
          } else {
            this.errorMessage = 'A apărut o eroare la autentificare. Încercați din nou.';
          }
          
          this.isLoading = false;
          return of(null);
        })
      )
      .subscribe({
        next: (response) => {
          if (response) {
            this.isLoading = false;
            console.log('Login successful:', response);
          }
        }
      });
  }
}