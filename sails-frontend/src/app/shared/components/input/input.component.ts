import { Component, Input, forwardRef, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-input',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true,
    },
  ],
  template: `
    <div class="input-wrapper">
      @if (label) {
        <label [for]="inputId" class="label">{{ label }}</label>
      }
      <input
        [id]="inputId"
        [type]="type"
        [placeholder]="placeholder"
        [autocomplete]="autocomplete"
        [value]="value()"
        [class.has-error]="showError()"
        (input)="onInput($event)"
        (blur)="onTouched()"
      />
      @if (showError() && errorMessage) {
        <span class="error-message">{{ errorMessage }}</span>
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
    
    input {
      width: 100%;
      background: rgba(0, 0, 0, 0.3);
      border: 1px solid var(--glass-border);
      border-radius: var(--radius-md);
      padding: 14px 16px;
      color: var(--text-primary);
      font-size: 1rem;
      transition: all 0.3s ease;
      outline: none;
      box-sizing: border-box;
    }
    
    input::placeholder {
      color: var(--text-muted);
    }
    
    input:focus {
      border-color: var(--aurora-blue);
      box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.2);
    }
    
    input.has-error {
      border-color: var(--danger);
      animation: shake 0.4s ease-in-out;
    }
    
    @keyframes shake {
      0%, 100% { transform: translateX(0); }
      20% { transform: translateX(-4px); }
      40% { transform: translateX(4px); }
      60% { transform: translateX(-4px); }
      80% { transform: translateX(4px); }
    }
    
    .error-message {
      color: var(--danger);
      font-size: 0.75rem;
      animation: fadeIn 0.2s ease-out;
    }
    
    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(-4px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class InputComponent implements ControlValueAccessor {
  @Input() label = '';
  @Input() type = 'text';
  @Input() placeholder = '';
  @Input() autocomplete = '';
  @Input() errorMessage = '';
  @Input() inputId = `input-${Math.random().toString(36).slice(2, 9)}`;
  
  // Signal-based state
  readonly value = signal('');
  private readonly _showError = signal(false);
  private readonly _touched = signal(false);
  
  readonly showError = computed(() => this._showError() && this._touched());
  
  // ControlValueAccessor
  private onChange: (value: string) => void = () => {};
  onTouched: () => void = () => {
    this._touched.set(true);
  };
  
  writeValue(value: string): void {
    this.value.set(value || '');
  }
  
  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }
  
  registerOnTouched(fn: () => void): void {
    const originalFn = fn;
    this.onTouched = () => {
      this._touched.set(true);
      originalFn();
    };
  }
  
  setDisabledState?(isDisabled: boolean): void {
    // Handle disabled state if needed
  }
  
  onInput(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.value.set(target.value);
    this.onChange(target.value);
  }
  
  // Called by parent to show errors after form submission
  markAsError(hasError: boolean): void {
    this._showError.set(hasError);
    this._touched.set(true);
  }
}
