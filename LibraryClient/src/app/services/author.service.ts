import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination/paged-response';
import { AuthorDetail } from '../models/author/author-detail/author-detail';
import { AuthorRequest } from '../models/author/author/author-request';
import { Author } from '../models/author/author/author';

@Injectable({
  providedIn: 'root'
})
export class AuthorService {

  private baseUrl = `${environment.apiUrls.authors}`;
  private endpoints = environment.api.authors;
  
  constructor(private http:HttpClient) { }

  getAuthors(searchFilter: AuthorRequest, page:number, pageSize: number): Observable<PagedResponse<Author>>{
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())    
    return this.http.post<PagedResponse<Author>>(this.baseUrl + this.endpoints.getAuthors, searchFilter, {params})  
  }


  getAuthorDetail(id: number, page: number, pageSize: number): Observable<AuthorDetail> {
      const params = new HttpParams()
          .set('page', page)
          .set('pageSize', pageSize)
      const url = this.baseUrl + this.endpoints.getById.replace('{id}', id.toString());
      return this.http.get<AuthorDetail>(url, { params });
  }  
}
