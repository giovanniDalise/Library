import { Component, OnInit } from '@angular/core';
import { AuthorsGridComponent } from '../../../components/authors/authors-grid/authors-grid.component';
import { AuthorsFiltersComponent } from '../../../components/authors/authors-filters/authors-filters.component';
import { RouterLink } from '@angular/router';
import { Author } from '../../../models/author/author';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { AuthorRequest } from '../../../models/author/author-request';
import { AuthorsService } from '../../../services/authors.service';
import { UserRoleService } from '../../../services/user-role.service';
import { Router } from 'express';
import { error } from 'console';

@Component({
  selector: 'app-authors-page',
  standalone: true,
  imports: [AuthorsGridComponent, AuthorsFiltersComponent, RouterLink],
  templateUrl: './authors-page.component.html',
  styleUrl: './authors-page.component.scss'
})
export class AuthorsPageComponent implements OnInit{

  authors: Author [] = [];
  isAdmin = false;
  isAuthenticated = false;

  pagination = new PaginationState();

  private lastSearchFilter: AuthorRequest = {};

  constructor(
    private authorService: AuthorsService,
    private userRoleService: UserRoleService,
    private router: Router
  ){};

  ngOnInit(): void {
    this.isAdmin = this.userRoleService.isAdmin();
    this.isAuthenticated = this.userRoleService.isAuthenticated();
    this.searchAuthors();
  }

  searchAuthors(searchFilter: AuthorRequest = this.lastSearchFilter): void{
    this.lastSearchFilter = searchFilter;
    const normalizedFilter: AuthorRequest = {
      id: searchFilter.id ?? undefined,
      name: searchFilter.name?.trim() ?? undefined,
      surname: searchFilter.name?.trim() ?? undefined
    }
    this.authorService.getAuthors(normalizedFilter, this.pagination.currentPage, this.pagination.pageSize).subscribe(
      {
        next:result =>{
          this.authors = result.items,
          this.pagination.totalRecords = result.totalRecords
        },
        error:error =>{
          console.error("Search error:", error);
          this.authors = [];
          this.pagination.totalRecords = 0;
        }

      }
    )
  }
}
