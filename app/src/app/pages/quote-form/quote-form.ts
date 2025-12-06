import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import {
  QuotesService,
  Quote,
  QuoteCreateRequest,
  QuoteUpdateRequest,
} from '../../services/quotes';
import { sanitizeText, isSanitizedNonEmpty } from '../../validators/input-sanitizer';

@Component({
  standalone: true,
  selector: 'app-quote-form',
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './quote-form.html',
  styleUrls: ['./quote-form.css'],
})
export class QuoteFormPage implements OnInit {
  private quotesService = inject(QuotesService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  quoteData: Partial<Quote> = {};
  loading = true;
  error = '';
  isEdit = false;

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.quotesService.getQuote(id).subscribe({
        next: (data) => {
          this.quoteData = data;
          this.loading = false;
        },
        error: () => {
          this.error = 'Failed to load quote.';
          this.loading = false;
        },
      });
    } else {
      this.loading = false;
    }
  }

  submit() {
    // Validate + sanitize quote text in one step
    const sanitizedQuote = isSanitizedNonEmpty(this.quoteData.quote);
    if (!sanitizedQuote) {
      this.error = 'Quote text is required.';
      return;
    }

    // Sanitize author (optional field)
    const sanitizedAuthor = sanitizeText(this.quoteData.author ?? '');

    if (this.isEdit && this.quoteData.id) {
      const updateRequest: QuoteUpdateRequest = {
        quote: sanitizedQuote,
        author: sanitizedAuthor,
      };

      this.quotesService.updateQuote(this.quoteData.id, updateRequest).subscribe({
        next: () => this.router.navigate(['/quotes']),
        error: () => (this.error = 'Failed to update quote.'),
      });
    } else {
      const createRequest: QuoteCreateRequest = {
        quote: sanitizedQuote,
        author: sanitizedAuthor,
      };

      this.quotesService.addQuote(createRequest).subscribe({
        next: () => this.router.navigate(['/quotes']),
        error: () => (this.error = 'Failed to add quote.'),
      });
    }
  }

  cancel() {
    this.router.navigate(['/quotes']);
  }
}
