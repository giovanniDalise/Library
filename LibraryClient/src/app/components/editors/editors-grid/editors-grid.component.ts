import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Editor } from '../../../models/editor/editor';
import { PaginationComponent } from '../../shared/pagination/pagination.component';

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

  @Output() nextPage = new EventEmitter<void>();
  @Output() prevPage = new EventEmitter<void>();

  onNextPage(): void {
    this.nextPage.emit();
  }

  onPrevPage(): void {
    this.prevPage.emit();
  }  
}
