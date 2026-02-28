import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AuthorsGridComponent } from './authors-grid.component';

describe('AuthorsGridComponent', () => {
  let component: AuthorsGridComponent;
  let fixture: ComponentFixture<AuthorsGridComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AuthorsGridComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(AuthorsGridComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
