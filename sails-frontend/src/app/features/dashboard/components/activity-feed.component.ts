import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-activity-feed',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="bento-card activity-card">
      <div class="card-header">
        <h3 class="card-title">Recent Activity</h3>
        <button class="view-all">View All</button>
      </div>
      
      <div class="activity-list">
        <!-- Item 1 -->
        <div class="activity-item">
          <div class="activity-icon blue">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M13 2L3 14h9l-1 8 10-12h-9l1-8z"/>
            </svg>
          </div>
          <div class="activity-details">
            <p class="activity-title">Logged Energy</p>
            <p class="activity-meta">Supercharger • 45 kWh</p>
          </div>
          <span class="activity-time">2h ago</span>
        </div>

        <!-- Item 2 -->
        <div class="activity-item">
          <div class="activity-icon purple">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
              <circle cx="9" cy="7" r="4"/>
            </svg>
          </div>
          <div class="activity-details">
            <p class="activity-title">Joined Gang</p>
            <p class="activity-meta">Tesla Fanatics</p>
          </div>
          <span class="activity-time">1d ago</span>
        </div>
        
        <!-- Item 3 -->
         <div class="activity-item">
          <div class="activity-icon cyan">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
               <path d="M19 17h2c.6 0 1-.4 1-1v-3c0-.9-.7-1.7-1.5-1.9C18.7 10.6 16 10 16 10s-1.3-1.4-2.2-2.3c-.5-.4-1.1-.7-1.8-.7H5c-.6 0-1.1.4-1.4.9l-1.4 2.9A3.7 3.7 0 0 0 2 12v4c0 .6.4 1 1 1h2"/>
               <circle cx="7" cy="17" r="2"/>
               <path d="M9 17h6"/>
               <circle cx="17" cy="17" r="2"/>
            </svg>
          </div>
          <div class="activity-details">
            <p class="activity-title">Added Car</p>
            <p class="activity-meta">Tesla Model 3</p>
          </div>
          <span class="activity-time">2d ago</span>
        </div>
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
        /* Activity spans 6 in row 2 (if 12 col grid) */
        /* Currently small stats are 3 each. So 2 stats = 6 cols. 
           Wait, in dashboard:
           Row 1: Hero (8) + Actions (4) = 12
           Row 2: Stat (3) + Stat (3) + Activity (6) = 12
           So yes, Activity spans 6.
        */
        grid-column: span 6;
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
    
    .activity-card {
      min-height: 140px;
      padding: 20px;
    }
    
    .card-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 16px;
    }
    
    .card-header .card-title {
      margin: 0;
      font-size: 1rem;
      font-weight: 600;
      color: var(--text-primary);
    }
    
    .view-all {
      background: none;
      border: none;
      color: var(--aurora-blue);
      font-size: 0.85rem;
      font-weight: 600;
      cursor: pointer;
    }
    
    .activity-list {
      display: flex;
      flex-direction: column;
      gap: 12px;
    }
    
    .activity-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 8px;
      border-radius: 12px;
      transition: background 0.2s;
    }
    
    .activity-item:hover {
      background: rgba(255, 255, 255, 0.03);
    }
    
    .activity-icon {
      width: 36px;
      height: 36px;
      border-radius: 10px;
      background: rgba(255, 255, 255, 0.05);
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
    }
    
    .activity-icon.blue { color: var(--aurora-blue); background: rgba(59, 130, 246, 0.1); }
    .activity-icon.purple { color: var(--aurora-purple); background: rgba(139, 92, 246, 0.1); }
    .activity-icon.cyan { color: var(--aurora-cyan); background: rgba(6, 182, 212, 0.1); }
    
    .activity-icon svg {
      width: 18px;
      height: 18px;
    }
    
    .activity-details {
      flex: 1;
    }
    
    .activity-title {
      margin: 0;
      font-weight: 600;
      color: var(--text-primary);
      font-size: 0.9rem;
    }
    
    .activity-meta {
      margin: 0;
      font-size: 0.8rem;
      color: var(--text-secondary);
    }
    
    .activity-time {
      font-size: 0.8rem;
      color: var(--text-muted);
    }
  `]
})
export class ActivityFeedComponent {}
