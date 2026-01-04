import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing'; // Needed for Service
import { FormsModule } from '@angular/forms'; // Needed for ngModel

// FIX: Import 'UrlTableComponent', not 'UrlTable'
import { UrlTableComponent } from './url-table';

describe('UrlTableComponent', () => {
  let component: UrlTableComponent;
  let fixture: ComponentFixture<UrlTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      // Import modules that the component uses
      imports: [HttpClientTestingModule, FormsModule],
      // Declare the component we are testing
      declarations: [UrlTableComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UrlTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
