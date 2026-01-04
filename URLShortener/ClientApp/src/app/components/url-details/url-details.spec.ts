import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { RouterTestingModule } from '@angular/router/testing'; /

// FIX: Import the correct class name 'UrlDetailsComponent'
import { UrlDetailsComponent } from './url-details';

describe('UrlDetailsComponent', () => {
  let component: UrlDetailsComponent;
  let fixture: ComponentFixture<UrlDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      // Import fake modules for dependencies
      imports: [HttpClientTestingModule, RouterTestingModule],
      // FIX: Declare the correct component name
      declarations: [UrlDetailsComponent]
    })
      .compileComponents();

    fixture = TestBed.createComponent(UrlDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
