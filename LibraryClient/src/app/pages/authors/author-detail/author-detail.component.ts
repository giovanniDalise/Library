import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthorsService } from '../../../services/authors.service';

@Component({
  selector: 'app-author-detail',
  standalone: true,
  imports: [],
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
