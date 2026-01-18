import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TokenService } from './token.service';
import { 
  User, 
  AuthResponse, 
  LoginRequest, 
  RegisterRequest,
  RefreshTokenRequest 
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly tokenService = inject(TokenService);
  
  private readonly apiUrl = environment.apiUrl;
  
  // Signals for reactive state
  private readonly _user = signal<User | null>(null);
  private readonly _loading = signal(false);
  private readonly _error = signal<string | null>(null);
  private readonly _initialized = signal(false);
  
  // Public readonly signals
  readonly user = this._user.asReadonly();
  readonly loading = this._loading.asReadonly();
  readonly error = this._error.asReadonly();
  readonly isAuthenticated = computed(() => !!this._user());
  readonly initialized = this._initialized.asReadonly();
  
  constructor() {
    this.initializeAuth();
  }
  
  private async initializeAuth(): Promise<void> {
    const token = this.tokenService.getAccessToken();
    if (token && !this.tokenService.isTokenExpired(token)) {
      try {
        await this.fetchCurrentUser();
      } catch {
        this.tokenService.clearTokens();
      }
    }
    this._initialized.set(true);
  }
  
  async login(request: LoginRequest): Promise<boolean> {
    this._loading.set(true);
    this._error.set(null);
    
    try {
      const response = await firstValueFrom(
        this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, request)
      );
      
      this.handleAuthSuccess(response);
      await this.router.navigate(['/dashboard']);
      return true;
    } catch (error) {
      this.handleError(error);
      return false;
    } finally {
      this._loading.set(false);
    }
  }
  
  async register(request: RegisterRequest): Promise<boolean> {
    this._loading.set(true);
    this._error.set(null);
    
    try {
      const response = await firstValueFrom(
        this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, request)
      );
      
      this.handleAuthSuccess(response);
      await this.router.navigate(['/dashboard']);
      return true;
    } catch (error) {
      this.handleError(error);
      return false;
    } finally {
      this._loading.set(false);
    }
  }
  
  async logout(): Promise<void> {
    try {
      await firstValueFrom(
        this.http.post(`${this.apiUrl}/auth/logout`, {})
      );
    } catch {
      // Ignore logout errors
    } finally {
      this.tokenService.clearTokens();
      this._user.set(null);
      await this.router.navigate(['/auth/login']);
    }
  }
  
  async refreshToken(): Promise<boolean> {
    const accessToken = this.tokenService.getAccessToken();
    const refreshToken = this.tokenService.getRefreshToken();
    
    if (!accessToken || !refreshToken) {
      return false;
    }
    
    try {
      const request: RefreshTokenRequest = { accessToken, refreshToken };
      const response = await firstValueFrom(
        this.http.post<AuthResponse>(`${this.apiUrl}/auth/refresh`, request)
      );
      
      this.handleAuthSuccess(response);
      return true;
    } catch {
      this.tokenService.clearTokens();
      this._user.set(null);
      return false;
    }
  }
  
  async fetchCurrentUser(): Promise<void> {
    const response = await firstValueFrom(
      this.http.get<User>(`${this.apiUrl}/me`)
    );
    this._user.set(response);
  }
  
  private handleAuthSuccess(response: AuthResponse): void {
    this.tokenService.setTokens(response.accessToken, response.refreshToken);
    this._user.set({
      id: response.userId,
      email: response.email,
      displayName: response.displayName,
    });
  }
  
  private handleError(error: unknown): void {
    if (error instanceof HttpErrorResponse) {
      if (error.error?.detail) {
        this._error.set(error.error.detail);
      } else if (error.error?.title) {
        this._error.set(error.error.title);
      } else if (error.status === 422) {
        this._error.set('Invalid credentials. Please check your email and password.');
      } else if (error.status === 0) {
        this._error.set('Unable to connect to server. Please try again.');
      } else {
        this._error.set('An unexpected error occurred. Please try again.');
      }
    } else {
      this._error.set('An unexpected error occurred. Please try again.');
    }
  }
  
  clearError(): void {
    this._error.set(null);
  }
}
