import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { QuotesService, Quote } from '../../services/quotes';
import { RouterModule, Router } from '@angular/router';

@Component({
  standalone: true,
  selector: 'app-quotes',
  imports: [CommonModule, RouterModule],
  templateUrl: './quotes.html',
  styleUrls: ['./quotes.css'],
})
export class QuotesPage implements OnInit {
  private quotesService = inject(QuotesService);
  private router = inject(Router);

  quotes: Quote[] = [];
  loading = true;
  error = '';
  maxQuotes = 5;

  ngOnInit() {
    this.loadQuotes();
  }

  loadQuotes() {
    this.loading = true;
    this.error = '';
    this.quotesService.getQuotes().subscribe({
      next: (data) => {
        this.quotes = data;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load quotes.';
        this.loading = false;
      },
    });
  }

  deleteQuote(id: string) {
    if (!confirm('Are you sure you want to delete this quote?')) return;

    this.quotesService.deleteQuote(id).subscribe({
      next: () => this.loadQuotes(),
      error: () => (this.error = 'Failed to delete quote.'),
    });
  }

  goToAdd() {
    this.router.navigate(['/quotes/add']);
  }

  goToEdit(quote: Quote) {
    this.router.navigate(['/quotes/edit', quote.id]);
  }

  canAddQuote(): boolean {
    return this.quotes.length < this.maxQuotes;
  }
}
