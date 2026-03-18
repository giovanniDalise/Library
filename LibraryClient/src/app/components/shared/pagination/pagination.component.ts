import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './pagination.component.html',
  styleUrl: './pagination.component.scss'
})

export class PaginationComponent {

  @Input() currentPage = 1;
  @Input() totalPages = 0;
  @Input() totalRecords = 0;


  @Output() next = new EventEmitter<void>();
  @Output() prev = new EventEmitter<void>();

  nextPage(): void {
    this.next.emit();
  }

  prevPage(): void {
    this.prev.emit();
  }

}