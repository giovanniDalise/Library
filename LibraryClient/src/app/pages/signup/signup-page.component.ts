import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

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
    private router: Router
  ) { }

  register(): void {

    if (
      !this.name ||
      !this.surname ||
      !this.email ||
      !this.password ||
      !this.confirmPassword
    ) {
      alert('Please fill in all fields');
      return;
    }

    if (this.password !== this.confirmPassword) {
      alert('Passwords do not match');
      return;
    }

    this.userService.addUser({
      name: this.name,
      surname: this.surname,
      email: this.email,
      password: this.password,
      role: 1 // ruolo standard utente
    }).subscribe({
      next: (userId) => {
        console.log('User created:', userId);

        alert('Registration completed successfully');

        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error(err);
        alert('Registration failed');
      }
    });
  }
}