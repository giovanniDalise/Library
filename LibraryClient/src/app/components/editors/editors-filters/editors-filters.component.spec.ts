import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditorsFiltersComponent } from './editors-filters.component';

describe('EditorsFiltersComponent', () => {
  let component: EditorsFiltersComponent;
  let fixture: ComponentFixture<EditorsFiltersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditorsFiltersComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EditorsFiltersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
