import { Component } from '@angular/core';
import { Auth } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  username = '';
  password = '';

  constructor(private auth: Auth, private router: Router) {}

  submit() {
    this.auth.login(this.username, this.password).subscribe({
      next: (res) => {
        console.log('Login response:', res);

        // Set localStorage flag
        localStorage.setItem('isLoggedIn', 'true');

        // Navigate to home page
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Login failed', err);
      }
    });
  }
}
