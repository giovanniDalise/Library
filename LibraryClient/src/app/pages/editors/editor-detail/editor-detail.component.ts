import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EditorsService } from '../../../services/editors.service';
import { EditorDetail } from '../../../models/editor/editor-detail/editor-details';

@Component({
  selector: 'app-editor-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './editor-detail.component.html',
  styleUrl: './editor-detail.component.scss'
})
export class EditorDetailComponent implements OnInit {

  editor: EditorDetail | null = null;
  isLoading = true;
  errorMessage = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private editorService: EditorsService
  ) {}

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.editorService.getEditorDetail(id).subscribe({ 
      next: (editor) => {
        this.editor = editor;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Error loading editor.';
        this.isLoading = false;
      }
    });
  }

  onEdit(): void {
    this.router.navigate(['/editors', this.editor?.id, 'edit']);
  }

  onBack(): void {
    this.router.navigate(['/editors']);
  }
}