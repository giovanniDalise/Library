import { Component, OnInit } from '@angular/core';
import { AuthorsGridComponent } from '../../../components/authors/authors-grid/authors-grid.component';
import { AuthorsFiltersComponent } from '../../../components/authors/authors-filters/authors-filters.component';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-authors-page',
  standalone: true,
  imports: [AuthorsGridComponent, AuthorsFiltersComponent, RouterLink],
  templateUrl: './authors-page.component.html',
  styleUrl: './authors-page.component.scss'
})
export class AuthorsPageComponent implements OnInit{

  ngOnInit(): void {
  }
}
