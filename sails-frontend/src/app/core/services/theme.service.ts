import { Injectable, signal, effect, PLATFORM_ID, inject } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export type Theme = 'dark' | 'light';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private readonly platformId = inject(PLATFORM_ID);
  
  readonly theme = signal<Theme>('dark');
  readonly isDark = signal(true);
  
  constructor() {
    if (isPlatformBrowser(this.platformId)) {
      // Check stored preference
      const stored = localStorage.getItem('theme') as Theme;
      if (stored) {
        this.theme.set(stored);
        this.isDark.set(stored === 'dark');
      } else {
        // Check system preference
        const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        this.theme.set(prefersDark ? 'dark' : 'light');
        this.isDark.set(prefersDark);
      }
      
      this.applyTheme(this.theme());
    }
    
    // React to theme changes
    effect(() => {
      if (isPlatformBrowser(this.platformId)) {
        const currentTheme = this.theme();
        this.applyTheme(currentTheme);
        localStorage.setItem('theme', currentTheme);
      }
    });
  }
  
  toggleTheme(): void {
    const newTheme = this.theme() === 'dark' ? 'light' : 'dark';
    this.theme.set(newTheme);
    this.isDark.set(newTheme === 'dark');
  }
  
  setTheme(theme: Theme): void {
    this.theme.set(theme);
    this.isDark.set(theme === 'dark');
  }
  
  private applyTheme(theme: Theme): void {
    document.documentElement.classList.remove('dark', 'light');
    document.documentElement.classList.add(theme);
    
    // Update meta theme-color for mobile browsers
    const metaTheme = document.querySelector('meta[name="theme-color"]');
    if (metaTheme) {
      metaTheme.setAttribute('content', theme === 'dark' ? '#0a0a1a' : '#ffffff');
    }
  }
}
