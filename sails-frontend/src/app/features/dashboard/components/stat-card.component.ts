import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="bento-card stat-small">
      <div class="stat-icon" [ngClass]="color()">
        <!-- Projection for the icon -->
        <ng-content></ng-content>
      </div>
      <div class="stat-value-small">{{ value() }} <span class="unit" *ngIf="unit()">{{ unit() }}</span></div>
      <div class="stat-label">{{ label() }}</div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      grid-column: span 12;
    }
    
    @media (min-width: 1025px) {
      :host {
        grid-column: span 3;
      }
    }

    .bento-card {
      background: rgba(255, 255, 255, 0.03);
      border: 1px solid rgba(255, 255, 255, 0.08);
      border-radius: 24px;
      padding: 24px;
      position: relative;
      overflow: hidden;
      backdrop-filter: blur(12px);
      -webkit-backdrop-filter: blur(12px);
      transition: transform 0.3s ease, box-shadow 0.3s ease;
      height: 100%;
    }

    .bento-card:hover {
      transform: translateY(-2px);
      box-shadow: 0 12px 32px rgba(0, 0, 0, 0.2);
      border-color: rgba(255, 255, 255, 0.12);
    }
    
    .stat-small {
      display: flex;
      flex-direction: column;
      justify-content: center;
      min-height: 140px;
    }
    
    .stat-small .stat-icon {
      width: 40px;
      height: 40px;
      border-radius: 12px;
      background: rgba(16, 185, 129, 0.1);
      color: #34d399; /* Default Green */
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 12px;
    }
    
    /* Color Variants */
    .stat-icon.purple {
      background: rgba(139, 92, 246, 0.1);
      color: #a78bfa;
    }
    
    .stat-icon.blue {
      background: rgba(59, 130, 246, 0.1);
      color: #60a5fa;
    }
    
    .stat-icon.cyan {
      background: rgba(6, 182, 212, 0.1);
      color: #22d3ee;
    }
    
    ::ng-deep .stat-icon svg {
      width: 20px;
      height: 20px;
    }
    
    .stat-value-small {
      font-size: 1.8rem;
      font-weight: 700;
      color: var(--text-primary);
      margin-bottom: 4px;
    }
    
    .unit {
      font-size: 1rem;
      color: var(--text-secondary);
      font-weight: 500;
      margin-left: 2px;
    }
    
    .stat-label {
      color: var(--text-muted);
      font-size: 0.9rem;
    }
    
    /* Responsive adjustment for small screens inherited from dashboard */
    @media (max-width: 1024px) {
      .stat-small {
        min-height: auto;
        flex-direction: row;
        align-items: center;
        justify-content: space-between;
      }
      
      .stat-small .stat-icon {
        margin-bottom: 0;
        margin-right: 16px;
      }
      
      .stat-value-small {
        margin-bottom: 0;
      }
    }
  `]
})
export class StatCardComponent {
  readonly value = input.required<string | number>();
  readonly label = input.required<string>();
  readonly unit = input<string>();
  readonly color = input<string>('green'); // 'green', 'purple', 'blue', 'cyan'
}
