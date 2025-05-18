import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MOCKComponent } from './mock.component';

describe('MOCKComponent', () => {
  let component: MOCKComponent;
  let fixture: ComponentFixture<MOCKComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [MOCKComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MOCKComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
