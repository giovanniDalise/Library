import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { AuthorService } from '../../services/author.service';
import { AuthenticationService } from '../../services/authentication.service';

@Component({
  selector: 'app-confirm-email',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirm-email.component.html',
  styleUrl: './confirm-email.component.scss'
})
export class ConfirmEmailComponent implements OnInit {

  isLoading = true;
  success = false;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthenticationService
  ) {}

  ngOnInit(): void {
    const token = this.route.snapshot.queryParamMap.get('token');

    if (!token) {
      this.isLoading = false;
      this.errorMessage = 'Invalid confirmation link.';
      return;
    }

    this.authService.confirmEmail(token).subscribe({
      next: () => {
        this.isLoading = false;
        this.success = true;
        // redirect al login dopo 3 secondi
        setTimeout(() => this.router.navigate(['/login']), 3000);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = err.error?.error ?? 'Token not valid or expired.';
      }
    });
  }

  goToLogin(): void {
    this.router.navigate(['/login']);
  }  
}