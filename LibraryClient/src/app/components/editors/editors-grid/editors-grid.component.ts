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
  
  @Input() isAdmin = false;
  @Input() editors: Editor[] = [];
  @Input() isAuthenticated = false;
  @Input() currentPage = 1;
  @Input() totalPages = 0;  
  @Input() totalRecords = 0;
  
  //emitter per il padre, chiamata a nextPage
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

  onViewDetail(editorId: number): void { this.viewDetail.emit(editorId); } 

}
