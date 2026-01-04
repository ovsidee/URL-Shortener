import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { UrlService, ShortUrl } from '../../services/url';

@Component({
  standalone: false,
  selector: 'app-url-details',
  templateUrl: './url-details.html',
  styleUrls: ['./url-details.css']
})
export class UrlDetailsComponent implements OnInit {
  url: ShortUrl | null = null;
  isLoading = true;
  error = '';

  constructor(
    private route: ActivatedRoute,
    private urlService: UrlService,
    private router: Router,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    if (id) {
      this.urlService.getUrlById(id).subscribe({
        next: (data) => {
          this.url = data;
          this.isLoading = false;
          this.cdr.detectChanges(); // force update
        },
        error: (err) => {
          this.error = "Could not load details. You might not be authorized.";
          this.isLoading = false;
          this.cdr.detectChanges(); // force update
        }
      });
    }
  }

  goBack() {
    this.router.navigate(['/']);
  }
}
