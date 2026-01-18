import { Component } from '@angular/core';

@Component({
  selector: 'app-car-list',
  standalone: true,
  template: `
    <div class="page">
      <header class="header">
        <h1 class="heading-2">My Cars</h1>
        <p class="subtitle">Manage your electric vehicles</p>
      </header>
      
      <div class="empty-state glass-card">
        <svg class="empty-icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.5">
          <path d="M5 11l1.5-4.5a2 2 0 0 1 1.9-1.5h7.2a2 2 0 0 1 1.9 1.5L19 11"/>
          <path d="M5 11h14v7a2 2 0 0 1-2 2H7a2 2 0 0 1-2-2v-7z"/>
          <circle cx="7.5" cy="15.5" r="1.5"/>
          <circle cx="16.5" cy="15.5" r="1.5"/>
        </svg>
        <p class="empty-text">No cars registered</p>
        <p class="empty-hint">Add your first EV to start tracking</p>
        <button class="btn-primary">
          <span>Add Car</span>
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
export class CarListComponent {}
