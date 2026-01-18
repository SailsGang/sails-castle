import { Component, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-password-generator',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="generator-container" [class.expanded]="isExpanded()">
      <button 
        type="button" 
        class="toggle-btn"
        (click)="toggle()"
        [attr.aria-expanded]="isExpanded()"
      >
        <svg class="icon" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <rect x="3" y="11" width="18" height="11" rx="2" ry="2"/>
          <path d="M7 11V7a5 5 0 0 1 10 0v4"/>
        </svg>
        <span>Generate secure password</span>
        <svg class="chevron" [class.rotated]="isExpanded()" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
          <polyline points="6 9 12 15 18 9"/>
        </svg>
      </button>
      
      @if (isExpanded()) {
        <div class="generator-content" @slideDown>
          <div class="options">
            <label class="option">
              <input type="checkbox" [(ngModel)]="includeUppercase" (change)="generatePassword()" />
              <span>Uppercase (A-Z)</span>
            </label>
            <label class="option">
              <input type="checkbox" [(ngModel)]="includeLowercase" (change)="generatePassword()" />
              <span>Lowercase (a-z)</span>
            </label>
            <label class="option">
              <input type="checkbox" [(ngModel)]="includeNumbers" (change)="generatePassword()" />
              <span>Numbers (0-9)</span>
            </label>
            <label class="option">
              <input type="checkbox" [(ngModel)]="includeSpecial" (change)="generatePassword()" />
              <span>Special (!&#64;#$%)</span>
            </label>
          </div>
          
          <div class="length-slider">
            <label>Length: {{ passwordLength }}</label>
            <input 
              type="range" 
              min="8" 
              max="32" 
              [(ngModel)]="passwordLength"
              (input)="generatePassword()"
            />
          </div>
          
          <div class="preview-box">
            <code class="password-preview">{{ generatedPassword() }}</code>
            <button 
              type="button" 
              class="copy-btn"
              (click)="copyAndUse()"
              [class.copied]="copied()"
            >
              @if (copied()) {
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <polyline points="20 6 9 17 4 12"/>
                </svg>
              } @else {
                <svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <rect x="9" y="9" width="13" height="13" rx="2" ry="2"/>
                  <path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"/>
                </svg>
              }
            </button>
          </div>
          
          <button 
            type="button" 
            class="use-btn"
            (click)="copyAndUse()"
          >
            Use this password
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    .generator-container {
      border: 1px solid var(--glass-border);
      border-radius: 12px;
      overflow: hidden;
      transition: all 0.3s ease;
    }
    
    .generator-container.expanded {
      border-color: var(--aurora-purple);
      background: rgba(139, 92, 246, 0.05);
    }
    
    .toggle-btn {
      width: 100%;
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 14px 16px;
      background: transparent;
      border: none;
      color: var(--text-secondary);
      font-size: 0.9rem;
      cursor: pointer;
      transition: all 0.2s ease;
    }
    
    .toggle-btn:hover {
      color: var(--aurora-purple);
      background: rgba(139, 92, 246, 0.05);
    }
    
    .icon {
      width: 18px;
      height: 18px;
    }
    
    .chevron {
      width: 16px;
      height: 16px;
      margin-left: auto;
      transition: transform 0.3s ease;
    }
    
    .chevron.rotated {
      transform: rotate(180deg);
    }
    
    .generator-content {
      padding: 0 16px 16px;
      animation: slideDown 0.3s ease-out;
    }
    
    @keyframes slideDown {
      from { opacity: 0; transform: translateY(-10px); }
      to { opacity: 1; transform: translateY(0); }
    }
    
    .options {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 8px;
      margin-bottom: 16px;
    }
    
    .option {
      display: flex;
      align-items: center;
      gap: 8px;
      font-size: 0.8rem;
      color: var(--text-secondary);
      cursor: pointer;
    }
    
    .option input[type="checkbox"] {
      accent-color: var(--aurora-purple);
    }
    
    .length-slider {
      margin-bottom: 16px;
    }
    
    .length-slider label {
      display: block;
      font-size: 0.8rem;
      color: var(--text-secondary);
      margin-bottom: 8px;
    }
    
    .length-slider input[type="range"] {
      width: 100%;
      accent-color: var(--aurora-purple);
    }
    
    .preview-box {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 12px;
      background: rgba(0, 0, 0, 0.3);
      border-radius: 8px;
      margin-bottom: 12px;
    }
    
    .password-preview {
      flex: 1;
      font-family: 'SF Mono', 'Monaco', monospace;
      font-size: 0.9rem;
      color: var(--aurora-cyan);
      word-break: break-all;
      user-select: all;
    }
    
    .copy-btn {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 36px;
      height: 36px;
      background: transparent;
      border: 1px solid var(--glass-border);
      border-radius: 8px;
      color: var(--text-secondary);
      cursor: pointer;
      transition: all 0.2s ease;
      flex-shrink: 0;
    }
    
    .copy-btn:hover {
      border-color: var(--aurora-purple);
      color: var(--aurora-purple);
    }
    
    .copy-btn.copied {
      border-color: var(--energy-green);
      color: var(--energy-green);
    }
    
    .copy-btn svg {
      width: 18px;
      height: 18px;
    }
    
    .use-btn {
      width: 100%;
      padding: 12px;
      background: linear-gradient(135deg, var(--aurora-purple), var(--aurora-blue));
      border: none;
      border-radius: 8px;
      color: white;
      font-weight: 600;
      font-size: 0.9rem;
      cursor: pointer;
      transition: all 0.2s ease;
    }
    
    .use-btn:hover {
      transform: translateY(-1px);
      box-shadow: 0 4px 12px rgba(139, 92, 246, 0.3);
    }
  `]
})
export class PasswordGeneratorComponent {
  @Output() passwordGenerated = new EventEmitter<string>();
  
  readonly isExpanded = signal(false);
  readonly generatedPassword = signal('');
  readonly copied = signal(false);
  
  includeUppercase = true;
  includeLowercase = true;
  includeNumbers = true;
  includeSpecial = true;
  passwordLength = 16;
  
  toggle(): void {
    this.isExpanded.update(v => !v);
    if (this.isExpanded() && !this.generatedPassword()) {
      this.generatePassword();
    }
  }
  
  generatePassword(): void {
    let chars = '';
    if (this.includeUppercase) chars += 'ABCDEFGHIJKLMNOPQRSTUVWXYZ';
    if (this.includeLowercase) chars += 'abcdefghijklmnopqrstuvwxyz';
    if (this.includeNumbers) chars += '0123456789';
    if (this.includeSpecial) chars += '!@#$%^&*()_+-=[]{}|;:,.<>?';
    
    if (!chars) {
      this.includeLowercase = true;
      chars = 'abcdefghijklmnopqrstuvwxyz';
    }
    
    let password = '';
    const array = new Uint32Array(this.passwordLength);
    crypto.getRandomValues(array);
    
    for (let i = 0; i < this.passwordLength; i++) {
      password += chars[array[i] % chars.length];
    }
    
    this.generatedPassword.set(password);
    this.copied.set(false);
  }
  
  async copyAndUse(): Promise<void> {
    const password = this.generatedPassword();
    
    try {
      await navigator.clipboard.writeText(password);
      this.copied.set(true);
      setTimeout(() => this.copied.set(false), 2000);
    } catch {
      // Fallback
    }
    
    this.passwordGenerated.emit(password);
    this.isExpanded.set(false);
  }
}
