import { NgZone, Component } from '@angular/core';
import { Auth } from '../../services/auth';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { isSanitizedNonEmpty } from '../../validators/input-sanitizer';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  username = '';
  password = '';
  error = '';

  constructor(private auth: Auth, public router: Router, private zone: NgZone) {}

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

    this.auth.register(sanitizedUsername, sanitizedPassword).subscribe({
      next: (res) => {
        console.log('Register response:', res);
        this.router.navigate(['/login']); // optional — depends on your flow
      },
      error: (err) => {
        console.error('Registration failed', err);
        this.zone.run(() => {
          this.error = 'Invalid username or password.';
        });
      },
    });
  }
}
