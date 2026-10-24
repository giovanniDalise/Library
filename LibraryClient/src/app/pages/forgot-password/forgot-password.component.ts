import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { AuthenticationService } from '../../services/authentication.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent {
  email: string = '';

  constructor(
    private authService:AuthenticationService,
    private snackBar: MatSnackBar
  ){}

  sendResetLink(): void {
    if (!this.email) {
      this.snackBar.open('Email is required', 'OK', { duration: 5000, panelClass: ['snackbar-error'] });
      return;
    }

    this.authService.sendResetLink(this.email).subscribe({
      next: (response) => {
        this.snackBar.open(response.message, 'OK', { duration: 6000, panelClass: ['snackbar-success'] });
      },
      error: () => {
        this.snackBar.open('Something went wrong', 'OK', { duration: 8000, panelClass: ['snackbar-error'] });
      }
    });
  }
}
