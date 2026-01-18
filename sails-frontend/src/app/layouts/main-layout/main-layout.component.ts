import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <div class="main-layout">
      <!-- Aurora Background -->
      <div class="aurora-background">
        <div class="aurora-cyan"></div>
      </div>
      
      <!-- Sidebar -->
      <aside class="sidebar glass-card">
        <div class="sidebar-header">
          <div class="logo">
            <svg viewBox="0 0 32 32" fill="none">
              <circle cx="16" cy="16" r="14" stroke="url(#logo-grad)" stroke-width="2.5"/>
              <path d="M11 16L14 19L21 12" stroke="url(#logo-grad)" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"/>
              <defs>
                <linearGradient id="logo-grad" x1="4" y1="4" x2="28" y2="28" gradientUnits="userSpaceOnUse">
                  <stop stop-color="#3b82f6"/>
                  <stop offset="1" stop-color="#8b5cf6"/>
                </linearGradient>
              </defs>
            </svg>
          </div>
          <span class="brand-text">SailsEnergy</span>
        </div>
        
        <nav class="nav">
          <a routerLink="/dashboard" routerLinkActive="active" class="nav-item">
            <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <rect x="3" y="3" width="7" height="7" rx="1"/>
              <rect x="14" y="3" width="7" height="7" rx="1"/>
              <rect x="3" y="14" width="7" height="7" rx="1"/>
              <rect x="14" y="14" width="7" height="7" rx="1"/>
            </svg>
            <span>Dashboard</span>
          </a>
          
          <a routerLink="/gangs" routerLinkActive="active" class="nav-item">
            <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
              <circle cx="9" cy="7" r="4"/>
              <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
              <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
            </svg>
            <span>My Gangs</span>
          </a>
          
          <a routerLink="/cars" routerLinkActive="active" class="nav-item">
            <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M5 11l1.5-4.5a2 2 0 0 1 1.9-1.5h7.2a2 2 0 0 1 1.9 1.5L19 11"/>
              <path d="M5 11h14v7a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2v-7z"/>
              <circle cx="7.5" cy="15.5" r="1.5"/>
              <circle cx="16.5" cy="15.5" r="1.5"/>
            </svg>
            <span>My Cars</span>
          </a>
          
          <a routerLink="/energy/log" routerLinkActive="active" class="nav-item accent">
            <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <polygon points="13,2 3,14 12,14 11,22 21,10 12,10"/>
            </svg>
            <span>Log Energy</span>
          </a>
        </nav>
        
        <div class="sidebar-footer">
          <div class="user-info">
            <div class="avatar">{{ getUserInitials() }}</div>
            <div class="user-details">
              <span class="user-name">{{ authService.user()?.displayName }}</span>
              <span class="user-email">{{ authService.user()?.email }}</span>
            </div>
          </div>
          <button class="logout-btn" (click)="logout()" title="Sign out">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/>
              <polyline points="16,17 21,12 16,7"/>
              <line x1="21" y1="12" x2="9" y2="12"/>
            </svg>
          </button>
        </div>
      </aside>
      
      <!-- Main Content -->
      <main class="main-content">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .main-layout {
      display: flex;
      min-height: 100vh;
    }
    
    .sidebar {
      width: 260px;
      display: flex;
      flex-direction: column;
      position: fixed;
      top: 16px;
      left: 16px;
      bottom: 16px;
      padding: 20px;
      z-index: 100;
    }
    
    .sidebar-header {
      display: flex;
      align-items: center;
      gap: 12px;
      padding-bottom: 24px;
      border-bottom: 1px solid var(--glass-border);
      margin-bottom: 24px;
    }
    
    .logo {
      width: 32px;
      height: 32px;
    }
    
    .logo svg {
      width: 100%;
      height: 100%;
    }
    
    .brand-text {
      font-size: 1.25rem;
      font-weight: 600;
      background: linear-gradient(135deg, #3b82f6, #8b5cf6);
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
    }
    
    .nav {
      display: flex;
      flex-direction: column;
      gap: 8px;
      flex: 1;
    }
    
    .nav-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 12px 16px;
      border-radius: 10px;
      color: var(--text-secondary);
      text-decoration: none;
      transition: all 0.2s ease;
    }
    
    .nav-item:hover {
      background: var(--glass-bg);
      color: var(--text-primary);
    }
    
    .nav-item.active {
      background: linear-gradient(135deg, rgba(59, 130, 246, 0.2), rgba(139, 92, 246, 0.2));
      color: var(--text-primary);
      border: 1px solid rgba(59, 130, 246, 0.3);
    }
    
    .nav-item.accent {
      background: linear-gradient(135deg, rgba(16, 185, 129, 0.15), rgba(6, 182, 212, 0.15));
      color: var(--energy-green);
      margin-top: 16px;
    }
    
    .nav-item.accent:hover {
      background: linear-gradient(135deg, rgba(16, 185, 129, 0.25), rgba(6, 182, 212, 0.25));
    }
    
    .nav-icon {
      width: 20px;
      height: 20px;
      flex-shrink: 0;
    }
    
    .sidebar-footer {
      display: flex;
      align-items: center;
      gap: 12px;
      padding-top: 16px;
      border-top: 1px solid var(--glass-border);
    }
    
    .user-info {
      display: flex;
      align-items: center;
      gap: 12px;
      flex: 1;
      min-width: 0;
    }
    
    .avatar {
      width: 36px;
      height: 36px;
      border-radius: 50%;
      background: linear-gradient(135deg, #3b82f6, #8b5cf6);
      display: flex;
      align-items: center;
      justify-content: center;
      font-weight: 600;
      font-size: 0.875rem;
      flex-shrink: 0;
    }
    
    .user-details {
      display: flex;
      flex-direction: column;
      min-width: 0;
    }
    
    .user-name {
      font-weight: 500;
      font-size: 0.875rem;
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    
    .user-email {
      font-size: 0.75rem;
      color: var(--text-muted);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }
    
    .logout-btn {
      width: 36px;
      height: 36px;
      border-radius: 8px;
      border: 1px solid var(--glass-border);
      background: transparent;
      color: var(--text-secondary);
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.2s ease;
    }
    
    .logout-btn:hover {
      background: rgba(239, 68, 68, 0.15);
      border-color: rgba(239, 68, 68, 0.3);
      color: #f87171;
    }
    
    .logout-btn svg {
      width: 18px;
      height: 18px;
    }
    
    .main-content {
      flex: 1;
      margin-left: 292px;
      padding: 24px;
      min-height: 100vh;
    }
    
    @media (max-width: 768px) {
      .sidebar {
        display: none;
      }
      
      .main-content {
        margin-left: 0;
      }
    }
  `]
})
export class MainLayoutComponent {
  readonly authService = inject(AuthService);
  
  getUserInitials(): string {
    const name = this.authService.user()?.displayName || '';
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }
  
  async logout(): Promise<void> {
    await this.authService.logout();
  }
}
