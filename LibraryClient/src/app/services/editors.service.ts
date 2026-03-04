import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/paged-response';
import { Editor } from '../models/editor/editor';
import { EditorRequest } from '../models/editor/editor-request';

@Injectable({
  providedIn: 'root'
})
export class EditorsService {
  private baseUrl = `${environment.apiUrls.editors}`;
  private endpoints = environment.api.editors;

  constructor(private http:HttpClient) { }
  
  getEditors(criteria: EditorRequest, page:number, pagesize:number): Observable<PagedResponse<Editor>>{
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pagesize.toString())
    return this.http.post<PagedResponse<Editor>>(this.baseUrl + this.endpoints.getEditors, criteria, {params})
  }
}
