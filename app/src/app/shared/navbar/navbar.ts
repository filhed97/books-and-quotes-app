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
    // Subscribe to reactive login state
    this.auth.loggedIn$.subscribe(state => {
      this.isLoggedIn = state;
    });
  }

  async logout() {
    try {
      await lastValueFrom(this.auth.logout());
    } catch (err) {
      console.error('Logout failed', err);
    }

    this.auth.setLoggedIn(false);
    this.router.navigate(['/login']);
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
