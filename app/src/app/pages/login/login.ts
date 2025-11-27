import { Component } from '@angular/core';
import { Auth } from '../../services/auth';

@Component({
  selector: 'app-login',
  imports: [],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  email = '';
  password = '';

  constructor(private auth: Auth) {}

  submit() {
    this.auth.login({ email: this.email, password: this.password }).subscribe({
      next: (res) => console.log('Logged in:', res),
      error: (err) => console.error(err),
    });
  }
}
