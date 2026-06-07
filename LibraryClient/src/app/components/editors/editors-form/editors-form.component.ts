import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { EditorService } from '../../../services/editor.service';
import { EditorRequest } from '../../../models/editor/editor/editor-request';

@Component({
  selector: 'app-editor-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './editors-form.component.html',
  styleUrl: './editors-form.component.scss'
})
export class EditorsFormComponent implements OnInit {

  mode: 'create' | 'update' = 'create';
  editorId: number | null = null;
  pageTitle = '';
  submitLabel = '';

  editorForm = new FormGroup({
    name: new FormControl('', Validators.required)
  });

  constructor(
    private editorService: EditorService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      this.mode = params.get('mode') as 'create' | 'update' || 'create';
      const id = params.get('editorId');
      this.editorId = id ? Number(id) : null;

      this.setPageTexts();

      if (this.mode === 'update' && this.editorId) {
        this.editorService.getEditorDetail(this.editorId).subscribe({
          next: editor => {
            this.editorForm.patchValue({ name: editor.name });
          },
          error: err => console.error('Error loading editor:', err)
        });
      }
    });
  }

  onSubmit(): void {
    if (this.editorForm.invalid) return;

    const request: EditorRequest = {
      name: this.editorForm.value.name?.trim()
    };

    if (this.mode === 'create') {
      this.createEditor(request);
    } else {
      this.updateEditor(request);
    }
  }

  private createEditor(request: EditorRequest): void {
    this.editorService.addEditor(request).subscribe({
      next: createdId => {
        this.router.navigate(['/editors', createdId]);
      },
      error: err => console.error('Error creating editor:', err)
    });
  }

  private updateEditor(request: EditorRequest): void {
    if (!this.editorId) return;

    this.editorService.updateEditor(this.editorId, request).subscribe({
      next: () => {
        this.router.navigate(['/editors', this.editorId]);
      },
      error: err => console.error('Error updating editor:', err)
    });
  }

  private setPageTexts(): void {
    switch (this.mode) {
      case 'create':
        this.pageTitle = 'Add New Editor';
        this.submitLabel = 'Create Editor';
        break;
      case 'update':
        this.pageTitle = 'Edit Editor';
        this.submitLabel = 'Update Editor';
        break;
    }
  }
}