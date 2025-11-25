import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Test } from './services/test';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('app');

  message = signal<string>('Loading...');

  constructor(private api: Test) {
    this.api.getMessage().subscribe({
      next: (text) => this.message.set(text),
      error: () => this.message.set('Error calling API')
    });
  }
}
