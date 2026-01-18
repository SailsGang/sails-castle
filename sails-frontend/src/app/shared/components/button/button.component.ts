import { Component, Input, Output, EventEmitter, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button
      #buttonEl
      [type]="type"
      [disabled]="disabled || loading"
      [class]="variant"
      [attr.aria-busy]="loading"
      [attr.aria-disabled]="disabled"
      (click)="handleClick($event)"
      (mousedown)="createRipple($event)"
    >
      <span class="button-content" [class.loading]="loading">
        @if (loading) {
          <svg class="spinner" viewBox="0 0 24 24" aria-hidden="true">
            <circle cx="12" cy="12" r="10" stroke="currentColor" stroke-width="3" fill="none" stroke-dasharray="32" stroke-dashoffset="8"/>
          </svg>
        }
        <span class="text"><ng-content></ng-content></span>
      </span>
      <span class="ripple-container" aria-hidden="true"></span>
    </button>
  `,
  styles: [`
    button {
      position: relative;
      border: none;
      border-radius: 12px;
      padding: 14px 28px;
      font-weight: 600;
      font-size: 1rem;
      cursor: pointer;
      transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
      overflow: hidden;
      width: 100%;
      isolation: isolate;
    }
    
    button:disabled {
      opacity: 0.7;
      cursor: not-allowed;
    }
    
    /* Primary variant */
    button.primary {
      background: linear-gradient(135deg, var(--aurora-blue), var(--aurora-purple));
      color: white;
    }
    
    button.primary::before {
      content: '';
      position: absolute;
      inset: 0;
      background: linear-gradient(135deg, var(--aurora-purple), var(--aurora-cyan));
      opacity: 0;
      transition: opacity 0.3s ease;
      z-index: 0;
    }
    
    button.primary:not(:disabled):hover::before {
      opacity: 1;
    }
    
    button.primary:not(:disabled):hover {
      transform: translateY(-2px);
      box-shadow: 
        0 8px 24px rgba(59, 130, 246, 0.4),
        0 0 40px rgba(139, 92, 246, 0.2);
    }
    
    button.primary:not(:disabled):active {
      transform: translateY(0) scale(0.98);
    }
    
    /* Secondary variant */
    button.secondary {
      background: var(--glass-bg);
      color: var(--text-primary);
      border: 1px solid var(--glass-border);
      backdrop-filter: blur(10px);
    }
    
    button.secondary:not(:disabled):hover {
      background: var(--glass-highlight);
      border-color: var(--glass-highlight);
    }
    
    .button-content {
      position: relative;
      z-index: 1;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      transition: opacity 0.2s ease;
    }
    
    .button-content.loading .text {
      opacity: 0.8;
    }
    
    .spinner {
      width: 20px;
      height: 20px;
      animation: spin 1s linear infinite;
    }
    
    @keyframes spin {
      from { transform: rotate(0deg); }
      to { transform: rotate(360deg); }
    }
    
    /* Ripple effect */
    .ripple-container {
      position: absolute;
      inset: 0;
      overflow: hidden;
      border-radius: inherit;
      z-index: 0;
    }
    
    :host ::ng-deep .ripple {
      position: absolute;
      border-radius: 50%;
      background: rgba(255, 255, 255, 0.3);
      transform: scale(0);
      animation: ripple 0.6s ease-out;
      pointer-events: none;
    }
    
    @keyframes ripple {
      to {
        transform: scale(4);
        opacity: 0;
      }
    }
    
    /* Success state */
    button.success {
      background: linear-gradient(135deg, var(--energy-green), #06b6d4);
    }
    
    button.success .button-content {
      animation: successPop 0.3s ease-out;
    }
    
    @keyframes successPop {
      0% { transform: scale(0.9); }
      50% { transform: scale(1.05); }
      100% { transform: scale(1); }
    }
  `]
})
export class ButtonComponent {
  @ViewChild('buttonEl') buttonEl!: ElementRef<HTMLButtonElement>;
  
  @Input() type: 'button' | 'submit' = 'button';
  @Input() variant: 'primary' | 'secondary' | 'success' = 'primary';
  @Input() disabled = false;
  @Input() loading = false;
  
  @Output() onClick = new EventEmitter<MouseEvent>();
  
  handleClick(event: MouseEvent): void {
    if (!this.disabled && !this.loading) {
      this.onClick.emit(event);
    }
  }
  
  createRipple(event: MouseEvent): void {
    if (this.disabled || this.loading) return;
    
    const button = this.buttonEl.nativeElement;
    const rippleContainer = button.querySelector('.ripple-container');
    if (!rippleContainer) return;
    
    const rect = button.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    const x = event.clientX - rect.left - size / 2;
    const y = event.clientY - rect.top - size / 2;
    
    const ripple = document.createElement('span');
    ripple.className = 'ripple';
    ripple.style.width = ripple.style.height = `${size}px`;
    ripple.style.left = `${x}px`;
    ripple.style.top = `${y}px`;
    
    rippleContainer.appendChild(ripple);
    
    ripple.addEventListener('animationend', () => {
      ripple.remove();
    });
  }
}
