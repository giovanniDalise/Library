import { CommonModule } from '@angular/common';
import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Book } from '../../../models/book/book/book';

@Component({
  selector: 'app-books-grid',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './books-grid.component.html',
  styleUrls: ['./books-grid.component.scss']
})
export class BooksGridComponent {

  @Input() books: Book[] = [];
  @Input() isAdmin = false;

  @Output() viewDetail = new EventEmitter<number>();

  onViewDetail(bookId:number):void{
    this.viewDetail.emit(bookId);
  }
}
