import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { Auth } from '../../services/auth';

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

  isLoggedIn = false;

  async ngOnInit() {
    this.isLoggedIn = await this.auth.check();
  }

  async logout() {
    /* try {
      await this.auth.logout();
    } catch (err) {
      console.error('Logout failed', err);
    } */
    this.isLoggedIn = false;
    this.router.navigate(['/login']);
  }
}
