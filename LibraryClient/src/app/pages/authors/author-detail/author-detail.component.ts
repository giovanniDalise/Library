import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthorDetail } from '../../../models/author/author-detail/author-detail';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { PaginationComponent } from '../../../components/shared/pagination/pagination.component';
import { AuthorService } from '../../../services/author.service';

@Component({
  selector: 'app-author-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, PaginationComponent],
  templateUrl: './author-detail.component.html',
  styleUrl: './author-detail.component.scss'
})
export class AuthorDetailComponent {

  author: AuthorDetail | null = null;
  isLoading = true;
  errorMessage = '';
  authorId!:number;
  pagination = new PaginationState();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authorService: AuthorService
  ) {} 
  
  ngOnInit(): void {
    this.authorId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDetail();
  }  

  loadDetail(): void {
    this.isLoading = true;
    this.authorService.getAuthorDetail(this.authorId, this.pagination.currentPage, this.pagination.pageSize)
      .subscribe({
        next: (author) => {
          this.author = author;
          this.pagination.totalRecords = author.books.totalRecords;
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Error loading author.';
          this.isLoading = false;
        }
      });
  }  

  onEdit(): void {
    this.router.navigate(['/editors', this.author?.id, 'edit']);
  }  
  onBack(): void {
    this.router.navigate(['/authors']);
  }  

  nextPage(): void {
    this.pagination.next();
    this.loadDetail();
  }

  prevPage(): void {
    this.pagination.prev();
    this.loadDetail();
  }  
}
