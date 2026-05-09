import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { EditorDetail } from '../../../models/editor/editor-detail/editor-details';
import { PaginationState } from '../../../models/pagination/pagination-state';
import { PaginationComponent } from '../../../components/shared/pagination/pagination.component';
import { EditorService } from '../../../services/editor.service';

@Component({
  selector: 'app-editor-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, PaginationComponent],
  templateUrl: './editor-detail.component.html',
  styleUrl: './editor-detail.component.scss'
})
export class EditorDetailComponent implements OnInit {

  //rispetto all'EditorRequest che ha tutte proprietà nullabili l'EditorDetail è un dato reale che ci torna dall'api senza dati nullabili,
  // che inizialmente mettiamo a null fino a che non ci viene restituito dall'API.
  // potresti utilizzare anche editor!:EditorDetail (trust me) facendo si che typescript non controlli ma è più rischioso se poi non viene inizializzato
  // in tempo, romendo il FE.
  //diverso è il caso di editors: Editor [] = []; dove editor è un altro dato reale con nuessuna proprietà opzionale e anche nel suo caso nell'editor page component
  //ci arriva da un api ma la differenza qui è che un array vuoto ( o una lista vuota) è uno stato valido mentre un oggetto EditorDetail {} 
  // vuoto non esiste. Potevi creare una classe al posto di un interface con costruttore vuote ma comunque sarebbe incoerente per le logiche.
  editor: EditorDetail | null = null;
  isLoading = true;
  errorMessage = '';
  // in questo caso invece è sicuto utilizzare il trust me (!) del definite assignment assertion dato che editorId viene valorizzato subito
  // nell'onOnInit
  editorId!: number;

  pagination = new PaginationState();


  constructor(
    //ActivatedRoute: legge info(queryparams, pathparams e via dicendo) dalla rotta attuale
    private route: ActivatedRoute,
    //Router: naviga tra le rotte
    private router: Router,
    private editorService: EditorService
  ) {}

  ngOnInit(): void {
    //quindi in questo caso stiamo valorizzando l'editorId con uno snapshot del valore di id nel pathparams dell'url es. da /editors/123 estraggo il valore 123
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