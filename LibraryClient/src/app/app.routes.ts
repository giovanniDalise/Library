import { Routes } from '@angular/router';
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
import { ConfirmEmailComponent } from './pages/confirm-email/confirm-email.component';
import { ForgotPasswordComponent } from './pages/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { ProfilePageComponent } from './pages/profile/profile-page/profile-page.component';

export const routes: Routes = [
    // senza barra
    { path: 'login', component: LoginComponent },
    { path: 'signup', component: SignupComponent },
    { path: 'confirm-email', component: ConfirmEmailComponent },
    { path: 'forgot-password', component: ForgotPasswordComponent },
    { path: 'reset-password', component: ResetPasswordComponent },
    { path: 'books/add', component: BookFormComponent, canActivate: [AuthGuard] },
    { path: 'books/edit/:bookId', component: BookFormComponent, canActivate: [AuthGuard] },
    { path: 'editors/add', component: EditorsFormComponent, canActivate: [AuthGuard] },
    { path: 'editors/edit/:editorId', component: EditorsFormComponent, canActivate: [AuthGuard] },
    { path: 'authors/add', component: AuthorsFormComponent, canActivate: [AuthGuard] },
    { path: 'authors/edit/:authorId', component: AuthorsFormComponent, canActivate: [AuthGuard] },

    // con barra
    {
        path: '',
        component: MainLayoutComponent,
        children: [
            { path: '', redirectTo: 'books', pathMatch: 'full' },
            { path: 'home', redirectTo: 'books', pathMatch: 'full' },
            { path: 'books', component: BooksPageComponent },
            { path: 'books/:id', component: BookDetailComponent },
            { path: 'editors', component: EditorsPageComponent },
            { path: 'editors/:id', component: EditorDetailComponent },
            { path: 'authors', component: AuthorsPageComponent },
            { path: 'authors/:id', component: AuthorDetailComponent },
            { path: 'profile', component: ProfilePageComponent, canActivate: [AuthGuard] },            
        ]
    }
];

/* Prima non avevamo il MainLayoutComponent con le sue rotte figle.
Ogni rotta era una pagina che occupava tutto lo schermo, e ogni pagina cambiava l’intera finestra.

Ora ci sono due gruppi:

Senza barra: rotte normali, come prima.

Con barra: una rotta madre con component: MainLayoutComponent e dentro children. La madre disegna la 
barra e un <router-outlet>, cioè il “buco”. Le pagine figlie (libri, editori, autori, profilo)
compaiono dentro quel buco. Quando navighi tra due figlie, la barra non viene ricreata: cambia 
solo il contenuto sotto. Questo è il motivo per cui ci serviva il signal.

path: '' sulla madre significa che non aggiunge niente all’indirizzo: /books resta /books.

Perché le rotte dei form stanno prima?
Il router prova le rotte in ordine e prende la prima che combacia. Dentro il layout c’è books/:id, 
dove :id accetta qualsiasi valore, anche la parola add. Se books/add stesse dopo, il router aprirebbe 
il dettaglio di un libro con id add. Mettendo i form sopra, books/add viene riconosciuta per prima. 
È la regola “le statiche prima di quelle con parametro” che già usavi, applicata ai due gruppi. */