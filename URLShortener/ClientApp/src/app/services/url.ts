import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ShortUrl {
  id: number;
  originalURL: string;
  shortCode: string;
  createdBy: string;
  createdDate: Date;
}

// 1. Define the shape of the User Info
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

  // 2. Add this method to get current user details
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
}
