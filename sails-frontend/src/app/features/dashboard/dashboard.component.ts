import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <header class="header">
        <div>
          <h1 class="heading-2">Welcome back, {{ authService.user()?.displayName }}!</h1>
          <p class="subtitle">Here's what's happening with your energy today.</p>
        </div>
      </header>
      
      <div class="stats-grid">
        <div class="stat-card glass-card">
          <div class="stat-icon energy">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polygon points="13,2 3,14 12,14 11,22 21,10 12,10"/>
            </svg>
          </div>
          <div class="stat-content">
            <span class="stat-value mono">0 <small>kWh</small></span>
            <span class="stat-label">Total Energy This Month</span>
          </div>
        </div>
        
        <div class="stat-card glass-card">
          <div class="stat-icon gangs">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
              <circle cx="9" cy="7" r="4"/>
              <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
              <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
            </svg>
          </div>
          <div class="stat-content">
            <span class="stat-value mono">0</span>
            <span class="stat-label">Active Gangs</span>
          </div>
        </div>
        
        <div class="stat-card glass-card">
          <div class="stat-icon cars">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M5 11l1.5-4.5a2 2 0 0 1 1.9-1.5h7.2a2 2 0 0 1 1.9 1.5L19 11"/>
              <path d="M5 11h14v7a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2v-7z"/>
              <circle cx="7.5" cy="15.5" r="1.5"/>
              <circle cx="16.5" cy="15.5" r="1.5"/>
            </svg>
          </div>
          <div class="stat-content">
            <span class="stat-value mono">0</span>
            <span class="stat-label">Registered Cars</span>
          </div>
        </div>
        
        <div class="stat-card glass-card">
          <div class="stat-icon cost">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="12" y1="1" x2="12" y2="23"/>
              <path d="M17 5H9.5a3.5 3.5 0 0 0 0 7h5a3.5 3.5 0 0 1 0 7H6"/>
            </svg>
          </div>
          <div class="stat-content">
            <span class="stat-value mono">€0.00</span>
            <span class="stat-label">Estimated Cost</span>
          </div>
        </div>
      </div>
      
      <section class="section">
        <h2 class="section-title">Quick Actions</h2>
        <div class="actions-grid">
          <a href="/energy/log" class="action-card glass-card">
            <div class="action-icon energy">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <polygon points="13,2 3,14 12,14 11,22 21,10 12,10"/>
              </svg>
            </div>
            <span class="action-title">Log Energy</span>
            <span class="action-desc">Record a charging session</span>
          </a>
          
          <a href="/gangs/new" class="action-card glass-card">
            <div class="action-icon gangs">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <line x1="12" y1="8" x2="12" y2="16"/>
                <line x1="8" y1="12" x2="16" y2="12"/>
              </svg>
            </div>
            <span class="action-title">Create Gang</span>
            <span class="action-desc">Start a new group</span>
          </a>
          
          <a href="/cars" class="action-card glass-card">
            <div class="action-icon cars">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="10"/>
                <line x1="12" y1="8" x2="12" y2="16"/>
                <line x1="8" y1="12" x2="16" y2="12"/>
              </svg>
            </div>
            <span class="action-title">Add Car</span>
            <span class="action-desc">Register your EV</span>
          </a>
        </div>
      </section>
      
      <section class="section">
        <h2 class="section-title">Recent Activity</h2>
        <div class="empty-state glass-card">
          <svg class="empty-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
            <path d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2"/>
            <rect x="9" y="3" width="6" height="4" rx="1"/>
            <path d="M9 12h6"/>
            <path d="M9 16h6"/>
          </svg>
          <p class="empty-text">No recent activity</p>
          <p class="empty-hint">Start by logging your first charging session</p>
        </div>
      </section>
    </div>
  `,
  styles: [`
    .dashboard {
      max-width: 1200px;
    }
    
    .header {
      margin-bottom: 32px;
    }
    
    .heading-2 {
      color: var(--text-primary);
      margin: 0 0 8px;
    }
    
    .subtitle {
      color: var(--text-secondary);
      margin: 0;
    }
    
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
      gap: 20px;
      margin-bottom: 40px;
    }
    
    .stat-card {
      display: flex;
      align-items: center;
      gap: 16px;
      padding: 24px;
    }
    
    .stat-icon {
      width: 48px;
      height: 48px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    
    .stat-icon svg {
      width: 24px;
      height: 24px;
    }
    
    .stat-icon.energy {
      background: linear-gradient(135deg, rgba(16, 185, 129, 0.2), rgba(6, 182, 212, 0.2));
      color: var(--energy-green);
    }
    
    .stat-icon.gangs {
      background: linear-gradient(135deg, rgba(59, 130, 246, 0.2), rgba(139, 92, 246, 0.2));
      color: var(--aurora-blue);
    }
    
    .stat-icon.cars {
      background: linear-gradient(135deg, rgba(139, 92, 246, 0.2), rgba(236, 72, 153, 0.2));
      color: var(--aurora-purple);
    }
    
    .stat-icon.cost {
      background: linear-gradient(135deg, rgba(245, 158, 11, 0.2), rgba(239, 68, 68, 0.2));
      color: #fbbf24;
    }
    
    .stat-content {
      display: flex;
      flex-direction: column;
    }
    
    .stat-value {
      font-size: 1.75rem;
      font-weight: 600;
      color: var(--text-primary);
    }
    
    .stat-value small {
      font-size: 0.875rem;
      font-weight: 400;
      color: var(--text-secondary);
    }
    
    .stat-label {
      font-size: 0.875rem;
      color: var(--text-secondary);
    }
    
    .section {
      margin-bottom: 40px;
    }
    
    .section-title {
      font-size: 1.25rem;
      font-weight: 600;
      margin: 0 0 20px;
      color: var(--text-primary);
    }
    
    .actions-grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
      gap: 16px;
    }
    
    .action-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      padding: 24px;
      text-decoration: none;
      cursor: pointer;
      transition: transform 0.2s ease;
    }
    
    .action-card:hover {
      transform: translateY(-4px);
    }
    
    .action-icon {
      width: 56px;
      height: 56px;
      border-radius: 16px;
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 16px;
    }
    
    .action-icon svg {
      width: 28px;
      height: 28px;
    }
    
    .action-title {
      font-weight: 600;
      color: var(--text-primary);
      margin-bottom: 4px;
    }
    
    .action-desc {
      font-size: 0.8rem;
      color: var(--text-muted);
    }
    
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 48px;
      text-align: center;
    }
    
    .empty-icon {
      width: 48px;
      height: 48px;
      color: var(--text-muted);
      margin-bottom: 16px;
    }
    
    .empty-text {
      color: var(--text-secondary);
      margin: 0 0 4px;
    }
    
    .empty-hint {
      color: var(--text-muted);
      font-size: 0.875rem;
      margin: 0;
    }
  `]
})
export class DashboardComponent {
  readonly authService = inject(AuthService);
}
