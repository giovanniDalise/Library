import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home-page.component';
import { BookFormComponent } from './components/books/book-form/book-form.component';
import { LoginComponent } from './pages/login/login-page.component';
import { AuthGuard } from './guards/auth.guard';
import { BooksPageComponent } from './pages/books/books-page/books-page.component';
import { EditorsPageComponent } from './pages/editors/editors-page/editors-page.component';
import { EditorDetailComponent } from './pages/editors/editor-detail/editor-detail.component';
import { AuthorsPageComponent } from './pages/authors/authors-page/authors-page.component';
import { AuthorDetailComponent } from './pages/authors/author-detail/author-detail.component';
import { BookDetailComponent } from './pages/books/book-detail/book-detail.component';
import { EditorsFormComponent } from './components/editors/editors-form/editors-form.component';

export const routes: Routes = [
    { path: '', component: HomeComponent }, 
    { path: 'home', component: HomeComponent },  
    { path: 'login', component: LoginComponent },  
    { path: 'books/add', component: BookFormComponent },
    { path: 'books/edit/:bookId', component: BookFormComponent },
    { path: 'books/:id', component: BookDetailComponent },
    { path: 'books', component: BooksPageComponent },    
    { path: 'editors/add', component: EditorsFormComponent },
    { path: 'editors/edit/:editorId', component: EditorsFormComponent },
    { path: 'editors/:id', component: EditorDetailComponent },
    { path: 'editors', component: EditorsPageComponent },
    { path: 'authors', component: AuthorsPageComponent },
    { path: 'authors/:id', component: AuthorDetailComponent },  
];