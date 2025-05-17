import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EmployeeAccessLogsComponent } from './employee-access-logs.component';

describe('EmployeeAccessLogsComponent', () => {
  let component: EmployeeAccessLogsComponent;
  let fixture: ComponentFixture<EmployeeAccessLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmployeeAccessLogsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(EmployeeAccessLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
