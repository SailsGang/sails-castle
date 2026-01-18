import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-success-redirect',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="success-overlay" [class.visible]="isVisible">
      <div class="success-content">
        <div class="checkmark-circle">
          <svg class="checkmark" viewBox="0 0 52 52">
            <circle class="checkmark-bg" cx="26" cy="26" r="25" fill="none"/>
            <path class="checkmark-check" fill="none" d="M14.1 27.2l7.1 7.2 16.7-16.8"/>
          </svg>
        </div>
        <h2 class="success-title">{{ title }}</h2>
        <p class="success-message">{{ message }}</p>
        <div class="redirect-bar">
          <div class="redirect-progress" [style.animation-duration]="redirectDelay + 'ms'"></div>
        </div>
        <p class="redirect-text">Redirecting in {{ countdown }}...</p>
      </div>
    </div>
  `,
  styles: [`
    .success-overlay {
      position: fixed;
      inset: 0;
      background: rgba(10, 10, 26, 0.95);
      backdrop-filter: blur(20px);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      opacity: 0;
      visibility: hidden;
      transition: all 0.3s ease;
    }
    
    .success-overlay.visible {
      opacity: 1;
      visibility: visible;
    }
    
    .success-content {
      text-align: center;
      animation: scaleIn 0.5s cubic-bezier(0.34, 1.56, 0.64, 1);
    }
    
    @keyframes scaleIn {
      from { transform: scale(0.8); opacity: 0; }
      to { transform: scale(1); opacity: 1; }
    }
    
    .checkmark-circle {
      width: 80px;
      height: 80px;
      margin: 0 auto 24px;
    }
    
    .checkmark {
      width: 100%;
      height: 100%;
      border-radius: 50%;
      stroke-width: 2;
      stroke: #10b981;
      stroke-miterlimit: 10;
    }
    
    .checkmark-bg {
      stroke: rgba(16, 185, 129, 0.2);
      animation: fillCircle 0.4s ease-in-out forwards;
    }
    
    @keyframes fillCircle {
      from { stroke-dasharray: 0 166; }
      to { stroke-dasharray: 166 166; }
    }
    
    .checkmark-check {
      stroke: #10b981;
      stroke-linecap: round;
      stroke-linejoin: round;
      stroke-dasharray: 48;
      stroke-dashoffset: 48;
      animation: drawCheck 0.4s 0.4s ease-in-out forwards;
    }
    
    @keyframes drawCheck {
      to { stroke-dashoffset: 0; }
    }
    
    .success-title {
      font-size: 1.75rem;
      font-weight: 600;
      color: var(--text-primary);
      margin: 0 0 8px;
    }
    
    .success-message {
      color: var(--text-secondary);
      margin: 0 0 32px;
    }
    
    .redirect-bar {
      width: 200px;
      height: 4px;
      background: rgba(255, 255, 255, 0.1);
      border-radius: 2px;
      margin: 0 auto 12px;
      overflow: hidden;
    }
    
    .redirect-progress {
      height: 100%;
      background: linear-gradient(90deg, var(--aurora-blue), var(--energy-green));
      border-radius: 2px;
      animation: progress linear forwards;
    }
    
    @keyframes progress {
      from { width: 100%; }
      to { width: 0%; }
    }
    
    .redirect-text {
      color: var(--text-muted);
      font-size: 0.875rem;
      margin: 0;
    }
  `]
})
export class SuccessRedirectComponent implements OnInit, OnDestroy {
  @Input() title = 'Success!';
  @Input() message = 'Operation completed successfully';
  @Input() redirectDelay = 2000;
  @Input() isVisible = false;
  
  @Output() onComplete = new EventEmitter<void>();
  
  countdown = 2;
  private intervalId: any;
  private timeoutId: any;
  
  ngOnInit(): void {
    if (this.isVisible) {
      this.startCountdown();
    }
  }
  
  ngOnChanges(): void {
    if (this.isVisible) {
      this.startCountdown();
    }
  }
  
  private startCountdown(): void {
    this.countdown = Math.ceil(this.redirectDelay / 1000);
    
    this.intervalId = setInterval(() => {
      this.countdown = Math.max(0, this.countdown - 1);
    }, 1000);
    
    this.timeoutId = setTimeout(() => {
      this.onComplete.emit();
    }, this.redirectDelay);
  }
  
  ngOnDestroy(): void {
    if (this.intervalId) clearInterval(this.intervalId);
    if (this.timeoutId) clearTimeout(this.timeoutId);
  }
}
