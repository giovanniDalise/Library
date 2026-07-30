import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthorDetail } from '../../../models/author/author-detail/author-detail';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { PaginationComponent } from '../../../components/shared/pagination/pagination.component';
import { AuthorService } from '../../../services/author.service';
import { UserRoleService } from '../../../services/user-role.service';
import { MatSnackBar } from '@angular/material/snack-bar';

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
  isAdmin = false;
  pagination = new PaginationState();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authorService: AuthorService,
    private userRoleService: UserRoleService,
    private snackBar: MatSnackBar
   
  ) {} 
  
  ngOnInit(): void {
    this.isAdmin = this.userRoleService.isAdmin();
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
    this.router.navigate(['/authors/edit', this.authorId]);
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

  onDelete(): void {
    const snackRef = this.snackBar.open(
      `Are you sure you want to delete "${this.author?.name} ${this.author?.surname}"?`,
      'Confirm',
      {
        duration: 5000,
        panelClass: ['snackbar-success']
      }
    );

    snackRef.onAction().subscribe(() => {
      this.authorService.deleteAuthor(this.authorId).subscribe({
        next: () => {
          this.snackBar.open('Author deleted successfully', 'OK', {
            duration: 6000,
            panelClass: ['snackbar-success']
          });
          this.router.navigate(['/authors']);
        },
        error: (err) => {
          const message = err.error?.error ?? 'Something went wrong while deleting the author';
          this.snackBar.open(message, 'OK', {
            duration: 8000,
            panelClass: ['snackbar-error']
          });
        }
      });
    });
  }
}
