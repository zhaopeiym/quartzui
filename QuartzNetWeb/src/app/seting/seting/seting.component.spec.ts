import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SetingComponent } from './seting.component';

describe('SetingComponent', () => {
  let component: SetingComponent;
  let fixture: ComponentFixture<SetingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SetingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SetingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
