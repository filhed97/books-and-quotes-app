import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BooksService, Book } from '../../services/books';
import { RouterModule, Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-books',
  imports: [CommonModule, RouterModule],
  templateUrl: './books.html',
  styleUrls: ['./books.css']
})
export class BooksPage implements OnInit {

  private booksService = inject(BooksService);
  private router = inject(Router);

  books: Book[] = [];
  loading = true;
  error = '';

  ngOnInit() {
    this.loadBooks();
  }

  loadBooks() {
    this.loading = true;
    this.error = '';
    this.booksService.getBooks().subscribe({
      next: (data) => {
        this.books = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load books.';
        this.loading = false;
      }
    });
  }

  deleteBook(id: string) {
    if (!confirm('Are you sure you want to delete this book?')) return;

    this.booksService.deleteBook(id).subscribe({
      next: () => this.loadBooks(),
      error: () => this.error = 'Failed to delete book.'
    });
  }

  goToAdd() {
    this.router.navigate(['/books/add']);
  }

  goToEdit(book: Book) {
    this.router.navigate(['/books/edit', book.id]);
  }
}
