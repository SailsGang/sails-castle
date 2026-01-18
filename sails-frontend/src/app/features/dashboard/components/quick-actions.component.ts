import { Component, output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-quick-actions',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="bento-card actions-card"
         (mousemove)="onCardMouseMove($event)" 
         (mouseleave)="onCardMouseLeave()"
         [style.transform]="cardTransform()">
      <div class="card-glow"></div>
      <h3 class="card-title">Quick Actions</h3>
      <div class="actions-grid">
        <button class="action-btn primary" (click)="onAction('log')">
          <div class="shine"></div>
          <div class="icon-box">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path class="lightning" d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/>
            </svg>
          </div>
          <span>Log Energy</span>
        </button>
        
        <button class="action-btn secondary" (click)="onAction('gang')">
          <div class="icon-box">
             <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
               <circle cx="12" cy="12" r="10"/>
               <line x1="12" y1="8" x2="12" y2="16"/>
               <line x1="8" y1="12" x2="16" y2="12"/>
             </svg>
          </div>
          <span>Create Gang</span>
        </button>
        
        <button class="action-btn secondary" (click)="onAction('car')">
          <div class="icon-box">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M19 17h2c.6 0 1-.4 1-1v-3c0-.9-.7-1.7-1.5-1.9C18.7 10.6 16 10 16 10s-1.3-1.4-2.2-2.3c-.5-.4-1.1-.7-1.8-.7H5c-.6 0-1.1.4-1.4.9l-1.4 2.9A3.7 3.7 0 0 0 2 12v4c0 .6.4 1 1 1h2"/>
              <circle cx="7" cy="17" r="2"/>
              <circle cx="17" cy="17" r="2"/>
            </svg>
          </div>
          <span>Add Car</span>
        </button>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      grid-column: span 12;
    }
    
    @media (min-width: 1025px) {
      :host {
        grid-column: span 4;
      }
    }
    
    /* ACTIONS CARD STYLES */
    .bento-card {
      background: rgba(255, 255, 255, 0.03);
      border: 1px solid rgba(255, 255, 255, 0.08);
      border-radius: 24px;
      padding: 24px;
      position: relative;
      overflow: hidden;
      backdrop-filter: blur(12px);
      -webkit-backdrop-filter: blur(12px);
      transition: transform 0.1s cubic-bezier(0.1, 0.9, 0.2, 1), box-shadow 0.3s ease; /* Faster transform for tilt */
      height: 100%;
    }

    .bento-card:hover {
      box-shadow: 0 12px 32px rgba(0, 0, 0, 0.2);
      border-color: rgba(255, 255, 255, 0.12);
    }
    
    .card-glow {
      position: absolute;
      top: -50px;
      right: -50px;
      width: 200px;
      height: 200px;
      background: radial-gradient(circle, rgba(139, 92, 246, 0.2) 0%, transparent 70%); /* Purple Tint for Actions */
      filter: blur(40px);
      opacity: 0.6;
      pointer-events: none;
    }
    
    .actions-card {
      display: flex;
      flex-direction: column;
    }
    
    .card-title {
      font-size: 1.1rem;
      font-weight: 700;
      color: var(--text-primary);
      margin: 0 0 20px;
      position: relative; /* Above glow */
    }

    .actions-grid {
      display: grid;
      grid-template-rows: 1fr 1fr;
      grid-template-columns: 1fr 1fr;
      gap: 12px;
      flex: 1;
      position: relative; /* Above glow */
    }

    .action-btn {
      border: none;
      border-radius: 16px;
      padding: 16px;
      display: flex;
      flex-direction: column;
      align-items: flex-start;
      justify-content: space-between;
      cursor: pointer;
      transition: all 0.2s;
      text-align: left;
    }

    .action-btn:hover {
      transform: scale(1.02);
      /* Inner border light effect */
      box-shadow: inset 0 0 0 1px rgba(255,255,255,0.15);
    }

    .action-btn.primary {
      grid-column: span 2; /* Full width */
      background: linear-gradient(135deg, var(--aurora-blue), var(--aurora-purple), var(--aurora-cyan));
      background-size: 200% 200%;
      animation: gradientFlow 4s ease infinite;
      color: white;
      flex-direction: row;
      align-items: center;
      justify-content: center;
      gap: 12px;
      box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
      position: relative;
      overflow: hidden;
    }
    
    @keyframes gradientFlow {
      0% { background-position: 0% 50%; }
      50% { background-position: 100% 50%; }
      100% { background-position: 0% 50%; }
    }
    
    .shine {
      position: absolute;
      top: 0;
      left: -100%;
      width: 50%;
      height: 100%;
      background: linear-gradient(90deg, transparent, rgba(255,255,255,0.2), transparent);
      transform: skewX(-20deg);
      pointer-events: none;
    }
    
    .action-btn.primary:hover .shine {
      animation: shineSweep 1.5s infinite;
    }
    
    @keyframes shineSweep {
      0% { left: -100%; }
      50% { left: 200%; }
      100% { left: 200%; }
    }
    
    .lightning {
      transition: filter 0.3s, transform 0.3s;
      transform-origin: center;
    }
    
    .action-btn.primary:hover .lightning {
      filter: drop-shadow(0 0 5px rgba(255,255,255,0.8));
      transform: scale(1.1);
    }
    
    .action-btn.primary .icon-box {
      background: rgba(255, 255, 255, 0.2);
    }
    
    .action-btn.primary span {
      font-size: 1.1rem;
      font-weight: 700;
    }

    .action-btn.secondary {
      background: rgba(255, 255, 255, 0.05);
      border: 1px solid rgba(255, 255, 255, 0.05);
      color: var(--text-primary);
    }

    .action-btn.secondary:hover {
      background: rgba(255, 255, 255, 0.08);
      border-color: rgba(255, 255, 255, 0.15);
    }
    
    .icon-box {
      width: 36px;
      height: 36px;
      border-radius: 10px;
      background: rgba(255, 255, 255, 0.05);
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 8px;
    }
    
    .action-btn.primary .icon-box {
      margin-bottom: 0;
    }
    
    .icon-box svg {
      width: 20px;
      height: 20px;
    }
    
    .action-btn span {
      font-weight: 600;
      font-size: 0.9rem;
    }
  `]
})
export class QuickActionsComponent {
  readonly action = output<string>();
  
  // 3D Tilt State for Card
  readonly cardTransform = signal('');

  onAction(type: string): void {
    this.action.emit(type);
  }
  
  // Card Tilt Logic
  onCardMouseMove(e: MouseEvent) {
    const card = e.currentTarget as HTMLElement;
    const rect = card.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    
    const centerX = rect.width / 2;
    const centerY = rect.height / 2;
    
    const rotateX = ((y - centerY) / centerY) * -3; 
    const rotateY = ((x - centerX) / centerX) * 3;
    
    this.cardTransform.set(
      `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) scale3d(1.02, 1.02, 1.02)`
    );
  }
  
  onCardMouseLeave() {
    this.cardTransform.set(
      'perspective(1000px) rotateX(0) rotateY(0) scale3d(1, 1, 1)'
    );
  }
}
