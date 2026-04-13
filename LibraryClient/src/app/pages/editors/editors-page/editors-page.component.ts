import { Component, OnInit } from '@angular/core';
import { EditorsGridComponent } from '../../../components/editors/editors-grid/editors-grid.component';
import { EditorsFiltersComponent } from '../../../components/editors/editors-filters/editors-filters.component';
import { Router, RouterLink } from '@angular/router';
import { UserRoleService } from '../../../services/user-role.service';
import { EditorsService } from '../../../services/editors.service';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { EditorRequest } from '../../../models/editor/editor/editor-request';
import { Editor } from '../../../models/editor/editor/editor';

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

  // uso il new perchè è una classe e ha il costruttore e logica (metodi)
  pagination = new PaginationState();

  // ultimo filtro impostato utile per tenere in memoria il filtro e non perderlo con il cambio pagina
  // Se una variabile: NON deve essere usata nel template, NON deve essere esposta mettila private senza pensarci
  // non uso il new perchè è un interface (molto utile per i dto) che non ha costruttore e logica e quindi inizializzo l'oggetto vuoto
  private lastSearchFilter: EditorRequest = {};

  // il private nel costruttore invece ha una funzione di shortcut in typescript ad esempio se no avresti doveuto prima dichiarare le singole
  // variabili. ESEMPIO:
  /* private authorService: AuthorsService;

    constructor(authorService: AuthorsService) {
      this.authorService = authorService; 
    }
  */
  constructor(
    private editorsService: EditorsService,
    private userRoleService: UserRoleService,
    private router: Router 
  ){}

  ngOnInit(): void {
    this.isAuthenticated = this.userRoleService.isAuthenticated();
    this.isAdmin = this.userRoleService.isAdmin();

    this.searchEditors();
  }  

  // Default parameter → "non mi è arrivato nulla, usa quello che avevo già salvato" 
  //  serve per i cambi pagina e i refresh senza nuovo filtro.
  searchEditors (searchFilter:EditorRequest = this.lastSearchFilter): void{
  //mi è arrivato un nuovo filtro, salvalo" — serve per portarlo nei cambi pagina futuri.
    
    this.lastSearchFilter = searchFilter;

    const normalizedFilter: EditorRequest = {
    //assegnazione chiave valore utiliziamo : (solitamente invece usato per definire i tipi) quando costruiamo un oggetto  e non = (javascriptBase)
      id: searchFilter.id ?? undefined,
      name: searchFilter.name?.trim() ?? undefined
    };

    this.editorsService.getEditors(normalizedFilter, this.pagination.currentPage, this.pagination.pageSize).subscribe(
      //subscribe accetta un oggetto con due callback dentro
      {
        // prima callback: riceve i risultati e aggiorna le proprietà del componente
        next: results => {
          this.editors = results.items;
          this.pagination.totalRecords = results.totalRecords;
        },
        // seconda callback: in caso di errore logga e resetta i dati     
        error: error => {
          console.error ("Search error:", error)
          this.editors = [];
          this.pagination.totalRecords = 0;
        }
      }
    );
  }

  viewDetail(editorId: number): void {
    this.router.navigate(['/editors', editorId]);
  }

  /* ===================== PAGINATION ===================== */

  nextPage(): void {
    this.pagination.next();
    this.searchEditors(this.lastSearchFilter);
  }

  prevPage(): void {
    this.pagination.prev();
    this.searchEditors(this.lastSearchFilter);
  } 


}

