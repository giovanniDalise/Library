import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BooksGridComponent } from '../../../components/books/books-grid/books-grid.component';
import { UserRoleService } from '../../../services/user-role.service';
import { BooksFiltersComponent } from '../../../components/books/books-filters/books-filters.component';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { BookRequest } from '../../../models/book/book/book-request';
import { Book } from '../../../models/book/book/book';
import { BookService } from '../../../services/book.service';

@Component({
  selector: 'app-books-page',
  standalone: true,
  imports: [CommonModule, RouterLink, BooksGridComponent, BooksFiltersComponent],
  templateUrl: './books-page.component.html',
  styleUrl: './books-page.component.scss'
})
export class BooksPageComponent implements OnInit {

  books: Book[] = [];

  isAdmin = false;
  isAuthenticated = false;

  pagination = new PaginationState();


  bookId?: number;
  private lastCriteria: BookRequest = {};

  constructor(
    private bookService: BookService,
    private snackBar: MatSnackBar,
    private userRoleService: UserRoleService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.isAuthenticated = this.userRoleService.isAuthenticated();
    this.isAdmin = this.userRoleService.isAdmin();

    this.searchBook(); 
  }

  /* ===================== ACTIONS ===================== */

  deleteBook(bookId: number): void {
    this.bookService.deleteBook(bookId).subscribe(() => {
      this.books = this.books.filter(b => b.id !== bookId);
      this.snackBar.open('Libro eliminato con successo!', 'Chiudi', {
        duration: 3000,
        verticalPosition: 'top',
        horizontalPosition: 'center'
      });
    });
  }

  /* ===================== PAGINATION ===================== */

  nextPage(): void {
    this.pagination.next();
    this.searchBook(this.lastCriteria);
  }

  prevPage(): void {
    this.pagination.prev();
    this.searchBook(this.lastCriteria);
  } 

  searchBook(criteria: BookRequest = this.lastCriteria): void {

    this.lastCriteria = criteria;

    const searchCriteria: BookRequest = {
      id: criteria.id ?? undefined,
      title: criteria.title?.trim() || undefined,
      isbn: criteria.isbn?.trim() || undefined,
      editor: criteria.editor
        ? {
            editorId: criteria.editor.editorId ?? undefined,
            name: criteria.editor.name?.trim() || undefined
          }
        : undefined,
      authors: criteria.authors?.length
        ? criteria.authors.map(a => ({
            authorId: a.id ?? undefined,
            name: a.name?.trim() || undefined,
            surname: a.surname?.trim() || undefined
          }))
        : undefined
    };

    this.bookService
      .getBooks(searchCriteria, this.pagination.currentPage, this.pagination.pageSize)
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
}
