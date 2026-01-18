import { Component, Input, forwardRef, signal, computed, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-password-input',
  standalone: true,
  imports: [CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => PasswordInputComponent),
      multi: true,
    },
  ],
  template: `
    <div class="password-wrapper">
      @if (label) {
        <label [for]="inputId" class="label">{{ label }}</label>
      }
      
      <div class="input-container" [class.focused]="isFocused()" [class.error]="hasError">
        <input
          [id]="inputId"
          [type]="showPassword() ? 'text' : 'password'"
          [placeholder]="placeholder"
          [autocomplete]="autocomplete"
          [value]="value()"
          [attr.aria-label]="ariaLabel || label"
          [attr.aria-invalid]="hasError"
          [attr.aria-describedby]="hasError ? inputId + '-error' : null"
          (input)="onInput($event)"
          (focus)="onFocus()"
          (blur)="onBlur()"
          (keydown.enter)="onEnter.emit()"
        />
        <button
          type="button"
          class="toggle-btn"
          (click)="togglePassword()"
          [attr.aria-label]="showPassword() ? 'Hide password' : 'Show password'"
          tabindex="-1"
        >
          @if (showPassword()) {
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
              <line x1="1" y1="1" x2="23" y2="23"/>
            </svg>
          } @else {
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
              <circle cx="12" cy="12" r="3"/>
            </svg>
          }
        </button>
      </div>
      
      @if (showStrength && value()) {
        <div class="strength-container">
          <div class="strength-bar">
            <div 
              class="strength-fill" 
              [class]="strengthClass()"
              [style.width]="strengthWidth()"
            ></div>
          </div>
          <span class="strength-text" [class]="strengthClass()">{{ strengthLabel() }}</span>
        </div>
      }
      
      @if (hasError && errorMessage) {
        <span [id]="inputId + '-error'" class="error-text" role="alert">{{ errorMessage }}</span>
      }
    </div>
  `,
  styles: [`
    .password-wrapper {
      display: flex;
      flex-direction: column;
      gap: 8px;
    }
    
    .label {
      font-size: 0.875rem;
      font-weight: 500;
      color: var(--text-secondary);
    }
    
    .input-container {
      position: relative;
      display: flex;
      align-items: center;
      background: rgba(0, 0, 0, 0.3);
      border: 1px solid var(--glass-border);
      border-radius: 12px;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }
    
    .input-container.focused {
      border-color: var(--aurora-blue);
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.2);
    }
    
    .input-container.error {
      border-color: var(--danger);
      animation: shake 0.4s ease-in-out;
    }
    
    @keyframes shake {
      0%, 100% { transform: translateX(0); }
      20% { transform: translateX(-2px); }
      40% { transform: translateX(2px); }
      60% { transform: translateX(-2px); }
      80% { transform: translateX(2px); }
    }
    
    input {
      flex: 1;
      background: transparent;
      border: none;
      padding: 14px 16px;
      color: var(--text-primary);
      font-size: 1rem;
      outline: none;
    }
    
    input::placeholder {
      color: var(--text-muted);
      transition: opacity 0.2s ease;
    }
    
    input:focus::placeholder {
      opacity: 0.5;
    }
    
    .toggle-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 44px;
      height: 44px;
      background: transparent;
      border: none;
      color: var(--text-muted);
      cursor: pointer;
      transition: color 0.2s ease;
    }
    
    .toggle-btn:hover {
      color: var(--text-primary);
    }
    
    .toggle-btn svg {
      width: 20px;
      height: 20px;
    }
    
    /* Strength indicator */
    .strength-container {
      display: flex;
      align-items: center;
      gap: 12px;
      animation: fadeIn 0.2s ease-out;
    }
    
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(-4px); }
      to { opacity: 1; transform: translateY(0); }
    }
    
    .strength-bar {
      flex: 1;
      height: 4px;
      background: rgba(255, 255, 255, 0.1);
      border-radius: 2px;
      overflow: hidden;
    }
    
    .strength-fill {
      height: 100%;
      border-radius: 2px;
      transition: all 0.3s ease;
    }
    
    .strength-fill.weak {
      background: #ef4444;
    }
    
    .strength-fill.fair {
      background: #f59e0b;
    }
    
    .strength-fill.good {
      background: #10b981;
    }
    
    .strength-fill.strong {
      background: linear-gradient(90deg, #10b981, #06b6d4);
    }
    
    .strength-text {
      font-size: 0.75rem;
      font-weight: 500;
      min-width: 50px;
    }
    
    .strength-text.weak { color: #ef4444; }
    .strength-text.fair { color: #f59e0b; }
    .strength-text.good { color: #10b981; }
    .strength-text.strong { color: #06b6d4; }
    
    .error-text {
      color: var(--danger);
      font-size: 0.75rem;
      animation: fadeIn 0.2s ease-out;
    }
  `]
})
export class PasswordInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() placeholder = '';
  @Input() autocomplete = 'current-password';
  @Input() errorMessage = '';
  @Input() hasError = false;
  @Input() showStrength = false;
  @Input() ariaLabel = '';
  @Input() inputId = `password-${Math.random().toString(36).slice(2, 9)}`;
  
  @Output() onEnter = new EventEmitter<void>();
  
  readonly value = signal('');
  readonly showPassword = signal(false);
  readonly isFocused = signal(false);
  
  // Password strength calculation
  readonly strength = computed(() => {
    const pwd = this.value();
    if (!pwd) return 0;
    
    let score = 0;
    
    // Length
    if (pwd.length >= 8) score += 1;
    if (pwd.length >= 12) score += 1;
    
    // Complexity
    if (/[a-z]/.test(pwd)) score += 1;
    if (/[A-Z]/.test(pwd)) score += 1;
    if (/[0-9]/.test(pwd)) score += 1;
    if (/[^a-zA-Z0-9]/.test(pwd)) score += 1;
    
    return Math.min(score, 4);
  });
  
  readonly strengthClass = computed(() => {
    const s = this.strength();
    if (s <= 1) return 'weak';
    if (s === 2) return 'fair';
    if (s === 3) return 'good';
    return 'strong';
  });
  
  readonly strengthLabel = computed(() => {
    const s = this.strength();
    if (s <= 1) return 'Weak';
    if (s === 2) return 'Fair';
    if (s === 3) return 'Good';
    return 'Strong';
  });
  
  readonly strengthWidth = computed(() => {
    return `${(this.strength() / 4) * 100}%`;
  });
  
  // ControlValueAccessor
  private onChange: (value: string) => void = () => {};
  private onTouched: () => void = () => {};
  
  writeValue(value: string): void {
    this.value.set(value || '');
  }
  
  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }
  
  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }
  
  onInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.value.set(target.value);
    this.onChange(target.value);
  }
  
  onFocus(): void {
    this.isFocused.set(true);
  }
  
  onBlur(): void {
    this.isFocused.set(false);
    this.onTouched();
  }
  
  togglePassword(): void {
    this.showPassword.update(v => !v);
  }
}
