import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-gang-list',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="page-container">
      <div class="header">
        <div>
          <h1 class="page-title">My Gangs</h1>
          <p class="subtitle">Manage your charging groups</p>
        </div>
        <button class="create-btn">
          + Create Gang
        </button>
      </div>
      
      <!-- Placeholder content -->
      <div class="empty-state">
        <div class="icon-circle">
          <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
            <circle cx="9" cy="7" r="4"/>
            <path d="M23 21v-2a4 4 0 0 0-3-3.87"/>
            <path d="M16 3.13a4 4 0 0 1 0 7.75"/>
          </svg>
        </div>
        <h3>No gangs yet</h3>
        <p>Create a gang or join one to start sharing costs.</p>
      </div>
    </div>
  `,
  styles: [`
    .page-container {
      animation: fadeIn 0.4s ease-out;
    }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: flex-start;
      margin-bottom: 32px;
    }

    .page-title {
      font-size: 1.8rem;
      font-weight: 700;
      margin: 0 0 8px;
    }
    
    .subtitle {
      color: var(--text-secondary);
      margin: 0;
    }

    .create-btn {
      background: linear-gradient(135deg, var(--aurora-blue), var(--aurora-purple));
      color: white;
      border: none;
      padding: 10px 20px;
      border-radius: 10px;
      font-weight: 600;
      cursor: pointer;
      transition: transform 0.2s, opacity 0.2s;
    }

    .create-btn:hover {
      transform: translateY(-1px);
      opacity: 0.9;
    }
    
    .empty-state {
      background: var(--bg-surface);
      border: 1px solid var(--glass-border);
      border-radius: 16px;
      padding: 64px 24px;
      text-align: center;
      display: flex;
      flex-direction: column;
      align-items: center;
    }
    
    .icon-circle {
      width: 64px;
      height: 64px;
      border-radius: 50%;
      background: rgba(59, 130, 246, 0.1);
      color: var(--aurora-blue);
      display: flex;
      align-items: center;
      justify-content: center;
      margin-bottom: 24px;
    }
    
    .icon-circle svg {
      width: 32px;
      height: 32px;
    }

    .empty-state h3 {
      font-size: 1.2rem;
      font-weight: 600;
      margin: 0 0 8px;
      color: var(--text-primary);
    }

    .empty-state p {
      color: var(--text-secondary);
      margin: 0;
    }

    @keyframes fadeIn {
      from { opacity: 0; transform: translateY(10px); }
      to { opacity: 1; transform: translateY(0); }
    }
  `]
})
export class GangListComponent {}
