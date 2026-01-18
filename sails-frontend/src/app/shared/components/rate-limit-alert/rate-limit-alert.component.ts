import { Component, Input, OnDestroy, Output, EventEmitter, OnChanges, SimpleChanges, ChangeDetectorRef, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-rate-limit-alert',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (isLocked) {
      <div class="rate-limit-alert" role="alert">
        <div class="progress-ring">
          <svg viewBox="0 0 44 44">
            <circle 
              cx="22" cy="22" r="20" 
              fill="none" 
              stroke="rgba(239, 68, 68, 0.15)" 
              stroke-width="4"
            />
            <circle 
              class="progress-circle"
              cx="22" cy="22" r="20" 
              fill="none" 
              stroke="#ef4444" 
              stroke-width="4"
              stroke-linecap="round"
              [style.stroke-dasharray]="circumference"
              [style.stroke-dashoffset]="dashOffset"
            />
          </svg>
          <span class="time-display">{{ remainingTime }}</span>
        </div>
        <div class="content">
          <span class="title">Too many attempts</span>
          <span class="countdown">Please wait {{ formattedTime }} before trying again</span>
        </div>
      </div>
    }
  `,
  styles: [`
    .rate-limit-alert {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 16px 20px;
      background: rgba(239, 68, 68, 0.08);
      border: 1px solid rgba(239, 68, 68, 0.25);
      border-radius: 12px;
      margin-bottom: 20px;
      animation: slideIn 0.3s ease-out;
    }
    
    @keyframes slideIn {
      from { opacity: 0; transform: translateY(-10px); }
      to { opacity: 1; transform: translateY(0); }
    }
    
    .progress-ring {
      position: relative;
      width: 52px;
      height: 52px;
      flex-shrink: 0;
    }
    
    .progress-ring svg {
      width: 100%;
      height: 100%;
      transform: rotate(-90deg);
    }
    
    .progress-circle {
      transition: stroke-dashoffset 1s linear;
      filter: drop-shadow(0 0 6px rgba(239, 68, 68, 0.5));
    }
    
    .time-display {
      position: absolute;
      top: 50%;
      left: 50%;
      transform: translate(-50%, -50%);
      font-size: 1.1rem;
      font-weight: 700;
      color: #ef4444;
      font-variant-numeric: tabular-nums;
    }
    
    .content {
      flex: 1;
      display: flex;
      flex-direction: column;
      gap: 4px;
    }
    
    .title {
      font-weight: 600;
      color: #f87171;
      font-size: 0.95rem;
    }
    
    .countdown {
      color: var(--text-secondary);
      font-size: 0.85rem;
    }
  `]
})
export class RateLimitAlertComponent implements OnChanges, OnDestroy {
  private readonly cdr = inject(ChangeDetectorRef);
  
  @Input() lockDuration = 30;
  @Input() isLocked = false;
  
  @Output() unlocked = new EventEmitter<void>();
  
  remainingTime = 0;
  circumference = 2 * Math.PI * 20;
  dashOffset = 0;
  
  private intervalId: ReturnType<typeof setInterval> | null = null;
  
  get formattedTime(): string {
    const minutes = Math.floor(this.remainingTime / 60);
    const seconds = this.remainingTime % 60;
    if (minutes > 0) {
      return `${minutes}:${seconds.toString().padStart(2, '0')}`;
    }
    return `${seconds} seconds`;
  }
  
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['isLocked']) {
      if (this.isLocked) {
        this.startCountdown();
      } else {
        this.clearCountdown();
      }
    }
  }
  
  private startCountdown(): void {
    this.clearCountdown();
    this.remainingTime = this.lockDuration;
    this.updateDashOffset();
    
    this.intervalId = setInterval(() => {
      this.remainingTime--;
      this.updateDashOffset();
      this.cdr.detectChanges(); // Force Angular to update the view
      
      if (this.remainingTime <= 0) {
        this.clearCountdown();
        this.unlocked.emit();
      }
    }, 1000);
  }
  
  private updateDashOffset(): void {
    const progress = this.remainingTime / this.lockDuration;
    this.dashOffset = this.circumference * (1 - progress);
  }
  
  private clearCountdown(): void {
    if (this.intervalId !== null) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }
  
  ngOnDestroy(): void {
    this.clearCountdown();
  }
}
