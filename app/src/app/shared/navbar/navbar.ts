import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.html',
  styleUrls: ['./navbar.css']
})
export class Navbar implements OnInit {

  isLoggedIn = false;

  constructor(
    private auth: Auth,
    private router: Router
  ) {}

  async ngOnInit() {
    this.isLoggedIn = await this.auth.check();
  }

  async logout() {
    /* try {
      await this.auth.logout(); // Make sure you have a logout endpoint in Auth service
    } catch (err) {
      console.error('Logout failed', err);
    } */
    this.isLoggedIn = false;
    this.router.navigate(['/login']);
  }
}
