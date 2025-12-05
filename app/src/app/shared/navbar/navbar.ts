import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { Auth } from '../../services/auth';
import { lastValueFrom } from 'rxjs';
import { ThemeService } from '../../services/theme';

@Component({
  standalone: true,
  selector: 'app-navbar',
  imports: [CommonModule, RouterModule],
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class Navbar implements OnInit {

  private auth = inject(Auth);
  private router = inject(Router);
  public themeService = inject(ThemeService); // Make public for template access

  isLoggedIn = false;

  async ngOnInit() {
    this.isLoggedIn = localStorage.getItem('isLoggedIn') === 'true';
  }

  async logout() {
    try {
      await lastValueFrom(this.auth.logout());
    } catch (err) {
      console.error('Logout failed', err);
    }

    localStorage.setItem('isLoggedIn', 'false');
    this.isLoggedIn = false;
    this.router.navigate(['/login']);
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
