import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { lastValueFrom, Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class Auth {
  private baseUrl = `${environment.apiBaseUrl}`;

  constructor(private http: HttpClient) {}

  register(username: string, password: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/AuthRegister`, { username, password });
  }

  login(username: string, password: string): Observable<any> {
    return this.http.post(`${this.baseUrl}/AuthLogin`, { username, password });
  }

  // Quick check to see if auth credentials are currently valid
  async check(): Promise<boolean> {
    try {
      await lastValueFrom(
        this.http.get(`${this.baseUrl}/TestFunction`, {
          responseType: 'text',
          withCredentials: true
        })
      );
      return true; // 200 OK
    } catch (error) {
      return false; // Any error (401, 500, etc.)
    }
  }
}
