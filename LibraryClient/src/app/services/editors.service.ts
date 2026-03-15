import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/paged-response';
import { Editor } from '../models/editor/editor/editor';
import { EditorRequest } from '../models/editor/editor/editor-request';
import { EditorDetail } from '../models/editor/editor-detail/editor-details';

@Injectable({
  providedIn: 'root'
})
export class EditorsService {
  private baseUrl = `${environment.apiUrls.editors}`;
  private endpoints = environment.api.editors;

  constructor(private http:HttpClient) { }

  getEditorDetail(id: number): Observable<EditorDetail> {
    const url = this.baseUrl + this.endpoints.getById.replace('{id}', id.toString());
    return this.http.get<EditorDetail>(url);
  }
  
  getEditors(criteria: EditorRequest, page:number, pagesize:number): Observable<PagedResponse<Editor>>{
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pagesize.toString())
    return this.http.post<PagedResponse<Editor>>(this.baseUrl + this.endpoints.getEditors, criteria, {params})
  }
}
