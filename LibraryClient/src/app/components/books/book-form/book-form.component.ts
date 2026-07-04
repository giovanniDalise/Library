import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BookService } from '../../../services/book.service';
import { EditorService } from '../../../services/editor.service';
import { AuthorService } from '../../../services/author.service';
import { Editor } from '../../../models/editor/editor/editor';
import { Author } from '../../../models/author/author/author';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, RouterLink, FormsModule],
  templateUrl: './book-form.component.html',
  styleUrl: './book-form.component.scss'
})
export class BookFormComponent implements OnInit {

  mode: 'create' | 'update' = 'create';
  bookId: number | null = null;
  pageTitle = '';
  submitLabel = '';
  coverFile: File | null = null;

  // Liste per le dropdown
  availableEditors: Editor[] = [];
  availableAuthors: Author[] = [];

  // Autori selezionati
  selectedAuthors: Author[] = [];
  selectedAuthorId: number | null = null;

  bookForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private bookService: BookService,
    private editorService: EditorService,
    private authorService: AuthorService,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.bookForm = this.fb.group({
      title: ['', Validators.required],
      isbn: ['', Validators.required],
      editorId: [null, Validators.required],
    });
  }

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('bookId');
    this.bookId = id ? Number(id) : null;
    this.mode = this.bookId ? 'update' : 'create';
    this.setPageTexts();

    this.loadEditors();
    this.loadAuthors();

    if (this.mode === 'update' && this.bookId) {
      this.bookService.getBookDetail(this.bookId).subscribe({
        next: book => {
          this.bookForm.patchValue({
            title: book.title,
            isbn: book.isbn,
            editorId: book.editor?.id
          });
          // pre-popola autori selezionati
          this.selectedAuthors = book.authors ?? [];
        },
        error: err => console.error('Error loading book:', err)
      });
    }
  }

  private loadEditors(): void {
    this.editorService.getAllEditors().subscribe({
      next: editors => this.availableEditors = editors,
      error: err => console.error('Error loading editors:', err)
    });
  }

  private loadAuthors(): void {
    this.authorService.getAuthors({}, 1, 100).subscribe({
      next: result => this.availableAuthors = result.items,
      error: err => console.error('Error loading authors:', err)
    });
  }

  addSelectedAuthor(): void {
    if (!this.selectedAuthorId) return;

    const author = this.availableAuthors.find(a => a.id === Number(this.selectedAuthorId));
    if (!author) return;

    // evita duplicati
    if (this.selectedAuthors.some(a => a.id === author.id)) return;

    this.selectedAuthors = [...this.selectedAuthors, author];
    this.selectedAuthorId = null;
  }

  removeAuthor(authorId: number): void {
    this.selectedAuthors = this.selectedAuthors.filter(a => a.id !== authorId);
  }

  onCoverSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.coverFile = input.files?.[0] ?? null;
  }

  onSubmit(): void {
    if (this.bookForm.invalid || this.selectedAuthors.length === 0) return;

    const formData = new FormData();
    formData.append('title', this.bookForm.value.title);
    formData.append('isbn', this.bookForm.value.isbn);
    formData.append('editor.id', this.bookForm.value.editorId.toString());

    this.selectedAuthors.forEach((author, index) => {
      formData.append(`authors[${index}].id`, author.id.toString());
    });

    if (this.coverFile) {
      formData.append('cover', this.coverFile, this.coverFile.name);
    }

    if (this.mode === 'create') {
      this.createBook(formData);
    } else {
      this.updateBook(formData);
    }
  }

  private createBook(formData: FormData): void {
    this.bookService.addBook(formData).subscribe({
      next: createdId => {
        this.snackBar.open('Book created successfully', 'OK', { duration: 6000, panelClass: ['snackbar-success'] });
        this.router.navigate(['/books', createdId]);
      },
      error: () => {
        this.snackBar.open('Something went wrong while saving the book', 'OK', { duration: 8000, panelClass: ['snackbar-error'] });
      }
    });
  }

  private updateBook(formData: FormData): void {
    if (!this.bookId) return;
    this.bookService.updateBook(this.bookId, formData).subscribe({
      next: () => {
        this.snackBar.open('Book updated successfully', 'OK', { duration: 3000 });
        this.router.navigate(['/books', this.bookId]);
      },
      error: () => {
        this.snackBar.open('Failed to update book', 'OK', { duration: 3000, panelClass: ['snackbar-error'] });
      }
    });
  }

  private setPageTexts(): void {
    this.pageTitle = this.mode === 'create' ? 'Add New Book' : 'Edit Book';
    this.submitLabel = this.mode === 'create' ? 'Create Book' : 'Update Book';
  }
}