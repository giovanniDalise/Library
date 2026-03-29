import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination/paged-response';
import { Author } from '../models/author/author';
import { AuthorRequest } from '../models/author/author-request';

@Injectable({
  providedIn: 'root'
})
export class AuthorsService {

  private baseUrl = `${environment.apiUrls.authors}`;
  private endpoints = environment.api.authors;
  
  constructor(private http:HttpClient) { }

  getAuthors(searchFilter: AuthorRequest, page:number, pageSize: number): Observable<PagedResponse<Author>>{
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())    
    return this.http.post<PagedResponse<Author>>(this.baseUrl + this.endpoints.getAuthors, searchFilter, {params})  
  }
}
