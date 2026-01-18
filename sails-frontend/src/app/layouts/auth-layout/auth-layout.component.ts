import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuroraBackgroundComponent } from '../../shared/components/aurora-background/aurora-background.component';
import { LogoComponent } from '../../shared/components/logo/logo.component';
import { GlassCardComponent } from '../../shared/components/glass-card/glass-card.component';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterOutlet, AuroraBackgroundComponent, LogoComponent, GlassCardComponent],
  template: `
    <app-aurora-background>
      <div class="auth-layout">
        <div class="auth-content">
          <!-- Logo/Brand -->
          <div class="brand">
            <app-logo size="large" />
            <p class="brand-tagline">EV charging cost-sharing made simple</p>
          </div>
          
          <!-- Form Container -->
          <app-glass-card>
            <router-outlet />
          </app-glass-card>
          
          <!-- Footer -->
          <p class="footer">© 2026 SailsEnergy. Share the power.</p>
        </div>
      </div>
    </app-aurora-background>
  `,
  styles: [`
    .auth-layout {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 24px;
    }
    
    .auth-content {
      width: 100%;
      max-width: 500px;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 32px;
      animation: fadeIn 0.8s cubic-bezier(0.4, 0, 0.2, 1);
    }
    
    @keyframes fadeIn {
      from {
        opacity: 0;
        transform: translateY(30px);
      }
      to {
        opacity: 1;
        transform: translateY(0);
      }
    }
    
    .brand {
      text-align: center;
    }
    
    .brand-tagline {
      color: var(--text-secondary);
      margin: 12px 0 0;
      font-size: 0.95rem;
    }
    
    .footer {
      color: var(--text-muted);
      font-size: 0.8rem;
      margin: 0;
    }
    
    /* Mobile responsive */
    @media (max-width: 480px) {
      .auth-content {
        gap: 24px;
      }
    }
  `]
})
export class AuthLayoutComponent {}
