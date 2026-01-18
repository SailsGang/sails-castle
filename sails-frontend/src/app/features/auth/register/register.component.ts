import { Component, inject, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService } from '../../../core/auth/auth.service';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { TextInputComponent } from '../../../shared/components/text-input/text-input.component';
import { PasswordInputComponent } from '../../../shared/components/password-input/password-input.component';
import { LoadingOverlayComponent } from '../../../shared/components/loading-overlay/loading-overlay.component';
import { RateLimitAlertComponent } from '../../../shared/components/rate-limit-alert/rate-limit-alert.component';
import { SuccessRedirectComponent } from '../../../shared/components/success-redirect/success-redirect.component';
import { PasswordGeneratorComponent } from '../../../shared/components/password-generator/password-generator.component';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    ButtonComponent, 
    TextInputComponent, 
    PasswordInputComponent,
    LoadingOverlayComponent,
    RateLimitAlertComponent,
    SuccessRedirectComponent,
    PasswordGeneratorComponent
  ],
  template: `
    <div class="register-page" [class.exiting]="isExiting()">
      <h2 class="title">Create account</h2>
      <p class="subtitle">Join SailsEnergy and start sharing</p>
      
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
            #usernameInput
            label="Username"
            placeholder="Choose a unique username"
            autocomplete="username"
            formControlName="displayName"
            [hasError]="showError('displayName')"
            errorMessage="Username is required (2-50 characters)"
            ariaLabel="Username"
          />
          
          <app-text-input
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
            placeholder="Create a strong password"
            autocomplete="new-password"
            formControlName="password"
            [hasError]="showError('password')"
            errorMessage="Password must be at least 8 characters"
            [showStrength]="true"
            ariaLabel="Password"
          />
          
          <!-- Password Generator (Progressive Disclosure) -->
          <app-password-generator
            (passwordGenerated)="useGeneratedPassword($event)"
          />
          
          <app-password-input
            label="Confirm Password"
            placeholder="Re-enter your password"
            autocomplete="new-password"
            formControlName="confirmPassword"
            [hasError]="showError('confirmPassword') || showPasswordMismatch()"
            [errorMessage]="showPasswordMismatch() ? 'Passwords do not match' : 'Please confirm your password'"
            ariaLabel="Confirm password"
            (onEnter)="onSubmit()"
          />
          
          <app-button 
            type="submit" 
            [variant]="isSuccess() ? 'success' : 'primary'"
            [loading]="authService.loading()"
            [disabled]="authService.loading() || isRateLimited()"
          >
            @if (isSuccess()) {
              ✓ Account created!
            } @else if (authService.loading()) {
              Creating account...
            } @else {
              Create account
            }
          </app-button>
        </div>
        
        <!-- Loading overlay -->
        <app-loading-overlay 
          [isVisible]="authService.loading()" 
          message="Creating your account..."
        />
      </form>
      
      <p class="footer-text">
        Already have an account? 
        <a 
          class="link"
          (click)="navigateTo('/auth/login')"
          (keydown.enter)="navigateTo('/auth/login')"
          tabindex="0"
          role="link"
        >Sign in</a>
      </p>
    </div>
    
    <!-- Success redirect overlay -->
    <app-success-redirect
      [isVisible]="showSuccessRedirect()"
      title="Welcome to SailsEnergy!"
      message="Your account has been created. Redirecting..."
      [redirectDelay]="2000"
      (onComplete)="onRedirectComplete()"
    />
  `,
  styles: [`
    .register-page {
      text-align: center;
      animation: slideIn 0.4s cubic-bezier(0.4, 0, 0.2, 1);
    }
    
    .register-page.exiting {
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
      gap: 16px;
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
        gap: 14px;
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
export class RegisterComponent implements AfterViewInit {
  @ViewChild('usernameInput', { read: ElementRef }) usernameInputRef!: ElementRef;
  
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
    displayName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]],
    confirmPassword: ['', [Validators.required]],
  }, { validators: this.passwordMatchValidator });
  
  ngAfterViewInit(): void {
    setTimeout(() => {
      const input = this.usernameInputRef?.nativeElement?.querySelector('input');
      input?.focus();
    }, 400);
  }
  
  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
    
    if (password && confirmPassword && password.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }
    return null;
  }
  
  showError(field: string): boolean {
    if (!this.submitted()) return false;
    const control = this.form.get(field);
    return control ? control.invalid : false;
  }
  
  showPasswordMismatch(): boolean {
    if (!this.submitted()) return false;
    const confirmPassword = this.form.get('confirmPassword');
    return this.form.hasError('passwordMismatch') && confirmPassword?.value !== '';
  }
  
  async onSubmit(): Promise<void> {
    if (this.isRateLimited()) return;
    
    this.submitted.set(true);
    
    if (this.form.invalid) {
      return;
    }
    
    this.authService.clearError();
    const success = await this.authService.register(this.form.value);
    
    if (success) {
      this.isSuccess.set(true);
      this.showSuccessRedirect.set(true);
      this.failedAttempts = 0;
    } else {
      this.failedAttempts++;
      
      // Rate limit after 5 failed attempts
      if (this.failedAttempts >= 5) {
        this.isRateLimited.set(true);
        this.rateLimitDuration.set(30 * Math.min(this.failedAttempts - 4, 4));
      }
    }
  }
  
  useGeneratedPassword(password: string): void {
    this.form.patchValue({
      password: password,
      confirmPassword: password
    });
  }
  
  onRateLimitUnlock(): void {
    this.isRateLimited.set(false);
    this.authService.clearError();
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
