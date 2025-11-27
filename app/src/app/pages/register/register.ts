import { Component } from '@angular/core';
import { Auth } from '../../services/auth';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {
  username = '';
  password = '';

  constructor(private auth: Auth) {}

  submit() {
    this.auth.register(this.username, this.password).subscribe(res => {
      console.log('Register response:', res);
    });
  }
}
