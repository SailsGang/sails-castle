import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-skeleton',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div 
      class="skeleton" 
      [class.circle]="variant === 'circle'"
      [class.text]="variant === 'text'"
      [class.button]="variant === 'button'"
      [style.width]="width"
      [style.height]="height"
      [style.border-radius]="borderRadius"
    ></div>
  `,
  styles: [`
    .skeleton {
      background: linear-gradient(
        90deg,
        rgba(255, 255, 255, 0.05) 0%,
        rgba(255, 255, 255, 0.1) 50%,
        rgba(255, 255, 255, 0.05) 100%
      );
      background-size: 200% 100%;
      animation: shimmer 1.5s ease-in-out infinite;
      border-radius: 8px;
    }
    
    @keyframes shimmer {
      0% { background-position: 200% 0; }
      100% { background-position: -200% 0; }
    }
    
    .skeleton.circle {
      border-radius: 50%;
    }
    
    .skeleton.text {
      height: 16px;
      border-radius: 4px;
    }
    
    .skeleton.button {
      height: 48px;
      border-radius: 12px;
    }
  `]
})
export class SkeletonComponent {
  @Input() width = '100%';
  @Input() height = '20px';
  @Input() borderRadius = '8px';
  @Input() variant: 'default' | 'circle' | 'text' | 'button' = 'default';
}
