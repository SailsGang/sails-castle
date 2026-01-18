import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/auth/auth.service';

// Sub-components
import { HeroStatCardComponent } from './components/hero-stat-card.component';
import { QuickActionsComponent } from './components/quick-actions.component';
import { ActivityFeedComponent } from './components/activity-feed.component';
import { StatCardComponent } from './components/stat-card.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    HeroStatCardComponent,
    QuickActionsComponent,
    ActivityFeedComponent,
    StatCardComponent
  ],
  template: `
    <div class="dashboard-container">
      <!-- Welcome Header -->
      <header class="welcome-header">
        <div class="header-content">
          <h1 class="greeting">
            Good afternoon, <span class="accent-text">{{ firstName() }}</span>
          </h1>
          <p class="date">{{ currentDate | date:'EEEE, MMMM d' }}</p>
        </div>
      </header>

      <!-- Bento Grid -->
      <div class="bento-grid">
        
        <!-- 1. Hero Stat Card (Total Spend + Chart) -->
        <app-hero-stat-card></app-hero-stat-card>

        <!-- 2. Quick Actions Grid -->
        <app-quick-actions (action)="onQuickAction($event)"></app-quick-actions>

        <!-- 3. Secondary Stats (Split) -->
        <!-- Energy Card -->
        <app-stat-card 
          value="320" 
          unit="kWh" 
          label="Energy Logged" 
          color="green">
            <!-- Icon Projection -->
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M18 20V10"/>
              <path d="M12 20V4"/>
              <path d="M6 20v-6"/>
            </svg>
        </app-stat-card>

        <!-- Gangs Card -->
        <app-stat-card 
          value="2" 
          label="Active Gangs" 
          color="purple">
            <!-- Icon Projection -->
            <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
              <circle cx="9" cy="7" r="4"/>
              <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
            </svg>
        </app-stat-card>

        <!-- 4. Recent Activity Feed -->
        <app-activity-feed></app-activity-feed>

      </div>
    </div>
  `,
  styles: [`
    :host {
      display: block;
      padding: 0 0 40px;
    }

    .dashboard-container {
      max-width: 1200px;
      margin: 0 auto;
      padding: 0 24px;
    }

    /* Header */
    .welcome-header {
      margin-bottom: 32px;
      padding-top: 16px;
    }

    .greeting {
      font-size: 2rem;
      font-weight: 700;
      color: var(--text-primary);
      margin: 0 0 4px;
      letter-spacing: -0.02em;
    }

    .accent-text {
      background: linear-gradient(135deg, var(--aurora-blue), var(--aurora-purple));
      -webkit-background-clip: text;
      -webkit-text-fill-color: transparent;
      background-clip: text;
    }

    .date {
      color: var(--text-secondary);
      font-size: 1rem;
      font-weight: 500;
    }

    /* Bento Grid Layout */
    .bento-grid {
      display: grid;
      grid-template-columns: repeat(12, 1fr);
      grid-template-rows: auto auto; /* Auto rows */
      gap: 24px;
    }

    /* Responsive */
    @media (max-width: 1024px) {
      .bento-grid {
        display: flex;
        flex-direction: column;
      }
    }
  `]
})
export class DashboardComponent {
  private authService = inject(AuthService);
  
  readonly currentDate = new Date();
  
  readonly firstName = computed(() => {
    const name = this.authService.user()?.displayName;
    if (!name) return 'there';
    return name.split(' ')[0]; // Returns first name or full name if no space
  });

  onQuickAction(type: string): void {
    console.log('Quick action:', type);
    // Future: Router navigation
  }
}
