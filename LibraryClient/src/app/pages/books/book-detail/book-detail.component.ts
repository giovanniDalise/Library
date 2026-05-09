import { Component } from '@angular/core';
import { BookDetail } from '../../../models/book/book-detail/book-detail';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { BookService } from '../../../services/book.service';

@Component({
  selector: 'app-book-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './book-detail.component.html',
  styleUrl: './book-detail.component.scss'
})
export class BookDetailComponent {

  book: BookDetail | null = null;
  isLoading = true;
  pagination = new PaginationState();
  bookId!: number;
  errorMessage = '';


  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService
  ) {} 
  
  ngOnInit(): void {
    this.bookId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDetail();
  }  

  loadDetail():void{
    this.isLoading = true;
    this.bookService.getBookDetail(this.bookId).subscribe(
       {
        next:(book)=>{
          this.book = book;
          this.isLoading = false;
        },
          error: () => {
          this.errorMessage = 'Error loading editor.';
          this.isLoading = false;
        }
       }
    )
  }

  onEdit(): void {
    this.router.navigate(['/books', this.book?.id, 'edit']);
  }

  onBack(): void {
    this.router.navigate(['/books']);
  }  
}
