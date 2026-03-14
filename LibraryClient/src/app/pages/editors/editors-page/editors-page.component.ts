import { Component, OnInit } from '@angular/core';
import { EditorsGridComponent } from '../../../components/editors/editors-grid/editors-grid.component';
import { EditorsFiltersComponent } from '../../../components/editors/editors-filters/editors-filters.component';
import { RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserRoleService } from '../../../services/user-role.service';
import { EditorsService } from '../../../services/editors.service';
import { Editor } from '../../../models/editor/editor';
import { EditorRequest } from '../../../models/editor/editor-request';
import { PaginationState } from '../../../models/pagination/pagination-state';

@Component({
  selector: 'app-editors-page',
  standalone: true,
  imports: [EditorsGridComponent, EditorsFiltersComponent, RouterLink],
  templateUrl: './editors-page.component.html',
  styleUrl: './editors-page.component.scss'
})
export class EditorsPageComponent implements OnInit {

  editors: Editor[] = [];
  isAdmin = false;
  isAuthenticated = false;

  pagination = new PaginationState();

  editorId?: number;
  private lastCriteria: EditorRequest = {};

  constructor(
    private editorsService: EditorsService,
    private snackBar: MatSnackBar,
    private userRoleService: UserRoleService
  ){}

  ngOnInit(): void {
    this.isAuthenticated = this.userRoleService.isAuthenticated();
    this.isAdmin = this.userRoleService.isAdmin()

    this.searchEditors();
  }  

  searchEditors(criteria: EditorRequest  = this.lastCriteria): void {

    this.lastCriteria = criteria;

    const searchCriteria: EditorRequest = {
      id: criteria.id ?? undefined,
      name: criteria.name?.trim() || undefined
    };

    this.editorsService
      .getEditors(searchCriteria, this.pagination.currentPage, this.pagination.pageSize)
      .subscribe({
        next: results => {
          this.editors = results.items;
          this.pagination.totalRecords = results.totalRecords;
        },
        error: error => {
          console.error('Errore nella ricerca:', error);
          this.editors = [];
          this.pagination.totalRecords = 0;
        }
      });
  }

  /* ===================== PAGINATION ===================== */

  nextPage(): void {
    this.pagination.next();
    this.searchEditors(this.lastCriteria);
  }

  prevPage(): void {
    this.pagination.prev();
    this.searchEditors(this.lastCriteria);
  } 
}
