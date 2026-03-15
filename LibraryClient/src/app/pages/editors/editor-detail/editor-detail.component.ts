import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EditorsService } from '../../../services/editors.service';
import { EditorDetail } from '../../../models/editor/editor-detail/editor-details';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { PaginationComponent } from '../../../components/shared/pagination/pagination.component';

@Component({
  selector: 'app-editor-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, PaginationComponent],
  templateUrl: './editor-detail.component.html',
  styleUrl: './editor-detail.component.scss'
})
export class EditorDetailComponent implements OnInit {

  editor: EditorDetail | null = null;
  isLoading = true;
  errorMessage = '';
  editorId!: number;

  pagination = new PaginationState();


  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private editorService: EditorsService
  ) {}

  ngOnInit(): void {
    this.editorId = Number(this.route.snapshot.paramMap.get('id'));
    this.loadDetail();
  }

    loadDetail(): void {
    this.isLoading = true;
    this.editorService.getEditorDetail(this.editorId, this.pagination.currentPage, this.pagination.pageSize)
      .subscribe({
        next: (editor) => {
          this.editor = editor;
          this.pagination.totalRecords = editor.books.totalRecords;
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Error loading editor.';
          this.isLoading = false;
        }
      });
  }

  nextPage(): void {
    this.pagination.next();
    this.loadDetail();
  }

  prevPage(): void {
    this.pagination.prev();
    this.loadDetail();
  }

  onEdit(): void {
    this.router.navigate(['/editors', this.editor?.id, 'edit']);
  }

  onBack(): void {
    this.router.navigate(['/editors']);
  }
}