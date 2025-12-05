import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { BooksPage } from './pages/books/books';
import { BookFormPage } from './pages/book-form/book-form';
import { QuotesPage } from './pages/quotes/quotes';
import { QuoteFormPage } from './pages/quote-form/quote-form';

export const routes: Routes = [
  { path: 'books', component: BooksPage },
  { path: 'books/add', component: BookFormPage },
  { path: 'books/edit/:id', component: BookFormPage },
  { path: 'quotes', component: QuotesPage },
  { path: 'quotes/add', component: QuoteFormPage },
  { path: 'quotes/edit/:id', component: QuoteFormPage },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: '', redirectTo: '/books', pathMatch: 'full' },
  { path: '**', redirectTo: '/books' }
];
