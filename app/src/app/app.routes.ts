import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { BooksPage } from './pages/books/books';

export const routes: Routes = [
  { path: '', component: BooksPage },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
];
