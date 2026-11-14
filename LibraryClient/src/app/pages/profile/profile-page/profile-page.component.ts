import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserService } from '../../../services/user.service';
import { UserRoleService } from '../../../services/user-role.service';
import { User } from '../../../models/user/user';


@Component({
  selector: 'app-profile-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './profile-page.component.html'
})
export class ProfilePageComponent implements OnInit {
  user: User | null = null;
  role: string | null = null;
  isLoading = false;
  isUploading = false;
  errorMessage = '';

  private readonly maxPhotoBytes = 2 * 1024 * 1024;
  private readonly allowedTypes = ['image/jpeg', 'image/png', 'image/webp'];

  constructor(
    private userService: UserService,
    private userRoleService: UserRoleService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.role = this.userRoleService.getRole();   // il nome del ruolo c'è già nel token
    this.loadProfile();
  }

  private loadProfile(): void {
    this.isLoading = true;
    this.userService.getMe().subscribe({
      next: user => {
        this.user = user;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Unable to load your profile.';
        this.isLoading = false;
      }
    });
  }

  onPhotoSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    input.value = '';   // permette di riselezionare lo stesso file
    if (!file) return;

    if (!this.allowedTypes.includes(file.type)) {
      this.showError('Only JPEG, PNG or WebP images are allowed.');
      return;
    }
    if (file.size > this.maxPhotoBytes) {
      this.showError('Photo must be 2 MB or smaller.');
      return;
    }

    this.isUploading = true;
    this.userService.updatePhoto(file).subscribe({
      next: user => {
        this.user = user;
        this.isUploading = false;
        this.snackBar.open('Photo updated', undefined, { duration: 3000, panelClass: 'snackbar-success' });
      },
      error: (err: HttpErrorResponse) => {
        this.isUploading = false;
        this.showError(typeof err.error === 'string' ? err.error : 'Photo upload failed.');
      }
    });
  }

  private showError(message: string): void {
    this.snackBar.open(message, undefined, { duration: 4000, panelClass: 'snackbar-error' });
  }
}