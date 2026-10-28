import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService, RegisterPayload } from '../../services/auth.service';

interface LocalRegisterPayload extends RegisterPayload {
  location?: string;
}

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div style="max-width: 480px; margin: 3rem auto; padding: 2rem; background: #ffffff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.1);">
      <h2 style="margin-bottom: 1.5rem; text-align: center;">Register DJ Profile</h2>
      
      @if (errorMessage) {
        <div style="background: #fef2f2; color: #dc2626; padding: 0.75rem; border-radius: 6px; margin-bottom: 1rem; font-size: 0.9rem;">
          {{ errorMessage }}
        </div>
      }

      <form (ngSubmit)="onSubmit()">
        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Stage Name</label>
          <input 
            type="text" 
            [(ngModel)]="formData.stageName" 
            name="stageName" 
            placeholder="e.g. DJ Hendo"
            required 
            style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1; box-sizing: border-box;" 
          />
        </div>

        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Location</label>
          <input 
            type="text" 
            [(ngModel)]="formData.location" 
            name="location" 
            placeholder="e.g. Salvador, BA" 
            required 
            style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1; box-sizing: border-box;" 
          />
        </div>

        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Email</label>
          <input 
            type="email" 
            [(ngModel)]="formData.email" 
            name="email" 
            required 
            style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1; box-sizing: border-box;" 
          />
        </div>

        <div style="margin-bottom: 1.5rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Password</label>
          <input 
            type="password" 
            [(ngModel)]="formData.password" 
            name="password" 
            required 
            style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1; box-sizing: border-box;" 
          />
        </div>

        <button 
          type="submit" 
          [disabled]="isLoading"
          style="width: 100%; background: #0284c7; color: white; padding: 0.75rem; border: none; border-radius: 6px; font-weight: 600; cursor: pointer; opacity: isLoading ? 0.7 : 1;">
          {{ isLoading ? 'Creating DJ Account...' : 'Create DJ Account' }}
        </button>
      </form>

      <p style="margin-top: 1.5rem; text-align: center; font-size: 0.9rem; color: #64748b;">
        Already registered? <a routerLink="/login" style="color: #0284c7; text-decoration: none; font-weight: 600;">Log in here</a>
      </p>
    </div>
  `
})
export class RegisterComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  formData: LocalRegisterPayload = { email: '', password: '', stageName: '', location: '' };
  errorMessage = '';
  isLoading = false;

  onSubmit(): void {
    if (!this.formData.email || !this.formData.password) return;

    this.isLoading = true;
    this.errorMessage = '';

    this.auth.register(this.formData).subscribe({
      next: () => {
        this.isLoading = false;
        this.router.navigate(['/dashboard']);
      },
      error: (err: any) => {
        this.isLoading = false;
        // Extracts message thrown by AuthService handleError or raw HttpErrorResponse
        this.errorMessage = err.message || err.error?.message || 'Registration failed.';
      }
    });
  }
}