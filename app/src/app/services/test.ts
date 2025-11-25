import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Test {
  private apiUrl = 'http://localhost:7071/api/TestFunction'; // Function URL

  constructor(private http: HttpClient) { }

  getMessage(): Observable<string> {
    return this.http.get(this.apiUrl, { responseType: 'text' });
  }
}
