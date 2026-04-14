import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { Editor } from '../../../models/editor/editor/editor';

@Component({
  selector: 'app-editors-grid',
  standalone: true,
  imports: [CommonModule, PaginationComponent],
  templateUrl: './editors-grid.component.html',
  styleUrl: './editors-grid.component.scss'
})
export class EditorsGridComponent {
  //@Input ricevo dal padre (page componet)
  @Input() isAdmin = false;
  @Input() editors: Editor[] = [];
  @Input() isAuthenticated = false;
  @Input() currentPage = 1;
  @Input() totalPages = 0;  
  @Input() totalRecords = 0;
  
  //@Output do al padre, chiamata a nextPage
  @Output() nextPage = new EventEmitter<void>();
  @Output() prevPage = new EventEmitter<void>();
  @Output() viewDetail = new EventEmitter<number>();

 // onNextPage va a chiamare nextPage (del padre pageComponent)
  onNextPage(): void {
    this.nextPage.emit();
  }

  onPrevPage(): void {
    this.prevPage.emit();
  }  
  // La paginazione procede dal bottone nel pagination-component figlio delle grid scaturisce l'event che
  // passa dal grid, al page fino alla classe PaginationState andando poi a cambiando i valori (come currentPages).
  // Questi valori poi a loro volta sono trasmessi all'inverso ossia dalla PaginationState nelle page fino al pagination component 
  // nelle grid
  onViewDetail(editorId: number): void { this.viewDetail.emit(editorId); } 

}
