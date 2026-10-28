import { Injectable, inject, signal, PLATFORM_ID } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

export interface RegisterPayload {
  email: string;
  password: string;
  stageName?: string;
}

export interface LoginPayload {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: {
    id: string;
    email: string;
    stageName: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);
  private isBrowser = isPlatformBrowser(this.platformId);

  private apiUrl = 'http://localhost:5275/api/auth';

  private currentUserSubject = new BehaviorSubject<AuthResponse['user'] | null>(this.getStoredUser());
  public currentUser$ = this.currentUserSubject.asObservable();
  
  // Signal representation for modern Angular templates
  public currentUser = signal<AuthResponse['user'] | null>(this.getStoredUser());

  register(payload: RegisterPayload): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, payload).pipe(
      tap(res => this.handleAuthSuccess(res)),
      catchError(this.handleError)
    );
  }

  login(payload: LoginPayload): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, payload).pipe(
      tap(res => this.handleAuthSuccess(res)),
      catchError(this.handleError)
    );
  }

  logout(): void {
    if (this.isBrowser) {
      localStorage.removeItem('dj_dir_token');
      localStorage.removeItem('dj_dir_user');
    }
    this.currentUserSubject.next(null);
    this.currentUser.set(null);
  }

  getToken(): string | null {
    if (!this.isBrowser) return null;
    return localStorage.getItem('dj_dir_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  private handleAuthSuccess(response: AuthResponse): void {
    if (response.token) {
      if (this.isBrowser) {
        localStorage.setItem('dj_dir_token', response.token);
        localStorage.setItem('dj_dir_user', JSON.stringify(response.user));
      }
      this.currentUserSubject.next(response.user);
      this.currentUser.set(response.user);
    }
  }

  private getStoredUser(): AuthResponse['user'] | null {
    if (!this.isBrowser) return null;
    const userJson = localStorage.getItem('dj_dir_user');
    try {
      return userJson ? JSON.parse(userJson) : null;
    } catch {
      return null;
    }
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unexpected error occurred.';

    if (error.error) {
      // Handles ASP.NET Identity string array errors: { errors: ["Password too short"] }
      if (Array.isArray(error.error.errors) && error.error.errors.length > 0) {
        errorMessage = error.error.errors.join(' ');
      }
      // Handles ASP.NET Validation Dictionary errors: { errors: { Password: ["..."] } }
      else if (typeof error.error.errors === 'object' && error.error.errors !== null) {
        const errorValues = Object.values(error.error.errors).flat();
        if (errorValues.length > 0) {
          errorMessage = errorValues.join(' ');
        }
      }
      // Handles custom API responses: { message: "Invalid email or password." }
      else if (error.error.message) {
        errorMessage = error.error.message;
      }
    }

    return throwError(() => new Error(errorMessage));
  }
}