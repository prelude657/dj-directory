import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div style="max-width: 400px; margin: 3rem auto; padding: 2rem; background: #ffffff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);">
      <h2 style="margin-bottom: 1.5rem; text-align: center;">DJ Portal Login</h2>
      
      <div *ngIf="errorMessage" style="background: #fef2f2; color: #dc2626; padding: 0.75rem; border-radius: 6px; margin-bottom: 1rem; font-size: 0.9rem;">
        {{ errorMessage }}
      </div>

      <form (ngSubmit)="onSubmit()">
        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Email</label>
          <input type="email" [(ngModel)]="credentials.email" name="email" required style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1;" />
        </div>

        <div style="margin-bottom: 1.5rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Password</label>
          <input type="password" [(ngModel)]="credentials.password" name="password" required style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1;" />
        </div>

        <button type="submit" style="width: 100%; background: #0284c7; color: white; padding: 0.75rem; border: none; border-radius: 6px; font-weight: 600; cursor: pointer;">
          Log In
        </button>
      </form>

      <p style="text-align: center; margin-top: 1.5rem; color: #64748b; font-size: 0.9rem;">
        Don't have an account? <a routerLink="/register" style="color: #0284c7; text-decoration: none; font-weight: 600;">Join as DJ</a>
      </p>
    </div>
  `
})
export class LoginComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  credentials = { email: '', password: '' };
  errorMessage = '';

  onSubmit(): void {
    this.auth.login(this.credentials).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => this.errorMessage = err.error?.message || 'Invalid login credentials.'
    });
  }
}