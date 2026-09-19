import { Component } from '@angular/core';
import { AuthenticationService } from '../../services/authentication.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router'; 
import { MatSnackBar } from '@angular/material/snack-bar';


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

  constructor(
    private auth: AuthenticationService, 
    private router: Router,
    private snackBar: MatSnackBar
  ) {}

  login() {
    if (this.email && this.password) {
      this.auth.login(this.email, this.password).subscribe({
        next: (resp) => {
          if (typeof window !== 'undefined' && window.sessionStorage) {
            sessionStorage.setItem("authToken", resp.token);
          }
          this.router.navigate(['home']);
        },
        error: (err) => {
          const message = err.error?.message ?? 'Email o password non validi';
          this.snackBar.open(message, 'OK', {
            duration: 8000,
            panelClass: ['snackbar-error']
          });
        }
      });
    }
  }
}
