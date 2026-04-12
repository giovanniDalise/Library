import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { Author } from '../../../models/author/author';

@Component({
  selector: 'app-authors-grid',
  standalone: true,
  imports: [CommonModule, PaginationComponent],
  templateUrl: './authors-grid.component.html',
  styleUrl: './authors-grid.component.scss'
})
export class AuthorsGridComponent {
  @Input() isAdmin = false;
  @Input() isAuthenticated = false;
  @Input() authors : Author[] = [];
  @Input() currentPage = 1;
  @Input() totalPages = 0;
  @Input() totalRecords = 0;

  @Output() nextPage = new EventEmitter<void>();
  @Output() prevPage = new EventEmitter<void>();
  @Output() viewDetail = new EventEmitter<number>();

  onNextPage():void{
    this.nextPage.emit()
  }

  onPrevPage():void{
    this.prevPage.emit()
  }

  onViewDetail(authorId: number): void { this.viewDetail.emit(authorId); } 

}
