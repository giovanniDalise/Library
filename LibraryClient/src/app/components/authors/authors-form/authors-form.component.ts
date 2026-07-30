import { Component } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { AuthorService } from '../../../services/author.service';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthorRequest } from '../../../models/author/author/author-request';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-authors-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './authors-form.component.html',
  styleUrl: './authors-form.component.scss'
})
export class AuthorsFormComponent {

  mode: 'create' | 'update' = 'create';
  authorId: number | null = null;
  pageTitle = '';
  submitLabel = '';

  authorForm = new FormGroup({
    name: new FormControl('', Validators.required),
    surname: new FormControl('', Validators.required)
  });

  constructor(
    private authorService: AuthorService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const id = params.get('authorId');
      this.authorId = id ? Number(id) : null;
      this.mode = this.authorId ? 'update' : 'create';


      this.setPageTexts();

      if (this.mode === 'update' && this.authorId) {
        this.authorService.getAuthorById(this.authorId).subscribe({
          next: author => {
            this.authorForm.patchValue({ name: author.name });
            this.authorForm.patchValue({ surname: author.surname });
          },
          error: err => console.error('Error loading author:', err)
        });
      }
    });
  }

  onSubmit(): void {
    if (this.authorForm.invalid) return;

    const request: AuthorRequest = {
      name: this.authorForm.value.name?.trim(),
      surname: this.authorForm.value.surname?.trim()
    };

    if (this.mode === 'create') {
      this.createAuthor(request);
    } else {
      this.updateAuthor(request);
    }
  }

  private createAuthor(request: AuthorRequest): void {
    this.authorService.addAuthor(request).subscribe({
      next: createdId => {
        this.snackBar.open('Author created successfully', 'OK', {
          duration: 6000,
          panelClass: ['snackbar-success']
        });
        this.router.navigate(['/authors', createdId]);
      },
      error: () => {
        this.snackBar.open('Something went wrong while saving the author', 'OK', {
          duration: 8000,
          panelClass: ['snackbar-error']
        });
      }
    });
  }

  private updateAuthor(request: AuthorRequest): void {
    if (!this.authorId) return;

    this.authorService.updateAuthor(this.authorId, request).subscribe({
      next: () => {
        this.snackBar.open('Author updated successfully', 'OK', {
          duration: 6000,
          panelClass: ['snackbar-success']
        });
        this.router.navigate(['/authors', this.authorId]);
      },
      error: () => {
        this.snackBar.open('Something went wrong while updating the author', 'OK', {
          duration: 8000,
          panelClass: ['snackbar-error']
        });
      }
    });
  }

  private setPageTexts(): void {
    switch (this.mode) {
      case 'create':
        this.pageTitle = 'Add New Author';
        this.submitLabel = 'Create Author';
        break;
      case 'update':
        this.pageTitle = 'Edit Author';
        this.submitLabel = 'Update Author';
        break;
    }
  }  
}
