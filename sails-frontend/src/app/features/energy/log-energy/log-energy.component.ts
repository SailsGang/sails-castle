import { Component } from '@angular/core';

@Component({
  selector: 'app-log-energy',
  standalone: true,
  template: `
    <div class="page">
      <header class="header">
        <h1 class="heading-2">Log Energy</h1>
        <p class="subtitle">Record a charging session</p>
      </header>
      
      <div class="form-card glass-card">
        <p class="coming-soon">Coming in Sprint 4</p>
      </div>
    </div>
  `,
  styles: [`
    .page { max-width: 600px; }
    .header { margin-bottom: 32px; }
    .heading-2 { margin: 0 0 8px; }
    .subtitle { color: var(--text-secondary); margin: 0; }
    .form-card { padding: 48px; text-align: center; }
    .coming-soon { color: var(--text-muted); margin: 0; }
  `]
})
export class LogEnergyComponent {}
