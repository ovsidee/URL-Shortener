import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UrlDetails } from './url-details';

describe('UrlDetails', () => {
  let component: UrlDetails;
  let fixture: ComponentFixture<UrlDetails>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [UrlDetails]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UrlDetails);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
