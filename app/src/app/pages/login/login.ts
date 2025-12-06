import { Component } from '@angular/core';
import { Auth } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { isSanitizedNonEmpty } from '../../validators/input-sanitizer';

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
  error = '';

  constructor(private auth: Auth, private router: Router) {}

  submit() {
    // Sanitize username (required)
    const sanitizedUsername = isSanitizedNonEmpty(this.username);
    if (!sanitizedUsername) {
      this.error = 'Username is required.';
      return;
    }

    // Sanitize password (required)
    const sanitizedPassword = isSanitizedNonEmpty(this.password);
    if (!sanitizedPassword) {
      this.error = 'Password is required.';
      return;
    }

    this.auth.login(sanitizedUsername, sanitizedPassword).subscribe({
      next: (res) => {
        console.log('Login response:', res);

        this.auth.setLoggedIn(true);
        this.router.navigate(['/']);
      },
      error: (err) => {
        console.error('Login failed', err);
        this.error = 'Invalid username or password.';
      },
    });
  }
}
