import { Component, OnInit } from '@angular/core';
import { EditorsGridComponent } from '../../../components/editors/editors-grid/editors-grid.component';
import { EditorsFiltersComponent } from '../../../components/editors/editors-filters/editors-filters.component';
import { RouterLink } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { UserRoleService } from '../../../services/user-role.service';
import { EditorsService } from '../../../services/editors.service';

@Component({
  selector: 'app-editors-page',
  standalone: true,
  imports: [EditorsGridComponent, EditorsFiltersComponent, RouterLink],
  templateUrl: './editors-page.component.html',
  styleUrl: './editors-page.component.scss'
})
export class EditorsPageComponent implements OnInit {

  isAdmin = false;
  isAuthenticated = false;

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

  searchEditors():void{
    
  }
}
