import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-hero-stat-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="bento-card hero-card" 
         (mousemove)="onCardMouseMove($event)" 
         (mouseleave)="onCardMouseLeave()"
         [style.transform]="cardTransform()">
      <div class="card-glow"></div>
      <div class="card-content">
        <div class="stat-header">
          <span class="label">Total Spend</span>
          <div class="trend positive">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polyline points="23 6 13.5 15.5 8.5 10.5 1 18"/>
              <polyline points="17 6 23 6 23 12"/>
            </svg>
            <span>12%</span>
          </div>
        </div>
        <div class="stat-value">₴124.50</div>
        <p class="stat-sub">This month</p>
        
        <!-- Abstract Chart Visual (Trend 2026: Interaction & Life) -->
        <div class="chart-container" (mousemove)="onChartMouseMove($event)" (mouseleave)="onChartMouseLeave()">
          
          <!-- Glass Tooltip (Premium Holographic) -->
          <div class="glass-tooltip" 
               [style.opacity]="isChartHovered() ? 1 : 0"
               [style.transform]="'translate(' + tooltipX() + 'px, ' + tooltipY() + 'px)'">
            <div class="tooltip-shine"></div>
            <div class="tooltip-content">
              <span class="tooltip-label">Spend</span>
              <span class="tooltip-price">₴{{ tooltipPrice() }}</span>
              <span class="tooltip-date">Feb {{ tooltipDate() }}</span>
            </div>
          </div>

          <svg viewBox="0 0 100 40" preserveAspectRatio="none">
            <defs>
              <!-- Alive Gradient (Animated via CSS) -->
              <linearGradient id="lineGradient" x1="0%" y1="0%" x2="100%" y2="0%" class="living-gradient">
                <stop offset="0%" stop-color="#3b82f6" />
                <stop offset="50%" stop-color="#8b5cf6" />
                <stop offset="100%" stop-color="#ec4899" />
              </linearGradient>
              
              <linearGradient id="areaGradient" x1="0%" y1="0%" x2="0%" y2="100%">
                <stop offset="0%" stop-color="rgba(59, 130, 246, 0.3)" />
                <stop offset="100%" stop-color="rgba(59, 130, 246, 0.0)" />
              </linearGradient>
              
              <filter id="glow" x="-20%" y="-20%" width="140%" height="140%">
                <feGaussianBlur stdDeviation="2" result="coloredBlur"/>
                <feMerge>
                  <feMergeNode in="coloredBlur"/>
                  <feMergeNode in="SourceGraphic"/>
                </feMerge>
              </filter>
            </defs>
            
            <!-- IDLE LINE (Background - Previous Month) -->
            <path d="M0,35 Q25,32 50,15 T100,10" fill="none" stroke="rgba(255,255,255,0.15)" stroke-width="1.5" 
                  class="chart-line-bg" stroke-linecap="round"/>

            <!-- ACTIVE AREA (Fill) -->
            <path d="M0,30 Q25,20 50,10 T100,20 V40 H0 Z" fill="url(#areaGradient)" class="chart-area"/>
            
            <!-- ACTIVE LINE (Stroke with Glow) -->
            <path d="M0,30 Q25,20 50,10 T100,20" fill="none" stroke="url(#lineGradient)" stroke-width="2.5" 
                  class="chart-line" filter="url(#glow)" stroke-linecap="round"/>
                  
            <!-- SCANNING LINE (Cursor Aware) -->
            <line [attr.x1]="hoverX()" y1="0" [attr.x2]="hoverX()" y2="40" 
                  stroke="rgba(255, 255, 255, 0.3)" stroke-width="0.5" stroke-dasharray="2,2"
                  [style.opacity]="isChartHovered() ? 1 : 0" 
                  style="transition: opacity 0.2s" />
                  
            <!-- INTERSECTION DOT -->
             <circle [attr.cx]="hoverX()" [attr.cy]="hoverY()" r="1.5" fill="#fff"
                    [style.opacity]="isChartHovered() ? 1 : 0"
                    style="transition: opacity 0.2s" />
          </svg>
        </div>
      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      /* Spanning logic should be handled by parent grid, but default span for desktop */
      grid-column: span 12; 
    }
    
    @media (min-width: 1025px) {
      :host {
        grid-column: span 8;
      }
    }

    /* Card Base (Shared styles could be global but keeping scoped here for safety) */
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
      box-shadow: 0 12px 32px rgba(0, 0, 0, 0.2);
      border-color: rgba(255, 255, 255, 0.12);
    }

    /* Hero Card specific */
    .hero-card {
      background: linear-gradient(135deg, rgba(59, 130, 246, 0.1), rgba(139, 92, 246, 0.05));
      display: flex;
      flex-direction: column;
      justify-content: space-between;
      min-height: 220px;
      /* transform transition handled by inline style for tilt */
    }

    .card-glow {
      position: absolute;
      top: -50px;
      right: -50px;
      width: 200px;
      height: 200px;
      background: radial-gradient(circle, rgba(59, 130, 246, 0.3) 0%, transparent 70%);
      filter: blur(40px);
      opacity: 0.6;
    }

    .stat-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }

    .label {
      font-size: 0.9rem;
      font-weight: 600;
      color: var(--text-secondary);
      text-transform: uppercase;
      letter-spacing: 0.05em;
    }

    .trend {
      display: flex;
      align-items: center;
      gap: 4px;
      padding: 4px 10px;
      border-radius: 20px;
      font-size: 0.85rem;
      font-weight: 600;
    }

    .trend.positive {
      background: rgba(16, 185, 129, 0.1);
      color: #34d399;
    }

    .trend svg {
      width: 14px;
      height: 14px;
    }

    .stat-value {
      font-size: 3.5rem;
      font-weight: 800;
      color: var(--text-primary);
      line-height: 1;
      margin-bottom: 4px;
      letter-spacing: -0.03em;
    }

    .stat-sub {
      color: var(--text-muted);
      font-size: 0.9rem;
    }
    
    .chart-container {
      margin-top: auto;
      height: 100px;
      width: 100%;
      position: relative;
      cursor: crosshair;
    }
    
    .chart-container svg {
      width: 100%;
      height: 100%;
      overflow: visible;
    }

    /* Animation: Slowed to 8s as requested */
    .chart-line-bg {
      stroke-dasharray: 300;
      stroke-dashoffset: 300;
      animation: drawLine 6s cubic-bezier(0.22, 1, 0.36, 1) forwards;
      opacity: 0.5;
    }

    .chart-line {
      stroke-dasharray: 300;
      stroke-dashoffset: 300;
      animation: drawLine 8s cubic-bezier(0.22, 1, 0.36, 1) forwards, breatheColor 8s infinite alternate;
    }

    .chart-area {
      opacity: 0;
      animation: fadeInArea 3s ease-out 2s forwards;
    }
    
    .living-gradient {
      animation: shiftGradient 8s infinite alternate ease-in-out;
    }
    
    @keyframes drawLine {
      to { stroke-dashoffset: 0; }
    }
    
    @keyframes fadeInArea {
      to { opacity: 1; }
    }
    
    @keyframes breatheColor {
      0% { filter: hue-rotate(0deg) drop-shadow(0 0 2px rgba(59, 130, 246, 0.5)); }
      100% { filter: hue-rotate(30deg) drop-shadow(0 0 5px rgba(139, 92, 246, 0.5)); }
    }
    
    /* Holographic Tooltip */
    .glass-tooltip {
      position: absolute;
      top: 0;
      left: 0;
      background: rgba(18, 18, 40, 0.85); /* Darker backdrop */
      border: 1px solid rgba(255, 255, 255, 0.1);
      backdrop-filter: blur(16px);
      -webkit-backdrop-filter: blur(16px);
      border-radius: 16px;
      pointer-events: none;
      z-index: 20;
      box-shadow: 
        0 20px 40px rgba(0,0,0,0.4),
        0 0 0 1px rgba(255,255,255,0.1),
        inset 0 0 20px rgba(255,255,255,0.05); /* Inner glow */
      overflow: hidden;
      display: flex;
      flex-direction: column;
      gap: 2px;
      transition: opacity 0.3s ease, transform 0.15s cubic-bezier(0.1, 0.9, 0.2, 1);
      margin-top: -120px; /* Shifted up significantly */
      margin-left: -50px;
      min-width: 100px;
    }

    .tooltip-shine {
      position: absolute;
      top: 0;
      left: -100%;
      width: 50%;
      height: 100%;
      background: linear-gradient(90deg, transparent, rgba(255,255,255,0.1), transparent);
      transform: skewX(-20deg);
      animation: shine 3s infinite;
    }
    
    @keyframes shine {
      0% { left: -100%; }
      20% { left: 200%; }
      100% { left: 200%; }
    }

    .tooltip-content {
      position: relative;
      padding: 12px 16px;
      display: flex;
      flex-direction: column;
      gap: 2px;
      z-index: 2;
    }
    
    .tooltip-label {
      font-size: 0.7rem;
      text-transform: uppercase;
      letter-spacing: 0.05em;
      color: var(--text-muted);
    }
    
    .tooltip-price {
      font-weight: 700;
      color: #fff;
      font-size: 1.1rem;
      text-shadow: 0 0 10px rgba(255,255,255,0.3);
    }
    
    .tooltip-date {
      color: var(--text-secondary);
      font-size: 0.75rem;
      margin-top: 2px;
    }
  `]
})
export class HeroStatCardComponent {
  // Chart Interaction State
  readonly isChartHovered = signal(false);
  readonly hoverX = signal(0);
  readonly hoverY = signal(0);
  readonly tooltipX = signal(0);
  readonly tooltipY = signal(0);
  readonly tooltipPrice = signal('124.50');
  readonly tooltipDate = signal('18');

  // 3D Tilt State
  readonly cardTransform = signal('');
  
  onCardMouseMove(e: MouseEvent) {
    const card = e.currentTarget as HTMLElement;
    const rect = card.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    
    const centerX = rect.width / 2;
    const centerY = rect.height / 2;
    
    // Calculate rotation (max 5 degrees)
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

  onChartMouseMove(e: MouseEvent): void {
    this.isChartHovered.set(true);
    const rect = (e.currentTarget as HTMLElement).getBoundingClientRect();
    const x = e.clientX - rect.left;
    
    // Normalize X to SVG 0-100
    const svgX = (x / rect.width) * 100;
    
    // Simulate curve Y (Approximate M0,30 Q25,20 50,10 T100,20)
    let curveY = 30;
    if (svgX <= 50) {
       const t = svgX / 50;
       curveY = Math.pow(1-t, 2)*30 + 2*(1-t)*t*20 + Math.pow(t, 2)*10;
    } else {
       const t = (svgX - 50) / 50;
       curveY = Math.pow(1-t, 2)*10 + 2*(1-t)*t*0 + Math.pow(t, 2)*20;
    }
    
    const pixelY = (curveY / 40) * rect.height;
    
    this.hoverX.set(svgX); // SVG coord
    this.hoverY.set(curveY); // SVG coord
    
    this.tooltipX.set(x);
    this.tooltipY.set(pixelY);
    
    const priceBase = 150 - curveY * 2; 
    this.tooltipPrice.set(priceBase.toFixed(2));
    
    const day = Math.floor(1 + (svgX / 100) * 28);
    this.tooltipDate.set(day.toString());
  }

  onChartMouseLeave(): void {
    this.isChartHovered.set(false);
  }
}
