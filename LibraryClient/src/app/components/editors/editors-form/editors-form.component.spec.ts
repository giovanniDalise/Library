import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorsFormComponent } from './editors-form.component';

describe('EditorsFormComponent', () => {
  let component: EditorsFormComponent;
  let fixture: ComponentFixture<EditorsFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorsFormComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditorsFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
