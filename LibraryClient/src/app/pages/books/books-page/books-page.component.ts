import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { BooksGridComponent } from '../../../components/books/books-grid/books-grid.component';
import { UserRoleService } from '../../../services/user-role.service';
import { BooksFiltersComponent } from '../../../components/books/books-filters/books-filters.component';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { BookRequest } from '../../../models/book/book/book-request';
import { Book } from '../../../models/book/book/book';
import { BookService } from '../../../services/book.service';
import { PaginationComponent } from '../../../components/shared/pagination/pagination.component';

@Component({
  selector: 'app-books-page',
  standalone: true,
  imports: [CommonModule, RouterLink, BooksGridComponent, BooksFiltersComponent, PaginationComponent],
  templateUrl: './books-page.component.html',
  styleUrl: './books-page.component.scss'
})
export class BooksPageComponent implements OnInit {

  books: Book[] = [];

  isAdmin = false;
  isAuthenticated = false;

  pagination = new PaginationState();


  bookId?: number;
  private lastSearchFilter: BookRequest = {};

  constructor(
    private bookService: BookService,
    private userRoleService: UserRoleService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.isAuthenticated = this.userRoleService.isAuthenticated();
    this.isAdmin = this.userRoleService.isAdmin();

    this.searchBook(); 
  }

  /* ===================== ACTIONS ===================== */

  searchBook(searchFilter: BookRequest = this.lastSearchFilter): void {
    this.lastSearchFilter = searchFilter;

    this.bookService
      .getBooks(searchFilter, this.pagination.currentPage, this.pagination.pageSize)
      .subscribe({
        next: results => {
          this.books = results.items;
          this.pagination.totalRecords = results.totalRecords;
        },
        error: error => {
          console.error('Errore nella ricerca:', error);
          this.books = [];
          this.pagination.totalRecords = 0;
        }
      });
  }

  viewDetail(bookId:number):void{
    this.router.navigate(["/books", bookId]);
  }

  /* ===================== PAGINATION ===================== */

  nextPage(): void {
    this.pagination.next();
    this.searchBook(this.lastSearchFilter);
  }

  prevPage(): void {
    this.pagination.prev();
    this.searchBook(this.lastSearchFilter);
  }

  goToAdd(): void {
    this.router.navigate(['/books/add']);
  }  
}
