import { Component, EventEmitter, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../../../core/auth/auth.service';
import { LogoComponent } from '../../../../shared/components/logo/logo.component';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, LogoComponent],
  template: `
    <aside class="sidebar-container">
      <div class="sidebar-glass">
        <!-- Spotlight Layer -->
        <div class="spotlight-layer" [style.background]="spotlightStyle()"></div>

        <!-- Header (Centered Logo) -->
        <div class="sidebar-header">
          <app-logo size="large" />
          <button class="close-btn" (click)="close.emit()" aria-label="Close menu">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="18" y1="6" x2="6" y2="18"/>
              <line x1="6" y1="6" x2="18" y2="18"/>
            </svg>
          </button>
        </div>

        <!-- Navigation -->
        <nav class="sidebar-nav" (mousemove)="onMouseMove($event)" (mouseleave)="onMouseLeave()">
          
          <span class="nav-label">MENU</span>
          
          <!-- Dashboard (Blue) -->
          <a routerLink="/dashboard" routerLinkActive="active-blue" class="nav-item item-blue">
            <div class="nav-icon-wrapper">
              <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <rect x="3" y="3" width="7" height="7"/>
                <rect x="14" y="3" width="7" height="7"/>
                <rect x="14" y="14" width="7" height="7"/>
                <rect x="3" y="14" width="7" height="7"/>
              </svg>
            </div>
            <span>Dashboard</span>
          </a>

          <!-- Gangs (Purple) -->
          <a routerLink="/gangs" routerLinkActive="active-purple" class="nav-item item-purple">
            <div class="nav-icon-wrapper">
              <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                <circle cx="9" cy="7" r="4"/>
                <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
                <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
              </svg>
            </div>
            <span>My Gangs</span>
          </a>

          <!-- Cars (Cyan) -->
          <a routerLink="/cars" routerLinkActive="active-cyan" class="nav-item item-cyan">
            <div class="nav-icon-wrapper">
              <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                 <path d="M19 17h2c.6 0 1-.4 1-1v-3c0-.9-.7-1.7-1.5-1.9C18.7 10.6 16 10 16 10s-1.3-1.4-2.2-2.3c-.5-.4-1.1-.7-1.8-.7H5c-.6 0-1.1.4-1.4.9l-1.4 2.9A3.7 3.7 0 0 0 2 12v4c0 .6.4 1 1 1h2"/>
                 <circle cx="7" cy="17" r="2"/>
                 <path d="M9 17h6"/>
                 <circle cx="17" cy="17" r="2"/>
              </svg>
            </div>
            <span>My Cars</span>
          </a>

          <!-- Energy (Green) -->
          <a routerLink="/energy" routerLinkActive="active-green" class="nav-item item-green">
            <div class="nav-icon-wrapper">
              <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <polygon points="13 2 3 14 12 14 11 22 21 10 12 10 13 2"/>
              </svg>
            </div>
            <span>Activity Log</span>
          </a>

          <span class="nav-label mt-4">SETTINGS</span>

          <!-- Profile (Orange/Default) -->
          <a routerLink="/profile" routerLinkActive="active-orange" class="nav-item item-orange">
            <div class="nav-icon-wrapper">
              <svg class="nav-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <circle cx="12" cy="12" r="3"/>
                <path d="M19.4 15a1.65 1.65 0 0 0 .33 1.82l.06.06a2 2 0 0 1 0 2.83 2 2 0 0 1-2.83 0l-.06-.06a1.65 1.65 0 0 0-1.82-.33 1.65 1.65 0 0 0-1 1.51V21a2 2 0 0 1-2 2 2 2 0 0 1-2-2v-.09A1.65 1.65 0 0 0 9 19.4a1.65 1.65 0 0 0-1.82.33l-.06.06a2 2 0 0 1-2.83 0 2 2 0 0 1 0-2.83l.06-.06a1.65 1.65 0 0 0 .33-1.82 1.65 1.65 0 0 0-1.51-1H3a2 2 0 0 1-2-2 2 2 0 0 1 2-2h.09A1.65 1.65 0 0 0 4.6 9a1.65 1.65 0 0 0-.33-1.82l-.06-.06a2 2 0 0 1 0-2.83 2 2 0 0 1 2.83 0l.06.06a1.65 1.65 0 0 0 1.82.33H9a1.65 1.65 0 0 0 1-1.51V3a2 2 0 0 1 2-2 2 2 0 0 1 2 2v.09a1.65 1.65 0 0 0 1 1.51 1.65 1.65 0 0 0 1.82-.33l.06-.06a2 2 0 0 1 2.83 0 2 2 0 0 1 0 2.83l-.06.06a1.65 1.65 0 0 0-.33 1.82V9a1.65 1.65 0 0 0 1.51 1H21a2 2 0 0 1 2 2 2 2 0 0 1-2 2h-.09a1.65 1.65 0 0 0-1.51 1z"/>
              </svg>
            </div>
            <span>Profile</span>
          </a>
        </nav>

        <!-- Premium Profile Footer -->
        <div class="sidebar-footer">
          <div class="profile-card">
            <!-- Avatar -->
            <div class="avatar-wrapper">
              <div class="avatar">
                <span>{{ getUserInitials() }}</span>
              </div>
            </div>
            
            <div class="user-info">
              <p class="user-name">{{ authService.user()?.displayName }}</p>
              <p class="user-email">{{ authService.user()?.email }}</p>
            </div>
            
            <button class="logout-btn" (click)="authService.logout()" aria-label="Sign out">
              <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/>
                <polyline points="16 17 21 12 16 7"/>
                <line x1="21" y1="12" x2="9" y2="12"/>
              </svg>
            </button>
          </div>
        </div>
      </div>
    </aside>
  `,
  styles: [`
    :host {
      display: block;
      height: 100%;
      pointer-events: none;
    }

    .sidebar-container {
      height: 100%;
      padding: 16px;
      padding-right: 0;
      pointer-events: auto;
      display: flex;
      flex-direction: column;
    }

    .sidebar-glass {
      position: relative;
      background: rgba(10, 10, 26, 0.6);
      backdrop-filter: blur(24px);
      -webkit-backdrop-filter: blur(24px);
      border: 1px solid rgba(255, 255, 255, 0.08);
      border-radius: 24px;
      height: 100%;
      display: flex;
      flex-direction: column;
      overflow: hidden;
      box-shadow: 0 20px 40px rgba(0, 0, 0, 0.2);
    }

    .spotlight-layer {
      position: absolute;
      inset: 0;
      pointer-events: none;
      z-index: 0;
      opacity: 0.6;
      transition: background 0.15s ease;
    }

    .sidebar-header {
      position: relative;
      z-index: 1;
      padding: 32px 24px 16px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
    
    .close-btn {
      position: absolute;
      right: 16px;
      top: 24px;
      display: none;
      background: transparent;
      border: none;
      color: var(--text-secondary);
      padding: 8px;
    }

    .sidebar-nav {
      position: relative;
      z-index: 1;
      flex: 1;
      padding: 8px 16px;
      display: flex;
      flex-direction: column;
      gap: 4px;
      overflow-y: auto;
    }

    .nav-label {
      font-size: 0.7rem;
      font-weight: 700;
      color: var(--text-muted);
      letter-spacing: 0.05em;
      margin: 16px 12px 8px;
      opacity: 0.6;
    }

    .nav-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 10px 12px;
      color: var(--text-secondary);
      text-decoration: none;
      border-radius: 14px;
      transition: all 0.2s ease;
      font-weight: 500;
      position: relative;
      border: 1px solid transparent;
    }

    .nav-item:hover {
      background: rgba(255, 255, 255, 0.03);
      color: var(--text-primary);
    }

    .nav-icon-wrapper {
      width: 32px;
      height: 32px;
      display: flex;
      align-items: center;
      justify-content: center;
      background: rgba(255, 255, 255, 0.05);
      border-radius: 10px;
      transition: all 0.2s;
    }

    .nav-icon {
      width: 18px;
      height: 18px;
    }

    /* Active States Colors */
    
    /* Blue - default/dashboard */
    .nav-item.active-blue {
      background: rgba(59, 130, 246, 0.1);
      color: var(--aurora-blue);
      border-color: rgba(59, 130, 246, 0.2);
    }
    .nav-item.active-blue .nav-icon-wrapper {
      background: linear-gradient(135deg, var(--aurora-blue), #60a5fa);
      color: white;
      box-shadow: 0 4px 12px rgba(59, 130, 246, 0.3);
    }

    /* Purple - gangs */
    .nav-item.active-purple {
      background: rgba(139, 92, 246, 0.1);
      color: var(--aurora-purple);
      border-color: rgba(139, 92, 246, 0.2);
    }
    .nav-item.active-purple .nav-icon-wrapper {
      background: linear-gradient(135deg, var(--aurora-purple), #a78bfa);
      color: white;
      box-shadow: 0 4px 12px rgba(139, 92, 246, 0.3);
    }

    /* Cyan - cars */
    .nav-item.active-cyan {
      background: rgba(6, 182, 212, 0.1);
      color: var(--aurora-cyan);
      border-color: rgba(6, 182, 212, 0.2);
    }
    .nav-item.active-cyan .nav-icon-wrapper {
      background: linear-gradient(135deg, var(--aurora-cyan), #22d3ee);
      color: white;
      box-shadow: 0 4px 12px rgba(6, 182, 212, 0.3);
    }

    /* Green - energy */
    .nav-item.active-green {
      background: rgba(16, 185, 129, 0.1);
      color: var(--energy-green);
      border-color: rgba(16, 185, 129, 0.2);
    }
    .nav-item.active-green .nav-icon-wrapper {
      background: linear-gradient(135deg, var(--energy-green), #34d399);
      color: white;
      box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
    }

    /* Orange - profile/settings */
    .nav-item.active-orange {
      background: rgba(249, 115, 22, 0.1);
      color: #f97316;
      border-color: rgba(249, 115, 22, 0.2);
    }
    .nav-item.active-orange .nav-icon-wrapper {
      background: linear-gradient(135deg, #f97316, #fb923c);
      color: white;
      box-shadow: 0 4px 12px rgba(249, 115, 22, 0.3);
    }

    /* Premium Profile Footer */
    .sidebar-footer {
      padding: 16px 16px 24px;
      margin-top: auto;
    }

    .profile-card {
      background: rgba(255, 255, 255, 0.03);
      border: 1px solid rgba(255, 255, 255, 0.08);
      border-radius: 18px;
      padding: 12px;
      display: flex;
      align-items: center;
      gap: 12px;
      transition: all 0.3s ease;
    }

    .profile-card:hover {
      background: rgba(255, 255, 255, 0.06);
      border-color: rgba(255, 255, 255, 0.15);
      transform: translateY(-2px);
      box-shadow: 0 8px 24px rgba(0, 0, 0, 0.2);
    }

    .avatar-wrapper {
      position: relative;
    }

    .avatar {
      width: 40px;
      height: 40px;
      border-radius: 50%;
      background: linear-gradient(135deg, var(--aurora-blue), var(--aurora-purple));
      display: flex;
      align-items: center;
      justify-content: center;
      color: white;
      font-weight: 600;
      font-size: 0.95rem;
      border: 2px solid rgba(255, 255, 255, 0.1);
    }

    .user-info {
      flex: 1;
      min-width: 0;
      display: flex;
      flex-direction: column;
      gap: 2px;
    }

    .user-name {
      margin: 0;
      font-weight: 600;
      font-size: 0.9rem;
      color: var(--text-primary);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .user-email {
      margin: 0;
      font-size: 0.75rem;
      color: var(--text-muted);
      white-space: nowrap;
      overflow: hidden;
      text-overflow: ellipsis;
    }

    .logout-btn {
      background: transparent;
      border: none;
      color: var(--text-muted);
      width: 32px;
      height: 32px;
      border-radius: 8px;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s;
      opacity: 0.6;
    }

    .profile-card:hover .logout-btn {
      opacity: 1;
    }

    .logout-btn:hover {
      color: #ef4444;
    }
    
    .logout-btn svg {
      width: 18px;
      height: 18px;
    }

    @media (max-width: 768px) {
      .sidebar-container {
        padding: 0;
      }
      
      .sidebar-glass {
        border-radius: 0;
        border: none;
        border-right: 1px solid rgba(255, 255, 255, 0.08);
      }

      .close-btn {
        display: block;
      }
      
      .sidebar-header {
        justify-content: flex-start;
        padding: 24px;
      }
      
      .sidebar-footer {
        padding: 16px;
      }
    }
    
    .mt-4 {
      margin-top: 24px;
    }
  `]
})
export class SidebarComponent {
  readonly authService = inject(AuthService);
  
  @Output() close = new EventEmitter<void>();
  
  spotlightStyle = signal('none');

  onMouseMove(e: MouseEvent): void {
    const el = e.currentTarget as HTMLElement;
    const rect = el.getBoundingClientRect();
    const x = e.clientX - rect.left;
    const y = e.clientY - rect.top;
    
    this.spotlightStyle.set(
      `radial-gradient(600px circle at ${x}px ${y}px, rgba(59, 130, 246, 0.06), transparent 40%)`
    );
  }

  onMouseLeave(): void {
    this.spotlightStyle.set('none');
  }

  getUserInitials(): string {
    const name = this.authService.user()?.displayName || '?';
    return name.slice(0, 2).toUpperCase();
  }
  
  onQuickAction(): void {
    console.log('Quick action clicked');
  }
}
