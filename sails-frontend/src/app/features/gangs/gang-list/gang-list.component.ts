import { Component } from '@angular/core';

@Component({
  selector: 'app-gang-list',
  standalone: true,
  template: `
    <div class="page">
      <header class="header">
        <h1 class="heading-2">My Gangs</h1>
        <p class="subtitle">Manage your energy-sharing groups</p>
      </header>
      
      <div class="empty-state glass-card">
        <svg class="empty-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
          <circle cx="9" cy="7" r="4"/>
          <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
          <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
        </svg>
        <p class="empty-text">No gangs yet</p>
        <p class="empty-hint">Create a gang or ask to join one</p>
        <button class="btn-primary">
          <span>Create Gang</span>
        </button>
      </div>
    </div>
  `,
  styles: [`
    .page { max-width: 1000px; }
    .header { margin-bottom: 32px; }
    .heading-2 { margin: 0 0 8px; }
    .subtitle { color: var(--text-secondary); margin: 0; }
    .empty-state {
      display: flex;
      flex-direction: column;
      align-items: center;
      padding: 64px;
      text-align: center;
    }
    .empty-icon {
      width: 64px;
      height: 64px;
      color: var(--text-muted);
      margin-bottom: 20px;
    }
    .empty-text { color: var(--text-secondary); margin: 0 0 4px; font-size: 1.125rem; }
    .empty-hint { color: var(--text-muted); margin: 0 0 24px; }
  `]
})
export class GangListComponent {}
