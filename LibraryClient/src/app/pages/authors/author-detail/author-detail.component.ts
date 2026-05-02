import { Component } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthorsService } from '../../../services/authors.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-author-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './author-detail.component.html',
  styleUrl: './author-detail.component.scss'
})
export class AuthorDetailComponent {

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authorService: AuthorsService
  ) {}  
  
  onBack(): void {
    this.router.navigate(['/authors']);
  }  
}
