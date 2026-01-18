import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { AuroraBackgroundComponent } from '../../shared/components/aurora-background/aurora-background.component';
import { LogoComponent } from '../../shared/components/logo/logo.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, AuroraBackgroundComponent, LogoComponent, SidebarComponent],
  template: `
    <app-aurora-background>
      <div class="layout-container">
        <!-- Mobile Header -->
        <header class="mobile-header">
          <app-logo size="small" />
          <button class="menu-btn" (click)="toggleSidebar()" aria-label="Open menu">
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <line x1="3" y1="12" x2="21" y2="12"/>
              <line x1="3" y1="6" x2="21" y2="6"/>
              <line x1="3" y1="18" x2="21" y2="18"/>
            </svg>
          </button>
        </header>

        <!-- Sidebar -->
        <div class="sidebar-wrapper" [class.open]="isSidebarOpen()">
          <app-sidebar (close)="closeSidebar()" />
        </div>

        <!-- Overlay for mobile -->
        @if (isSidebarOpen()) {
          <div class="sidebar-overlay" (click)="closeSidebar()"></div>
        }

        <!-- Main Content -->
        <main class="main-content">
          <div class="content-wrapper">
            <router-outlet />
          </div>
        </main>
      </div>
    </app-aurora-background>
  `,
  styles: [`
    .layout-container {
      display: flex;
      min-height: 100vh;
      position: relative;
    }

    /* Mobile Header */
    .mobile-header {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      height: 64px;
      padding: 0 16px;
      background: rgba(18, 18, 42, 0.8);
      backdrop-filter: blur(20px);
      -webkit-backdrop-filter: blur(20px);
      border-bottom: 1px solid var(--glass-border);
      display: flex;
      align-items: center;
      justify-content: space-between;
      z-index: 40;
    }

    .menu-btn {
      background: transparent;
      border: none;
      color: var(--text-primary);
      padding: 8px;
    }

    .menu-btn svg {
      width: 24px;
      height: 24px;
    }

    /* Sidebar Wrapper */
    .sidebar-wrapper {
      position: fixed;
      top: 0;
      left: 0;
      bottom: 0;
      width: 260px;
      z-index: 50;
      transform: translateX(-100%);
      transition: transform 0.3s cubic-bezier(0.4, 0, 0.2, 1);
    }

    .sidebar-wrapper.open {
      transform: translateX(0);
    }

    .sidebar-overlay {
      position: fixed;
      inset: 0;
      background: rgba(0, 0, 0, 0.5);
      backdrop-filter: blur(2px);
      z-index: 45;
      animation: fadeIn 0.3s ease;
    }

    @keyframes fadeIn {
      from { opacity: 0; }
      to { opacity: 1; }
    }

    /* Main Content */
    .main-content {
      flex: 1;
      min-width: 0; /* Prevent overflow */
      padding-top: 64px; /* Mobile header height */
    }

    .content-wrapper {
      padding: 24px;
      max-width: 1200px;
      margin: 0 auto;
      animation: fadeIn 0.5s ease-out;
    }

    /* Desktop Styles */
    @media (min-width: 769px) {
      .mobile-header {
        display: none;
      }

      .sidebar-wrapper {
        position: sticky;
        top: 0;
        height: 100vh;
        transform: none;
        box-shadow: none; /* Shadow handled by sidebar component */
        overflow-y: auto; /* Allow internal scrolling */
      }

      .main-content {
        padding-top: 0;
      }
      
      .sidebar-overlay {
        display: none;
      }
    }
  `]
})
export class MainLayoutComponent {
  readonly isSidebarOpen = signal(false);

  toggleSidebar(): void {
    this.isSidebarOpen.update(v => !v);
  }

  closeSidebar(): void {
    this.isSidebarOpen.set(false);
  }
}
