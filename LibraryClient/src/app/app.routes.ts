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
import { AuthorsFormComponent } from './components/authors/authors-form/authors-form.component';
import { SignupComponent } from './pages/signup/signup-page.component';

export const routes: Routes = [
    { path: '', component: HomeComponent }, 
    { path: 'home', component: HomeComponent },  
    { path: 'login', component: LoginComponent },
    { path: 'books', component: BooksPageComponent },
    { path: 'books/add', component: BookFormComponent, canActivate: [AuthGuard] },
    { path: 'books/edit/:bookId', component: BookFormComponent, canActivate: [AuthGuard] },
    { path: 'books/:id', component: BookDetailComponent },
    { path: 'editors', component: EditorsPageComponent },
    { path: 'editors/add', component: EditorsFormComponent, canActivate: [AuthGuard] },
    { path: 'editors/edit/:editorId', component: EditorsFormComponent, canActivate: [AuthGuard] },
    { path: 'editors/:id', component: EditorDetailComponent },
    { path: 'authors', component: AuthorsPageComponent },
    { path: 'authors/add', component: AuthorsFormComponent, canActivate: [AuthGuard] },
    { path: 'authors/edit/:authorId', component: AuthorsFormComponent, canActivate: [AuthGuard] },
    { path: 'authors/:id', component: AuthorDetailComponent },
    { path: 'signup', component: SignupComponent },
];