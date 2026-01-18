import { Component, inject, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthService } from '../../../core/auth/auth.service';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { PasswordInputComponent } from '../../../shared/components/password-input/password-input.component';
import { LoadingOverlayComponent } from '../../../shared/components/loading-overlay/loading-overlay.component';
import { RateLimitAlertComponent } from '../../../shared/components/rate-limit-alert/rate-limit-alert.component';
import { SuccessRedirectComponent } from '../../../shared/components/success-redirect/success-redirect.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    ButtonComponent, 
    TextInputComponent, 
    PasswordInputComponent,
    LoadingOverlayComponent,
    RateLimitAlertComponent,
    SuccessRedirectComponent
  ],
  template: `
    <div class="login-page" [class.exiting]="isExiting()">
      <h2 class="title">Welcome back</h2>
      <p class="subtitle">Sign in to continue to SailsEnergy</p>
      
      <!-- Rate limit alert -->
      <app-rate-limit-alert
        [isLocked]="isRateLimited()"
        [lockDuration]="rateLimitDuration()"
        (unlocked)="onRateLimitUnlock()"
      />
      
      @if (authService.error() && !isRateLimited()) {
        <div class="error-alert" role="alert">
          <svg class="error-icon" viewBox="0 0 20 20" fill="currentColor" aria-hidden="true">
            <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clip-rule="evenodd"/>
          </svg>
          <span>{{ authService.error() }}</span>
        </div>
      }
      
      <form [formGroup]="form" (ngSubmit)="onSubmit()" class="form" novalidate>
        <div class="form-content" [class.loading]="authService.loading()">
          <app-text-input
            #emailInput
            label="Email"
            type="email"
            placeholder="you@example.com"
            autocomplete="email"
            formControlName="email"
            [hasError]="showError('email')"
            errorMessage="Please enter a valid email address"
            ariaLabel="Email address"
          />
          
          <app-password-input
            label="Password"
            placeholder="Enter your password"
            autocomplete="current-password"
            formControlName="password"
            [hasError]="showError('password')"
            errorMessage="Password must be at least 8 characters"
            ariaLabel="Password"
            (onEnter)="onSubmit()"
          />
          
          <app-button 
            type="submit" 
            [variant]="isSuccess() ? 'success' : 'primary'"
            [loading]="authService.loading()"
            [disabled]="authService.loading() || isRateLimited()"
          >
            @if (isSuccess()) {
              ✓ Success!
            } @else if (authService.loading()) {
              Signing in...
            } @else {
              Sign in
            }
          </app-button>
        </div>
        
        <!-- Loading overlay -->
        <app-loading-overlay 
          [isVisible]="authService.loading()" 
          message="Signing you in..."
        />
      </form>
      
      <p class="footer-text">
        Don't have an account? 
        <a 
          class="link"
          (click)="navigateTo('/auth/register')"
          (keydown.enter)="navigateTo('/auth/register')"
          tabindex="0"
          role="link"
        >Create one</a>
      </p>
    </div>
    
    <!-- Success redirect overlay -->
    <app-success-redirect
      [isVisible]="showSuccessRedirect()"
      title="Welcome back!"
      message="Redirecting to your dashboard..."
      [redirectDelay]="1500"
      (onComplete)="onRedirectComplete()"
    />
  `,
  styles: [`
    .login-page {
      text-align: center;
      animation: slideIn 0.4s cubic-bezier(0.4, 0, 0.2, 1);
    }
    
    .login-page.exiting {
      animation: slideOut 0.3s cubic-bezier(0.4, 0, 0.2, 1) forwards;
    }
    
    @keyframes slideIn {
      from { opacity: 0; transform: translateX(20px); }
      to { opacity: 1; transform: translateX(0); }
    }
    
    @keyframes slideOut {
      from { opacity: 1; transform: translateX(0); }
      to { opacity: 0; transform: translateX(-20px); }
    }
    
    .title {
      font-size: 1.5rem;
      font-weight: 600;
      margin: 0 0 8px;
      color: var(--text-primary);
    }
    
    .subtitle {
      color: var(--text-secondary);
      margin: 0 0 24px;
      font-size: 0.95rem;
    }
    
    .error-alert {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 12px 16px;
      background: rgba(239, 68, 68, 0.15);
      border: 1px solid rgba(239, 68, 68, 0.3);
      border-radius: 10px;
      color: #f87171;
      font-size: 0.875rem;
      margin-bottom: 20px;
      text-align: left;
      animation: shake 0.4s ease-in-out;
    }
    
    @keyframes shake {
      0%, 100% { transform: translateX(0); }
      20% { transform: translateX(-4px); }
      40% { transform: translateX(4px); }
      60% { transform: translateX(-4px); }
      80% { transform: translateX(4px); }
    }
    
    .error-icon {
      width: 20px;
      height: 20px;
      flex-shrink: 0;
    }
    
    .form {
      position: relative;
    }
    
    .form-content {
      display: flex;
      flex-direction: column;
      gap: 20px;
      text-align: left;
      transition: opacity 0.3s ease;
    }
    
    .form-content.loading {
      opacity: 0.5;
      pointer-events: none;
    }
    
    .footer-text {
      margin-top: 24px;
      color: var(--text-secondary);
      font-size: 0.9rem;
    }
    
    .link {
      color: var(--aurora-blue);
      text-decoration: none;
      font-weight: 500;
      position: relative;
      cursor: pointer;
      transition: color 0.2s ease;
    }
    
    .link::after {
      content: '';
      position: absolute;
      left: 0;
      bottom: -2px;
      width: 100%;
      height: 2px;
      background: linear-gradient(90deg, var(--aurora-blue), var(--aurora-purple));
      transform: scaleX(0);
      transform-origin: right;
      transition: transform 0.3s ease;
    }
    
    .link:hover, .link:focus {
      color: var(--aurora-purple);
      outline: none;
    }
    
    .link:hover::after, .link:focus::after {
      transform: scaleX(1);
      transform-origin: left;
    }
    
    /* Mobile responsive styles */
    @media (max-width: 480px) {
      .title {
        font-size: 1.25rem;
      }
      
      .subtitle {
        font-size: 0.875rem;
        margin-bottom: 20px;
      }
      
      .form-content {
        gap: 16px;
      }
      
      .error-alert {
        padding: 10px 12px;
        font-size: 0.8rem;
      }
    }
    
    /* Touch-friendly targets */
    @media (pointer: coarse) {
      .link {
        padding: 8px 0;
      }
    }
  `]
})
export class LoginComponent implements AfterViewInit {
  @ViewChild('emailInput', { read: ElementRef }) emailInputRef!: ElementRef;
  
  readonly authService = inject(AuthService);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  
  readonly submitted = signal(false);
  readonly isExiting = signal(false);
  readonly isSuccess = signal(false);
  readonly showSuccessRedirect = signal(false);
  readonly isRateLimited = signal(false);
  readonly rateLimitDuration = signal(30);
  
  private failedAttempts = 0;
  
  form: FormGroup = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });
  
  ngAfterViewInit(): void {
    setTimeout(() => {
      const input = this.emailInputRef?.nativeElement?.querySelector('input');
      input?.focus();
    }, 400);
  }
  
  showError(field: string): boolean {
    if (!this.submitted()) return false;
    const control = this.form.get(field);
    return control ? control.invalid : false;
  }
  
  async onSubmit(): Promise<void> {
    if (this.isRateLimited()) return;
    
    this.submitted.set(true);
    
    if (this.form.invalid) {
      return;
    }
    
    this.authService.clearError();
    const success = await this.authService.login(this.form.value);
    
    if (success) {
      this.isSuccess.set(true);
      this.showSuccessRedirect.set(true);
      this.failedAttempts = 0;
    } else {
      this.failedAttempts++;
      
      // Rate limit after 5 failed attempts
      if (this.failedAttempts >= 5) {
        this.isRateLimited.set(true);
        // Set duration: 30s for first lockout, then increase
        const multiplier = Math.min(this.failedAttempts - 4, 4);
        this.rateLimitDuration.set(30 * multiplier);
      }
    }
  }
  
  onRateLimitUnlock(): void {
    this.isRateLimited.set(false);
    this.authService.clearError();
    // Reset failed attempts after waiting
    this.failedAttempts = 0;
  }
  
  onRedirectComplete(): void {
    this.router.navigate(['/dashboard']);
  }
  
  navigateTo(route: string): void {
    this.isExiting.set(true);
    setTimeout(() => {
      this.router.navigate([route]);
    }, 280);
  }
}
