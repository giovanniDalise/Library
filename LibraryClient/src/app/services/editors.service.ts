import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Editor } from '../models/editor/editor/editor';
import { EditorRequest } from '../models/editor/editor/editor-request';
import { EditorDetail } from '../models/editor/editor-detail/editor-details';
import { PagedResponse } from '../models/pagination/paged-response';

@Injectable({
  providedIn: 'root'
})
export class EditorsService {
  private baseUrl = `${environment.apiUrls.editors}`;
  private endpoints = environment.api.editors;

  constructor(private http:HttpClient) { }

  getEditorDetail(id: number, page: number, pageSize: number): Observable<EditorDetail> {
      const params = new HttpParams()
          .set('page', page.toString())
          .set('pageSize', pageSize.toString());
      const url = this.baseUrl + this.endpoints.getById.replace('{id}', id.toString());
      return this.http.get<EditorDetail>(url, { params });
  }
  
  getEditors(criteria: EditorRequest, page:number, pagesize:number): Observable<PagedResponse<Editor>>{
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pagesize.toString())
    return this.http.post<PagedResponse<Editor>>(this.baseUrl + this.endpoints.getEditors, criteria, {params})
  }
}
