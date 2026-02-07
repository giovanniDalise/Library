import { Component } from '@angular/core';
import { AuthenticationService } from '../../services/authentication.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router'; 

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterModule], 
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.scss'
})
export class LoginComponent {

  email: string = '';
  password: string = '';

  constructor(private auth: AuthenticationService, private router: Router) {}

  login() {
    if (this.email && this.password) {
      this.auth.login(this.email, this.password).subscribe({
        next: (resp) => {
          if (typeof window !== 'undefined' && window.sessionStorage) {
            console.log("Token salvato:", resp.token); // Verifica che il token sia corretto
            sessionStorage.setItem("authToken", resp.token);
          }
          this.router.navigate(['home']);
        },
        error: () => {
          alert("Email o password non validi");
        }
      });      
    }
  }
}
