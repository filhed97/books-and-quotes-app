import { TestBed } from '@angular/core/testing';

import { Quotes } from './quotes';

describe('Quotes', () => {
  let service: Quotes;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(Quotes);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
