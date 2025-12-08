import { Component, inject, OnInit, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { BooksService, Book, BookCreateRequest, BookUpdateRequest } from '../../services/books';
import { sanitizeText, isSanitizedNonEmpty } from '../../validators/input-sanitizer';
import { Auth } from '../../services/auth';

@Component({
  standalone: true,
  selector: 'app-book-form',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './book-form.html',
  styleUrls: ['./book-form.css'],
})
export class BookFormPage implements OnInit {
  private booksService = inject(BooksService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private auth = inject(Auth);
  private zone = inject(NgZone);

  book: Partial<Book> = {};
  loading = true;
  error = '';
  isEdit = false;

  ngOnInit() {
    // Reactive redirect on logout
    this.auth.loggedIn$.subscribe((loggedIn) => {
      this.zone.run(() => {
        if (!loggedIn) {
          this.router.navigate(['/books']);
        }
      });
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.booksService.getBook(id).subscribe({
        next: (data) => {
          this.book = data;
          this.loading = false;
        },
        error: () => {
          this.error = 'Failed to load book.';
          this.loading = false;
        },
      });
    } else {
      this.loading = false;
    }
  }

  submit() {
    const sanitizedTitle = isSanitizedNonEmpty(this.book.title);
    if (!sanitizedTitle) {
      this.error = 'Title is required.';
      return;
    }

    const sanitizedAuthor = sanitizeText(this.book.author ?? '');

    if (this.isEdit && this.book.id) {
      const updateRequest: BookUpdateRequest = {
        title: sanitizedTitle,
        author: sanitizedAuthor,
        published: this.book.published,
      };

      this.booksService.updateBook(this.book.id, updateRequest).subscribe({
        next: () => this.router.navigate(['/books']),
        error: () => (this.error = 'Failed to update book.'),
      });
    } else {
      const createRequest: BookCreateRequest = {
        title: sanitizedTitle,
        author: sanitizedAuthor,
        published: this.book.published,
      };

      this.booksService.addBook(createRequest).subscribe({
        next: () => this.router.navigate(['/books']),
        error: () => (this.error = 'Failed to add book.'),
      });
    }
  }

  cancel() {
    this.router.navigate(['/books']);
  }
}
