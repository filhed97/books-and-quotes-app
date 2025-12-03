import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BooksService, Book } from '../../services/books';
import { FormsModule } from '@angular/forms';

@Component({
  standalone: true,
  selector: 'app-books',
  imports: [CommonModule, FormsModule],
  templateUrl: './books.html',
  styleUrls: ['./books.css']
})
export class BooksPage implements OnInit {

  private booksService = inject(BooksService);

  books: Book[] = [];
  loading = true;
  error = '';
  newTitle = '';
  newAuthor = '';

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

  addBook() {
    if (!this.newTitle.trim()) return;

    this.booksService.addBook({
      title: this.newTitle,
      author: this.newAuthor
    }).subscribe({
      next: (created) => {
        this.books.push(created);
        this.newTitle = '';
        this.newAuthor = '';
      },
      error: () => {
        this.error = 'Failed to add book.';
      }
    });
  }

  deleteBook(id: string) {
    this.booksService.deleteBook(id).subscribe({
      next: () => {
        this.books = this.books.filter(b => b.id !== id);
      },
      error: () => {
        this.error = 'Failed to delete book.';
      }
    });
  }
}
