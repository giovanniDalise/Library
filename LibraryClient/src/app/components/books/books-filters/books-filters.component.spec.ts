import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BooksFiltersComponent } from './books-filters.component';


describe('BooksFiltersComponent', () => {
  let component: BooksFiltersComponent;
  let fixture: ComponentFixture<BooksFiltersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BooksFiltersComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(BooksFiltersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
