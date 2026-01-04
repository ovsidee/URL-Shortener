import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';

import { UrlTableComponent } from './url-table';

describe('UrlTableComponent', () => {
  let component: UrlTableComponent;
  let fixture: ComponentFixture<UrlTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule],
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
