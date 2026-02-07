import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home-page.component';
import { BookFormComponent } from './components/books/book-form/book-form.component';
import { LoginComponent } from './pages/login/login-page.component';
import { AuthGuard } from './guards/auth.guard';
import { BooksPageComponent } from './pages/books/books-page/books-page.component';
import { BooksGridComponent } from './components/books/books-grid/books-grid.component';

export const routes: Routes = [
    { path: '', component: HomeComponent }, 
    { path: 'home', component: HomeComponent},  
    { path: 'login', component: LoginComponent},  
    { path: 'books', component: BooksPageComponent },    
    { path: 'books-grid/:bookId', component: BooksGridComponent, canActivate: [AuthGuard] },
    { path: 'book-form/:mode', component: BookFormComponent, canActivate: [AuthGuard] },
    { path: 'book-form/:bookId/:mode', component: BookFormComponent, canActivate: [AuthGuard] },
];