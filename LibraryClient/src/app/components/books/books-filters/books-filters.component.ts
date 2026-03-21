import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Book } from '../../../models/book/book';
import { ReactiveFormsModule } from '@angular/forms';
import { FormControl, FormGroup } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-books-filters',
  standalone: true,
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './books-filters.component.html',
  styleUrl: './books-filters.component.scss'
})
export class BooksFiltersComponent {

@Input() isAdmin = false;
@Output() search = new EventEmitter<Partial<Book>>();

  filterForm = new FormGroup({
    id: new FormControl(''),
    title: new FormControl(''),
    isbn: new FormControl(''),
    author: new FormControl(''),
    editor: new FormControl('')
  });

onSearch() {
  const formValue = this.filterForm.value;
  const criteria: Partial<Book> = {
    id: this.isAdmin && formValue.id? Number(formValue.id): undefined,
    title: formValue.title?.trim() || undefined,
    isbn: formValue.isbn?.trim() || undefined,
    authors: formValue.author ? [{ name: formValue.author.trim(), surname: '' } as any] : [],
    editor: formValue.editor ? { name: formValue.editor.trim() } as any : undefined,
  };

  console.log('DEBUG Payload inviato:', criteria);

  this.search.emit(criteria);
}


}
