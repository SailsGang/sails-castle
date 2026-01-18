import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-overlay',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="overlay" [class.visible]="isVisible" [attr.aria-busy]="isVisible">
      <div class="overlay-content">
        @if (showSpinner) {
          <div class="spinner">
            <svg viewBox="0 0 50 50">
              <circle cx="25" cy="25" r="20" fill="none" stroke="url(#gradient)" stroke-width="4"/>
              <defs>
                <linearGradient id="gradient" x1="0%" y1="0%" x2="100%" y2="0%">
                  <stop offset="0%" stop-color="var(--aurora-blue)"/>
                  <stop offset="100%" stop-color="var(--aurora-purple)"/>
                </linearGradient>
              </defs>
            </svg>
          </div>
        }
        @if (message) {
          <p class="message">{{ message }}</p>
        }
      </div>
    </div>
  `,
  styles: [`
    .overlay {
      position: fixed;
      inset: 0;
      background: rgba(10, 10, 26, 0.8);
      backdrop-filter: blur(4px);
      display: flex;
      align-items: center;
      justify-content: center;
      opacity: 0;
      visibility: hidden;
      transition: all 0.3s ease;
      z-index: 1000;
    }
    
    .overlay.visible {
      opacity: 1;
      visibility: visible;
    }
    
    .overlay-content {
      text-align: center;
    }
    
    .spinner {
      width: 48px;
      height: 48px;
      margin: 0 auto;
    }
    
    .spinner svg {
      width: 100%;
      height: 100%;
      animation: spin 1s linear infinite;
    }
    
    .spinner circle {
      stroke-dasharray: 80;
      stroke-dashoffset: 20;
      stroke-linecap: round;
    }
    
    @keyframes spin {
      from { transform: rotate(0deg); }
      to { transform: rotate(360deg); }
    }
    
    .message {
      margin: 16px 0 0;
      color: var(--text-secondary);
      font-size: 0.9rem;
    }
  `]
})
export class LoadingOverlayComponent {
  @Input() isVisible = false;
  @Input() message = '';
  @Input() showSpinner = true;
}
