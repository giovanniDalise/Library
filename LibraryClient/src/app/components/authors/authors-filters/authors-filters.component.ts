import { Component, Input, Output } from '@angular/core';
import { EventEmitter } from '@angular/core';
import { AuthorRequest } from '../../../models/author/author/author-request';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-authors-filters',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './authors-filters.component.html',
  styleUrl: './authors-filters.component.scss'
})
export class AuthorsFiltersComponent {
  @Input() isAdmin = false
  @Output() search = new EventEmitter<AuthorRequest>();

  filterForm = new FormGroup({
    authorId: new FormControl(""),
    authorFullName: new FormControl("")
  });

  onSearch(): void {
    const formValue = this.filterForm.value;
    const criteria: AuthorRequest = {
      id: this.isAdmin && formValue.authorId ? Number(formValue.authorId) : 0,
      name: formValue.authorFullName?.trim() ?? undefined,
      surname: undefined,
    };
    this.search.emit(criteria);
  }
}
