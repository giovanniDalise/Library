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
  
  // potresti utilizzare anche dei semplici div con input, ma per tanti campi di input puoi avere un oggetto unico con il FormGroup
  // che ti porta a gestire meglio più campi, ti permette metodi di validazione e altri vantaggi
  filterForm = new FormGroup({
    editorId: new FormControl(''),
    name: new FormControl('')
  });

  onSearch() {
    const formValue = this.filterForm.value;
    const criteria: EditorRequest = {
      id: this.isAdmin && formValue.editorId ? Number(formValue.editorId) : undefined,
      // operatore ternario: se l'utente è admin e se editorId è valorizzato allora trasforma la stringa 
      // (form restituisce sempre stringhe) in number oppure id è undefined
      name: formValue.name?.trim() || undefined,
    };
    this.search.emit(criteria);
  } 
}
