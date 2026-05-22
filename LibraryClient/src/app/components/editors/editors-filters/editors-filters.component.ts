import { Component, EventEmitter, Input, Output } from '@angular/core';
import { EditorRequest } from '../../../models/editor/editor/editor-request';
import { Editor } from '../../../models/editor/editor/editor';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-editors-filters',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './editors-filters.component.html',
  styleUrl: './editors-filters.component.scss'
})
export class EditorsFiltersComponent {

  @Input() isAdmin = false;
  @Output() search = new EventEmitter<EditorRequest>();
  
  filterForm = new FormGroup({
    id: new FormControl(''),
    name: new FormControl('')
  });

  onSearch() {
    const formValue = this.filterForm.value;
    const criteria: EditorRequest = {
      id: this.isAdmin && formValue.id ? Number(formValue.id) : undefined,
      name: formValue.name?.trim() || undefined,
    };
    this.search.emit(criteria);
  } 
}
