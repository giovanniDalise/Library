import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthenticationService } from '../../services/authentication.service';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { ResetPasswordRequest } from '../../models/authentincation/reset-password-request';

@Component({
  selector: 'app-reset-password',
  standalone: true,
  imports: [FormsModule, RouterModule],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.scss'
})
export class ResetPasswordComponent {

  newPassword:string = '';
  confirmPassword:string = '';

  constructor(
    private authService:AuthenticationService,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar
  ){}

  resetPassword():void{
    const urlToken = this.route.snapshot.queryParamMap.get('token');
    
    if (!urlToken) {
      this.snackBar.open(
        'Invalid or missing reset token',
        'OK',
        { duration: 5000, panelClass: ['snackbar-error'] }
      );
      return;
    }

    if (!this.newPassword) {
      this.snackBar.open('New Password is required', 'OK', { duration: 5000, panelClass: ['snackbar-error'] });
      return;
    }    

    if (!this.confirmPassword) {
      this.snackBar.open('Confirm Password is required', 'OK', { duration: 5000, panelClass: ['snackbar-error'] });
      return;
    }    

    if (this.newPassword !== this.confirmPassword) {
      this.snackBar.open('Passwords do not match', 'OK', { duration: 5000, panelClass: ['snackbar-error'] });
      return;
    }

    const request: ResetPasswordRequest = {
      newPassword: this.newPassword,
      token: urlToken
    }

    this.authService.resetPassword(request).subscribe({
      next: (response) => {
        this.snackBar.open(response.message, 'OK', { duration: 6000, panelClass: ['snackbar-success'] });
      },
      error: () => {
        this.snackBar.open('Something went wrong', 'OK', { duration: 8000, panelClass: ['snackbar-error'] });
      }
    });
  }
}
