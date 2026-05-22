import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { FormControl, FormGroup } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Book } from '../../../models/book/book/book';
import { BookRequest } from '../../../models/book/book/book-request';

@Component({
  selector: 'app-books-filters',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './books-filters.component.html',
  styleUrl: './books-filters.component.scss'
})
export class BooksFiltersComponent {

@Input() isAdmin = false;
@Output() search = new EventEmitter<BookRequest>();

  filterForm = new FormGroup({
    id: new FormControl(''),
    title: new FormControl(''),
    isbn: new FormControl(''),
    author: new FormControl(''),
    editor: new FormControl('')
  });

  onSearch() {
    const formValue = this.filterForm.value;

    const authorValue = formValue.author?.trim() || undefined;

    const criteria: BookRequest = {
      id: this.isAdmin && formValue.id ? Number(formValue.id) : undefined,
      title: formValue.title?.trim() || undefined,
      isbn: formValue.isbn?.trim() || undefined,
      editor: formValue.editor?.trim()
        ? { name: formValue.editor.trim() }
        : undefined,
      authors: authorValue
        ? [{ name: authorValue, surname: authorValue }] // ← stesso valore in entrambi
        : undefined,
    };
    this.search.emit(criteria); // ← mancava questo
  }
}
