import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BooksGridComponent } from '../../../components/books/books-grid/books-grid.component';
import { BooksService } from '../../../services/books.service';
import { AuthenticationService } from '../../../services/authentication.service';
import { UserRoleService } from '../../../services/user-role.service';
import { Book } from '../../../models/book/book';
import { BooksFiltersComponent } from '../../../components/books/books-filters/books-filters.component';
import { HttpHeaders } from '@angular/common/http';
import { Author } from '../../../models/author';

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

  pageSize = 10;
  currentPage = 1;
  totalRecords = 0;

  bookId?: number;
  private lastCriteria: Partial<Book> = {};

  constructor(
    private booksService: BooksService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private authService: AuthenticationService,
    private userRoleService: UserRoleService
  ) {}

  ngOnInit(): void {
    this.isAuthenticated = this.userRoleService.isAuthenticated();
    this.isAdmin = this.userRoleService.isAdmin();

    this.searchBook(); // carica tutti i libri all’avvio
  }

  /* ===================== ACTIONS ===================== */

  deleteBook(bookId: number): void {
    this.booksService.deleteBook(bookId).subscribe(() => {
      this.books = this.books.filter(b => b.bookId !== bookId);
      this.snackBar.open('Libro eliminato con successo!', 'Chiudi', {
        duration: 3000,
        verticalPosition: 'top',
        horizontalPosition: 'center'
      });
    });
  }

  /* ===================== PAGINATION ===================== */

  get totalPages(): number {
    return Math.ceil(this.totalRecords / this.pageSize);
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.searchBook(this.lastCriteria);
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.searchBook(this.lastCriteria);; 
    }
  }

  searchBook(criteria: Partial<Book> = this.lastCriteria): void {
    this.lastCriteria = criteria; 
    const searchCriteria: any = {
      BookId: criteria.bookId || 0,
      Title: criteria.title?.trim() || null,
      Isbn: criteria.isbn?.trim() || null,
      Authors: criteria.authors?.length
        ? criteria.authors.map(a => ({
            AuthorId: a.authorId || 0,
            Name: a.name || null,
            Surname: a.surname || null
          }))
        : [],
      Editor: criteria.editor
        ? { EditorId: criteria.editor.editorId || 0, Name: criteria.editor.name || null }
        : null
    };

    this.booksService
      .getBooks(searchCriteria, this.currentPage, this.pageSize)
      .subscribe({
        next: results => {
          this.books = results.bookResponse;
          this.totalRecords = results.totalRecords;
        },
        error: error => {
          console.error('Errore nella ricerca:', error);
          this.books = [];
          this.totalRecords = 0;sda
        }
      });
  }
}
