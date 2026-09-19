import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './signup-page.component.html',
  styleUrl: './signup-page.component.scss'
})
export class SignupComponent {

  name: string = '';
  surname: string = '';
  email: string = '';
  password: string = '';
  confirmPassword: string = '';

  constructor(
    private userService: UserService,
    private router: Router,
    private snackBar: MatSnackBar
  ) { }

  register(): void {
    if (!this.name || !this.surname || !this.email || !this.password || !this.confirmPassword) {
      this.snackBar.open('Please fill in all fields', 'OK', {
        duration: 5000,
        panelClass: ['snackbar-error']
      });
      return;
    }

    if (this.password !== this.confirmPassword) {
      this.snackBar.open('Passwords do not match', 'OK', {
        duration: 5000,
        panelClass: ['snackbar-error']
      });
      return;
    }

    this.userService.addUser({
      name: this.name,
      surname: this.surname,
      email: this.email,
      password: this.password,
      role: 1
    }).subscribe({
      next: () => {
        this.snackBar.open('Registration completed! Check your email to confirm your account.', 'OK', {
          duration: 8000,
          panelClass: ['snackbar-success']
        });
        this.router.navigate(['/login']);
      },
      error: () => {
        this.snackBar.open('Registration failed. Please try again.', 'OK', {
          duration: 8000,
          panelClass: ['snackbar-error']
        });
      }
    });
  }
}