import { Component } from '@angular/core';
import { Auth } from '../../services/auth';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  email = '';
  password = '';

  constructor(private auth: Auth) {}

  submit() {
    this.auth.register({ email: this.email, password: this.password }).subscribe({
      next: (res) => console.log('Registered:', res),
      error: (err) => console.error(err),
    });
  }
}
