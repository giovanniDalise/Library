import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PaginationComponent } from '../../shared/pagination/pagination.component';
import { Author } from '../../../models/author/author/author';

@Component({
  selector: 'app-authors-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './authors-grid.component.html',
  styleUrl: './authors-grid.component.scss'
})
export class AuthorsGridComponent {
  @Input() isAdmin = false;
  @Input() authors : Author[] = [];

  @Output() viewDetail = new EventEmitter<number>();

  onViewDetail(authorId: number): void { this.viewDetail.emit(authorId); } 

}
