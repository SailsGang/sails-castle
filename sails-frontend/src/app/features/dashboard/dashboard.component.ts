import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="dashboard">
      <h1 class="page-title">Dashboard</h1>
      <p class="subtitle">Welcome to your energy overview</p>
      
      <!-- Placeholder content -->
      <div class="grid">
        <div class="card">
          <h3>Total Energy</h3>
          <p class="value">0 kWh</p>
        </div>
        <div class="card">
          <h3>Active Gangs</h3>
          <p class="value">0</p>
        </div>
        <div class="card">
          <h3>My Cars</h3>
          <p class="value">0</p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-title {
      font-size: 1.8rem;
      font-weight: 700;
      margin-bottom: 8px;
    }
    
    .subtitle {
      color: var(--text-secondary);
      margin-bottom: 32px;
    }
    
    .grid {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
      gap: 24px;
    }
    
    .card {
      background: var(--bg-surface);
      border: 1px solid var(--glass-border);
      border-radius: 16px;
      padding: 24px;
    }
    
    .card h3 {
      font-size: 0.9rem;
      color: var(--text-secondary);
      margin: 0 0 8px;
      font-weight: 500;
    }
    
    .card .value {
      font-size: 2rem;
      font-weight: 700;
      margin: 0;
      color: var(--text-primary);
    }
  `]
})
export class DashboardComponent {}
