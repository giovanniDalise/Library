import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Book } from '../../../models/book/book';
import { FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BooksService } from '../../../services/books.service';
import { ActivatedRoute, Router, RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { Author } from '../../../models/author';
import { HttpHeaders } from '@angular/common/http';
import { MatSnackBar } from '@angular/material/snack-bar'; // Aggiungi l'import per MatSnackBar

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, FormsModule, RouterOutlet, RouterLink, RouterLinkActive, ], 
  templateUrl: './book-form.component.html',
  styleUrl: './book-form.component.scss'
})
export class BookFormComponent {
  @Input() bookData: Book | null = null;
  bookForm: FormGroup;
  mode: 'create' | 'update' = 'create';
  bookId: string | null = null;
  pageTitle = '';
  submitLabel = '';
  coverFile: File | null = null;

  constructor(
    private booksService: BooksService,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    // Inizializza bookForm con un form per gestire un array di autori e un oggetto editor
    this.bookForm = this.fb.group({
      title: [''],
      isbn: [''],
      authors: this.fb.array([this.createAuthor()]),  // Un array per gli autori
      editor: this.fb.group({
        name: [''],
        address: ['']
      })
    });
  }

  // Getter per gli autori. La sintassi del getter è get<nomeProprietà>() e stai richiamando il get per accedere agli authors del formGroup
  // quando utilizzi this.authors sia in addAuthor che in removeAuthor()
  get authors(): FormArray {
    return this.bookForm.get('authors') as FormArray;
  }

  // Metodo per creare un form per un autore
  createAuthor(): FormGroup {
    return this.fb.group({
      name: [''],
      surname: ['']
    });
  }

  // Metodo per aggiungere un autore all'array
  addAuthor() {
    this.authors.push(this.createAuthor()); //Qui stai usando il getter authors per accedere al FormArray degli autori. Quando chiami this.authors, Angular invoca automaticamente il getter e ti restituisce il FormArray. Quindi, puoi aggiungere un nuovo autore con push.
  }

  // Metodo per rimuovere un autore dall'array
  removeAuthor(index: number) {
    this.authors.removeAt(index);
  }

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.mode = params.get('mode') as 'create' | 'update' || 'create';
      this.bookId = params.get('bookId');

      this.setPageTexts();

      if (this.mode === 'update' && this.bookId) {
        const bookIdNumber = Number(this.bookId);

        if (!isNaN(bookIdNumber)) {
          this.booksService.getBooks({ bookId: bookIdNumber }, 1, 10)
            .subscribe(response => {
              const book = response.bookResponse?.[0];

              if (book) {
                this.bookData = book;
                this.bookForm.patchValue(book);
              }
            });
        }
      }
    });
  }


  onSubmit(): void {   //quando premi il submit del form va a chiamare le differenti funzioni
    if (this.bookForm.valid) {
      const book: Book = this.bookForm.value;

      if (this.mode === 'create') {
        this.createBook(book);
      } else if (this.mode === 'update') {
        this.updateBook(book);
      } 
    }
  }

  private createBook(book: Book): void {
    const formData = new FormData();
    formData.append('title', book.title);
    formData.append('isbn', book.isbn);

    // Editor: se editorId esiste lo inviamo, altrimenti lasciamo che il backend lo generi
    if (book.editor?.editorId) {
      formData.append('editor.id', book.editor.editorId.toString());
    }
    formData.append('editor.name', book.editor?.name ?? '');

    // Autori
    book.authors.forEach((author, index) => {
      if (author.authorId) {
        formData.append(`authors[${index}].id`, author.authorId.toString());
      }
      formData.append(`authors[${index}].name`, author.name);
      formData.append(`authors[${index}].surname`, author.surname);
    });

    // Cover opzionale
    if (this.coverFile) {
      formData.append('cover', this.coverFile, this.coverFile.name);
    }

    // Chiamiamo il servizio che invia FormData
    this.booksService.createBookFormData(formData).subscribe(
      (createdBook: Book) => {
        this.snackBar.open(
          `Book created successfully`,
          'OK',
          {
            duration: 6000, // comunque si chiude
            panelClass: ['snackbar-success']
          }
        );
      },
      error => {
        this.snackBar.open(
          'Something went wrong while saving the book',
          'OK',
          {
            duration: 8000,
            panelClass: ['snackbar-error']
          }
        );
      }
    );
  }

  private updateBook(book: Book): void {
    // Verifica che l'ID del libro sia disponibile
    if (this.bookId) {
      const bookIdNumber = Number(this.bookId);  // Trasforma bookId in numero
      if (!isNaN(bookIdNumber)) {
        console.log('Book ID:', bookIdNumber);  // Utilizza bookIdNumber
        const updatedBook = { ...book, bookId: bookIdNumber };  // Imposta l'ID del libro da bookIdNumber
        this.booksService.updateBook(updatedBook).subscribe(response => {
          console.log("Book updated:", response);
          
          // Mostra una notifica di successo dopo l'aggiornamento
          this.snackBar.open('Book updated successfully!', 'Close', {
            duration: 3000,  // Durata della notifica (in millisecondi)
          });
        }, error => {
          // Mostra una notifica di errore in caso di fallimento
          this.snackBar.open('Failed to update book. Please try again.', 'Close', {
            duration: 3000,
            panelClass: ['error-snackbar']  // Puoi personalizzare l'aspetto con classi CSS
          });
        });
      } else {
        console.error('Invalid Book ID:', this.bookId);
      }
    } else {
      console.error('Book ID is missing for update');
    }
  }

  private setPageTexts(): void {
    switch (this.mode) {
      case 'create':
        this.pageTitle = 'Add New Book';
        this.submitLabel = 'Create Book';
        break;

      case 'update':
        this.pageTitle = 'Edit Book';
        this.submitLabel = 'Update Book';
        break;

    }
  }
  onCoverSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.coverFile = input.files[0];
    } else {
      this.coverFile = null; // reset se rimuovono il file
    }
  }

}
