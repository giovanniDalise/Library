import { Component, OnInit } from '@angular/core';
import { EditorsGridComponent } from '../../../components/editors/editors-grid/editors-grid.component';
import { EditorsFiltersComponent } from '../../../components/editors/editors-filters/editors-filters.component';
import { RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserRoleService } from '../../../services/user-role.service';
import { EditorsService } from '../../../services/editors.service';
import { Editor } from '../../../models/editor';

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

  pageSize = 10;
  currentPage = 1;
  totalRecords = 0;

  editorId?: number;
  private lastCriteria: Partial<Editor> = {};

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

  searchEditors(criteria: Partial<Editor> = this.lastCriteria): void {

    this.lastCriteria = criteria;

    const searchCriteria: any = {
      Id: criteria.editorId || 0,
      Name: criteria.name?.trim() || null
    };

    this.editorsService
      .getEditors(searchCriteria, this.currentPage, this.pageSize)
      .subscribe({
        next: results => {
          this.editors = results.items;
          this.totalRecords = results.totalRecords;
        },
        error: error => {
          console.error('Errore nella ricerca:', error);
          this.editors = [];
          this.totalRecords = 0;
        }
      });
  }

  /* ===================== PAGINATION ===================== */

  get totalPages(): number {
    return Math.ceil(this.totalRecords / this.pageSize);
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.searchEditors(this.lastCriteria);
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.searchEditors(this.lastCriteria);; 
    }
  }  
}
