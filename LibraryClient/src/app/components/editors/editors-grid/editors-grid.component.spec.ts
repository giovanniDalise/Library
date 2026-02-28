import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorsGridComponent } from './editors-grid.component';

describe('EditorsGridComponent', () => {
  let component: EditorsGridComponent;
  let fixture: ComponentFixture<EditorsGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorsGridComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditorsGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
