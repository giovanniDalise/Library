import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Book } from '../../../models/book/book';

@Component({
  selector: 'app-books-grid',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './books-grid.component.html',
  styleUrls: ['./books-grid.component.scss']
})
export class BooksGridComponent {

  @Input() books: Book[] = [];
  @Input() isAdmin = false;
  @Input() isAuthenticated = false;
  @Input() currentPage = 1;
  @Input() totalPages = 0;

  @Output() delete = new EventEmitter<number>();
  @Output() nextPage = new EventEmitter<void>();
  @Output() prevPage = new EventEmitter<void>();

  onDelete(bookId: number): void {
    this.delete.emit(bookId);
  }

  onNextPage(): void {
    this.nextPage.emit();
  }

  onPrevPage(): void {
    this.prevPage.emit();
  }
}
