import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { UrlService } from './url'; // Import the correct class name

describe('UrlService', () => {
  let service: UrlService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [UrlService]
    });
    service = TestBed.inject(UrlService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
