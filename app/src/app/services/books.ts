import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Book {
  id: string;
  title: string;
  author: string;
  published: Date;
}

export interface BookCreateRequest {
  title: string;
  author?: string;
}

@Injectable({
  providedIn: 'root'
})
export class BooksService {
  private http = inject(HttpClient);

  private apiUrl = `${environment.apiBaseUrl}/books`;

  getBooks(): Observable<Book[]> {
    return this.http.get<Book[]>(this.apiUrl, {
      withCredentials: true
    });
  }

  addBook(book: BookCreateRequest): Observable<Book> {
    return this.http.post<Book>(this.apiUrl, book, {
      withCredentials: true
    });
  }

  deleteBook(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, {
      withCredentials: true
    });
  }
}
