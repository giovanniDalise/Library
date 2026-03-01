import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Editor } from '../models/editor';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/paged-response';

@Injectable({
  providedIn: 'root'
})
export class EditorsService {
  private baseUrl = `${environment.apiUrls.editors}`;
  private endpoints = environment.api.editors;

  constructor(private http:HttpClient) { }
  
  getEditors(criteria: Partial<Editor>, page:number, pagesize:number): Observable<PagedResponse<Editor>>{
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pagesize.toString())
    return this.http.post<PagedResponse<Editor>>(this.baseUrl + this.endpoints.getEditors, criteria, {params})
  }
}
