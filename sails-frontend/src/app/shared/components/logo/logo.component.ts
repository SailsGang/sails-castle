import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-logo',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="logo-container" [ngClass]="size">
      <span class="sails">Sails</span><span class="energy">Energy</span>
    </div>
  `,
  styles: [`
    .logo-container {
      display: inline-flex;
      align-items: baseline;
      font-weight: 700;
      font-size: 1.75rem; /* normal */
      letter-spacing: -0.02em;
      white-space: nowrap;
      line-height: 1.4;
    }
    
    .logo-container.large {
      font-size: 2.25rem;
    }

    .logo-container.medium {
      font-size: 1.5rem;
    }

    .logo-container.small {
      font-size: 1.25rem;
    }
    
    .sails {
      color: var(--text-primary);
    }
    
    .energy {
      display: inline;
      background: linear-gradient(
        90deg,
        #3b82f6 0%,
        #8b5cf6 25%,
        #06b6d4 50%,
        #10b981 75%,
        #3b82f6 100%
      );
      background-size: 200% 100%;
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
      animation: aurora-text 4s linear infinite;
    }
    
    @keyframes aurora-text {
      0% {
        background-position: 0% 50%;
      }
      100% {
        background-position: 200% 50%;
      }
    }
  `]
})
export class LogoComponent {
  @Input() size: 'small' | 'medium' | 'normal' | 'large' = 'normal';
}
