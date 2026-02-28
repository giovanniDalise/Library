import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorsPageComponent } from './editors-page.component';

describe('EditorsPageComponent', () => {
  let component: EditorsPageComponent;
  let fixture: ComponentFixture<EditorsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorsPageComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditorsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
