import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div style="max-width: 800px; margin: 2rem auto; padding: 1.5rem; background: #ffffff; border-radius: 12px; box-shadow: 0 4px 12px rgba(0,0,0,0.08);">
      <h2 style="margin-bottom: 1.5rem; font-size: 1.75rem; font-weight: 700;">DJ Profile Dashboard</h2>
      
      <div *ngIf="savedMessage" style="background: #dcfce7; color: #15803d; padding: 1rem; border-radius: 8px; margin-bottom: 1.5rem;">
        {{ savedMessage }}
      </div>

      <form (ngSubmit)="saveProfile()">
        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Stage Name</label>
          <input type="text" [(ngModel)]="dj.stageName" name="stageName" required style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1;" />
        </div>

        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Tagline</label>
          <input type="text" [(ngModel)]="dj.tagline" name="tagline" placeholder="e.g. Open-Format & Baile Charme Specialist" style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1;" />
        </div>

        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Location</label>
          <input type="text" [(ngModel)]="dj.location" name="location" placeholder="City, State / Country" style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1;" />
        </div>

        <div style="margin-bottom: 1rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Profile Image URL</label>
          <input type="text" [(ngModel)]="dj.profileImageUrl" name="profileImageUrl" style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1;" />
        </div>

        <div style="margin-bottom: 1.5rem;">
          <label style="display: block; font-weight: 600; margin-bottom: 0.5rem;">Bio</label>
          <textarea [(ngModel)]="dj.bio" name="bio" rows="5" style="width: 100%; padding: 0.75rem; border-radius: 6px; border: 1px solid #cbd5e1;"></textarea>
        </div>

        <button type="submit" style="background: #0284c7; color: white; border: none; padding: 0.85rem 2rem; font-size: 1rem; border-radius: 8px; font-weight: 600; cursor: pointer;">
          Save Profile Updates
        </button>
      </form>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  private http = inject(HttpClient);
  private auth = inject(AuthService);

  dj: any = {};
  savedMessage = '';

  ngOnInit(): void {
    const token = this.auth.getToken();
    if (token) {
      const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
      this.http.get<any>('http://localhost:5275/api/DjProfiles/my-profile', { headers }).subscribe({
        next: (profile) => this.dj = profile || {},
        error: (err: any) => console.error('Error fetching profile:', err)
      });
    }
  }

  saveProfile(): void {
    const token = this.auth.getToken();
    if (!token) return;

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    // Clean payload matching UpdateDjProfileDto
    const payload = {
      stageName: this.dj.stageName || '',
      tagline: this.dj.tagline || '',
      location: this.dj.location || '',
      profileImageUrl: this.dj.profileImageUrl || '',
      bio: this.dj.bio || ''
    };

    this.http.put('http://localhost:5275/api/DjProfiles/my-profile', payload, { headers }).subscribe({
      next: () => {
        this.savedMessage = 'Profile updated successfully!';
        setTimeout(() => this.savedMessage = '', 4000);
      },
      error: (err: any) => {
        console.error('Error saving profile:', err.error);
      }
    });
  }
}