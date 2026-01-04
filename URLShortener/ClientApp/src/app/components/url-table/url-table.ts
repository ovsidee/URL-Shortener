import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { UrlService, ShortUrl, UserInfo } from '../../services/url';

@Component({
  standalone: false,
  selector: 'app-url-table',
  templateUrl: './url-table.html',
  styleUrls: ['./url-table.css']
})
export class UrlTableComponent implements OnInit {
  urls: ShortUrl[] = [];
  newUrl: string = '';
  errorMessage: string = '';

  // default state for user is "Anonymous"
  currentUser: UserInfo = { isAuthenticated: false, username: '', isAdmin: false };

  constructor(private urlService: UrlService, private cdr: ChangeDetectorRef) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    // load user Info first and then load the table
    this.urlService.getUserInfo().subscribe({
      next: (user) => {
        this.currentUser = user;
        this.loadUrls(); // load the table
      },
      error: () => this.loadUrls() // if auth check fails
    });
  }

  loadUrls() {
    this.urlService.getUrls().subscribe({
      next: (data) => {
        this.urls = data;
        this.cdr.detectChanges();
      },
      error: (err) => console.error('Failed to load URLs', err)
    });
  }

  // helper function for the HTML to check permissions
  canDelete(url: ShortUrl): boolean {
    if (!this.currentUser.isAuthenticated) return false;
    if (this.currentUser.isAdmin) return true; // admin deletes everything
    return url.createdBy === this.currentUser.username; // user deletes owns
  }

  addUrl() {
    if (!this.newUrl) return;

    this.urlService.addUrl(this.newUrl).subscribe({
      next: (url) => {
        this.urls = [...this.urls, url];
        this.newUrl = '';
        this.errorMessage = '';
        this.cdr.detectChanges();
      },
      error: (err) => {
        if (err.status === 409) this.errorMessage = 'This URL already exists.';
        else if (err.status === 401) this.errorMessage = 'You must be logged in to add URLs.';
        else this.errorMessage = 'Error adding URL.';
        this.cdr.detectChanges();
      }
    });
  }

  deleteUrl(id: number) {
    if(!confirm("Are you sure?")) return;

    this.urlService.deleteUrl(id).subscribe({
      next: () => {
        this.urls = this.urls.filter(u => u.id !== id);
        this.cdr.detectChanges();
      },
      error: (err) => {
        alert("Error deleting URL.");
      }
    });
  }
}
