import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterOutlet, RouterLink, RouterLinkActive, Router } from '@angular/router';
import { jwtDecode } from 'jwt-decode';
import { AuthenticationService } from '../../services/authentication.service';
import { UserRoleService } from '../../services/user-role.service';


@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, FormsModule ],
  templateUrl: './home-page.component.html',
  styleUrls: ['./home-page.component.scss']
})
export class HomeComponent implements OnInit  {
  bookId: number | undefined;
  text: string = '';
  isAdmin: boolean = false;
  isAuthenticated: boolean = false;
  imageFile = "img/libraryBackground.jpg";

  constructor(
    private router: Router,
    private authService: AuthenticationService,
    private userRoleService: UserRoleService
  ) {}

  ngOnInit() {
    this.isAuthenticated = this.userRoleService.isAuthenticated();
    this.isAdmin = this.userRoleService.isAdmin();
  }

  
  // Metodo per verificare se siamo nel contesto del browser
  private isBrowser(): boolean {
    return typeof window !== 'undefined' && typeof window.localStorage !== 'undefined';
  }  

  searchBookById(): void {
    if (this.bookId) {
      this.router.navigate(['/books-grid', this.bookId]);
    }
  }

  searchBookByText(): void {
    if (this.text) {
        this.router.navigate(['/books-grid'], { queryParams: { search: this.text } });
      }
  }

  logout(): void {
    this.authService.logout();
    this.isAuthenticated = false; // Aggiorna lo stato locale
    this.isAdmin = false;
    this.router.navigate(['/login']); // Reindirizza alla pagina di login
  }  
  
}