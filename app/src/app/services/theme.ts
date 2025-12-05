import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private storageKey = 'app-theme'; // "light" | "dark"
  private currentTheme: 'light' | 'dark' = 'light';

  constructor() {
    // Load saved user preference
    const saved = localStorage.getItem(this.storageKey) as 'light' | 'dark' | null;
    this.currentTheme = saved ?? 'light';
    this.applyTheme(this.currentTheme);
  }

  get theme() {
    return this.currentTheme;
  }

  toggleTheme() {
    this.currentTheme = this.currentTheme === 'light' ? 'dark' : 'light';
    this.applyTheme(this.currentTheme);
    localStorage.setItem(this.storageKey, this.currentTheme);
  }

  private applyTheme(theme: 'light' | 'dark') {
    const html = document.documentElement;

    if (theme === 'dark') {
      html.classList.add('dark');
    } else {
      html.classList.remove('dark');
    }
  }
}
