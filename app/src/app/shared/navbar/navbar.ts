import { Component, inject, OnInit, NgZone } from '@angular/core';
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
  public themeService = inject(ThemeService);
  private zone = inject(NgZone);

  isLoggedIn = false;

  async ngOnInit() {
    this.auth.loggedIn$.subscribe(state => {
      this.zone.run(() => {
        this.isLoggedIn = state;
      });
    });
  }

  goToLogin() {
    this.router.navigate(['/login']);
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }

  async logout() {
    try {
      await lastValueFrom(this.auth.logout());
    } catch (err) {
      console.error('Logout failed', err);
    }

    this.auth.setLoggedIn(false);
  }

  toggleTheme() {
    this.themeService.toggleTheme();
  }
}
