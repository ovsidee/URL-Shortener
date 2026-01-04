import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ShortUrl {
  id: number;
  originalUrl: string;
  shortCode: string;
  shortUrl: string;
  createdBy: string;
  createdDate: Date;
}

// user info
export interface UserInfo {
  isAuthenticated: boolean;
  username: string;
  isAdmin: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class UrlService {
  private apiUrl = 'http://localhost:5152/api/UrlApi';

  constructor(private http: HttpClient) { }

  getUrls(): Observable<ShortUrl[]> {
    return this.http.get<ShortUrl[]>(this.apiUrl);
  }

  getUserInfo(): Observable<UserInfo> {
    return this.http.get<UserInfo>(`${this.apiUrl}/me`, {
      withCredentials: true
    });
  }

  addUrl(originalUrl: string): Observable<ShortUrl> {
    const payload = { url: originalUrl };
    return this.http.post<ShortUrl>(`${this.apiUrl}/add`, payload, {
      headers: { 'Content-Type': 'application/json' },
      withCredentials: true
    });
  }

  deleteUrl(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/${id}`, {
      withCredentials: true
    });
  }

  getUrlById(id: number): Observable<ShortUrl> {
    return this.http.get<ShortUrl>(`${this.apiUrl}/${id}`, {
      withCredentials: true
    });
  }
}
