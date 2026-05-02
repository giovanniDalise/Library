import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthorsService } from '../../../services/authors.service';
import { CommonModule } from '@angular/common';
import { AuthorDetail } from '../../../models/author/author-detail/author-detail';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { PaginationComponent } from '../../../components/shared/pagination/pagination.component';

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
    private authorService: AuthorsService
  ) {}  


  onEdit(): void {
    this.router.navigate(['/editors', this.author?.id, 'edit']);
  }  
  onBack(): void {
    this.router.navigate(['/authors']);
  }  
}
