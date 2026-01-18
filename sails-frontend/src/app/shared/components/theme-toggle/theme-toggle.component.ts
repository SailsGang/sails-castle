import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeService } from '../../../core/services/theme.service';

@Component({
  selector: 'app-theme-toggle',
  standalone: true,
  imports: [CommonModule],
  template: `
    <button 
      type="button"
      class="theme-toggle"
      (click)="themeService.toggleTheme()"
      [attr.aria-label]="themeService.isDark() ? 'Switch to light mode' : 'Switch to dark mode'"
    >
      <div class="toggle-track" [class.light]="!themeService.isDark()">
        <div class="toggle-thumb">
          @if (themeService.isDark()) {
            <svg class="icon moon" viewBox="0 0 24 24" fill="currentColor">
              <path d="M21 12.79A9 9 0 1 1 11.21 3 7 7 0 0 0 21 12.79z"/>
            </svg>
          } @else {
            <svg class="icon sun" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="12" cy="12" r="5"/>
              <line x1="12" y1="1" x2="12" y2="3"/>
              <line x1="12" y1="21" x2="12" y2="23"/>
              <line x1="4.22" y1="4.22" x2="5.64" y2="5.64"/>
              <line x1="18.36" y1="18.36" x2="19.78" y2="19.78"/>
              <line x1="1" y1="12" x2="3" y2="12"/>
              <line x1="21" y1="12" x2="23" y2="12"/>
              <line x1="4.22" y1="19.78" x2="5.64" y2="18.36"/>
              <line x1="18.36" y1="5.64" x2="19.78" y2="4.22"/>
            </svg>
          }
        </div>
      </div>
    </button>
  `,
  styles: [`
    .theme-toggle {
      background: transparent;
      border: none;
      padding: 0;
      cursor: pointer;
      outline: none;
    }
    
    .toggle-track {
      width: 56px;
      height: 28px;
      background: linear-gradient(135deg, #1e1b4b, #312e81);
      border-radius: 14px;
      padding: 3px;
      transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
      position: relative;
      overflow: hidden;
    }
    
    .toggle-track::before {
      content: '';
      position: absolute;
      top: 3px;
      left: 3px;
      right: 3px;
      bottom: 3px;
      background: radial-gradient(circle at 10% 50%, rgba(139, 92, 246, 0.3), transparent 50%);
      border-radius: inherit;
    }
    
    .toggle-track.light {
      background: linear-gradient(135deg, #93c5fd, #60a5fa);
    }
    
    .toggle-track.light::before {
      background: radial-gradient(circle at 90% 50%, rgba(251, 191, 36, 0.4), transparent 50%);
    }
    
    .toggle-thumb {
      width: 22px;
      height: 22px;
      background: #ffffff;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.2);
    }
    
    .toggle-track.light .toggle-thumb {
      transform: translateX(28px);
      background: #fef3c7;
    }
    
    .icon {
      width: 14px;
      height: 14px;
      transition: all 0.3s ease;
    }
    
    .icon.moon {
      color: #6366f1;
    }
    
    .icon.sun {
      color: #f59e0b;
    }
    
    .theme-toggle:hover .toggle-thumb {
      transform: scale(1.05);
    }
    
    .toggle-track.light:hover .toggle-thumb {
      transform: translateX(28px) scale(1.05);
    }
    
    .theme-toggle:focus-visible .toggle-track {
      outline: 2px solid var(--aurora-blue);
      outline-offset: 2px;
    }
  `]
})
export class ThemeToggleComponent {
  readonly themeService = inject(ThemeService);
}
