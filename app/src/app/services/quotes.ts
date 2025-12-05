import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface Quote {
  id: string;
  userId: string;
  quote: string;
  author?: string;
}

export interface QuoteCreateRequest {
  quote: string;
  author?: string;
}

export interface QuoteUpdateRequest {
  quote?: string;
  author?: string;
}

@Injectable({
  providedIn: 'root',
})
export class QuotesService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/quotes`;

  getQuotes(): Observable<Quote[]> {
    return this.http.get<Quote[]>(this.apiUrl, { withCredentials: true });
  }

  getQuote(id: string): Observable<Quote> {
    return this.http.get<Quote>(`${this.apiUrl}/${id}`, { withCredentials: true });
  }

  addQuote(quote: QuoteCreateRequest): Observable<Quote> {
    return this.http.post<Quote>(this.apiUrl, quote, { withCredentials: true });
  }

  updateQuote(id: string, quote: QuoteUpdateRequest): Observable<Quote> {
    return this.http.put<Quote>(`${this.apiUrl}/${id}`, quote, { withCredentials: true });
  }

  deleteQuote(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`, { withCredentials: true });
  }
}
