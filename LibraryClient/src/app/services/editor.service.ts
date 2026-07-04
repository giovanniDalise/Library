import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { map, Observable } from 'rxjs';
import { Editor } from '../models/editor/editor/editor';
import { EditorRequest } from '../models/editor/editor/editor-request';
import { EditorDetail } from '../models/editor/editor-detail/editor-details';
import { PagedResponse } from '../models/pagination/paged-response';

@Injectable({
  providedIn: 'root'
})
export class EditorService {
  private baseUrl = environment.apiUrls.editors;
  private endpoints = environment.api.editors;

  constructor(private http:HttpClient) { }

  getEditorDetail(id: number, page: number, pageSize: number): Observable<EditorDetail> {
      const params = new HttpParams()
          .set('page', page)
          .set('pageSize', pageSize)
      const url = this.baseUrl + this.endpoints.getEditorDetail.replace('{id}', id.toString());
      return this.http.get<EditorDetail>(url, { params });
  }
  
  getEditors(searchFilter:EditorRequest, page:number, pageSize:number):Observable<PagedResponse<Editor>>{
    /*HttpParams è una classe particolare utilizzata per le query string negli url ?page=1&pageSize=10
    non puoi semplicemente scrivere key:value come gli altri oggetti js perchè httpparams è immutabile e quindi ogni volta che
    cambia il valore sarà creato un nuovo oggetto httpparams che vuole la sintassi con il set("key", value)
    */
    const params = new HttpParams().set("page", page).set("pageSize",pageSize);
    return this.http.post<PagedResponse<Editor>>(this.baseUrl + this.endpoints.getEditors, searchFilter, {params})
  }
    /*{params} è tra graffe perchè l'HttpClient.post come terzo parametro vuole un oggetto di configurazione e le graffe in angular
    servono proprio a definire un oggetto
    {
        params?: HttpParams;
        headers?: HttpHeaders;
        observe?: ...
        responseType?: ...
    } 
    che nel nostro caso avendo solo i params sarebbbe 
    {
          params: params
    } 
    oppure con lo shortland (uno shortcut) possiamo direttamente scrivere {params}     
    */
  getAllEditors(): Observable<Editor[]> {
    const params = new HttpParams().set('all', 'true');
    return this.http.post<PagedResponse<Editor>>(
      this.baseUrl + this.endpoints.getEditors, {}, { params }
/*       Il secondo parametro di http.post è il body della richiesta. Nella getEditors normale passi 
      searchFilter come body perché vuoi filtrare. Nella getAllEditors non vuoi filtrare nulla, 
      ma http.post vuole comunque un body — non puoi ometterlo. Quindi passi {} che è un oggetto vuoto,
       che il BE riceve come EditorRequest con tutti i campi null/default, quindi nessun filtro applicato. */
    ).pipe(map(response => response.items));
/*     getEditors ritorna Observable<PagedResponse<Editor>> — un oggetto con items e totalRecords. 
    getAllEditors invece vuole ritornare direttamente Observable<Editor[]> 
    solo la lista, senza la paginazione che non serve per la dropdown.
    Il .pipe(map(...)) trasforma l'Observable prima che arrivi al subscriber:
    È come fare il .Select() di LINQ in C# — prendi l'oggetto che arriva e ne estrai solo la parte che 
    ti interessa, così chi chiama getAllEditors riceve direttamente Editor[] 
    senza dover fare result.items ogni volta. */
  }
  
  addEditor(request: EditorRequest): Observable<number> {
    return this.http.post<number>(this.baseUrl + this.endpoints.addEditor, request);
  }

  updateEditor(id: number, request: EditorRequest): Observable<void> {
    const url = this.baseUrl + this.endpoints.update.replace('{id}', id.toString());
    return this.http.put<void>(url, request);
  }

  deleteEditor(id: number): Observable<void> {
    const url = this.baseUrl + this.endpoints.delete.replace('{id}', id.toString());
    return this.http.delete<void>(url);
  }    

  getEditorById(id: number): Observable<Editor> {
    const url = this.baseUrl + this.endpoints.getById.replace('{id}', id.toString());
    return this.http.get<Editor>(url);
  }  
}
