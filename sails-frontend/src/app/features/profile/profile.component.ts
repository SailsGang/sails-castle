import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/auth/auth.service';

@Component({
  selector: 'app-profile',
  standalone: true,
  template: `
    <div class="page">
      <header class="header">
        <h1 class="heading-2">Profile</h1>
        <p class="subtitle">Manage your account settings</p>
      </header>
      
      <div class="profile-card glass-card">
        <div class="avatar">{{ getUserInitials() }}</div>
        <div class="info">
          <h2 class="name">{{ authService.user()?.displayName }}</h2>
          <p class="email">{{ authService.user()?.email }}</p>
        </div>
      </div>
      
      <div class="coming-soon glass-card">
        <p>Profile editing coming in Sprint 6</p>
      </div>
    </div>
  `,
  styles: [`
    .page { max-width: 600px; }
    .header { margin-bottom: 32px; }
    .heading-2 { margin: 0 0 8px; }
    .subtitle { color: var(--text-secondary); margin: 0; }
    .profile-card {
      display: flex;
      align-items: center;
      gap: 20px;
      padding: 24px;
      margin-bottom: 24px;
    }
    .avatar {
      width: 72px;
      height: 72px;
      border-radius: 50%;
      background: linear-gradient(135deg, #3b82f6, #8b5cf6);
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.5rem;
      font-weight: 600;
    }
    .name { margin: 0 0 4px; font-size: 1.25rem; }
    .email { margin: 0; color: var(--text-secondary); }
    .coming-soon { padding: 32px; text-align: center; color: var(--text-muted); }
  `]
})
export class ProfileComponent {
  readonly authService = inject(AuthService);
  
  getUserInitials(): string {
    const name = this.authService.user()?.displayName || '';
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }
}
