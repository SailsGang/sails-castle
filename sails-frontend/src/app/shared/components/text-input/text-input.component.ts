import { Component, Input, forwardRef, signal, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

@Component({
  selector: 'app-text-input',
  standalone: true,
  imports: [CommonModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TextInputComponent),
      multi: true,
    },
  ],
  template: `
    <div class="input-wrapper">
      @if (label) {
        <label [for]="inputId" class="label">{{ label }}</label>
      }
      
      <div class="input-container" [class.focused]="isFocused()" [class.error]="hasError">
        <input
          #inputEl
          [id]="inputId"
          [type]="type"
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
        @if (value() && showClear) {
          <button
            type="button"
            class="clear-btn"
            (click)="clearValue()"
            aria-label="Clear input"
            tabindex="-1"
          >
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="12" cy="12" r="10"/>
              <line x1="15" y1="9" x2="9" y2="15"/>
              <line x1="9" y1="9" x2="15" y2="15"/>
            </svg>
          </button>
        }
      </div>
      
      @if (hasError && errorMessage) {
        <span [id]="inputId + '-error'" class="error-text" role="alert">{{ errorMessage }}</span>
      }
      
      @if (hint && !hasError) {
        <span class="hint-text">{{ hint }}</span>
      }
    </div>
  `,
  styles: [`
    .input-wrapper {
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
      box-shadow: 
        0 0 0 3px rgba(59, 130, 246, 0.2),
        0 0 20px rgba(59, 130, 246, 0.1);
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
    
    .clear-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 32px;
      height: 32px;
      margin-right: 8px;
      background: transparent;
      border: none;
      color: var(--text-muted);
      cursor: pointer;
      border-radius: 50%;
      transition: all 0.2s ease;
    }
    
    .clear-btn:hover {
      color: var(--text-primary);
      background: rgba(255, 255, 255, 0.1);
    }
    
    .clear-btn svg {
      width: 16px;
      height: 16px;
    }
    
    .error-text {
      color: var(--danger);
      font-size: 0.75rem;
      animation: fadeIn 0.2s ease-out;
    }
    
    .hint-text {
      color: var(--text-muted);
      font-size: 0.75rem;
    }
    
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(-4px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class TextInputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() autocomplete = '';
  @Input() errorMessage = '';
  @Input() hasError = false;
  @Input() hint = '';
  @Input() showClear = false;
  @Input() ariaLabel = '';
  @Input() inputId = `input-${Math.random().toString(36).slice(2, 9)}`;
  
  @Output() onEnter = new EventEmitter<void>();
  
  readonly value = signal('');
  readonly isFocused = signal(false);
  
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
  
  clearValue(): void {
    this.value.set('');
    this.onChange('');
  }
}
