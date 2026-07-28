import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { PagedResponse } from '../models/pagination/paged-response';
import { AuthorDetail } from '../models/author/author-detail/author-detail';
import { AuthorRequest } from '../models/author/author/author-request';
import { Author } from '../models/author/author/author';

@Injectable({
  providedIn: 'root'
})
export class AuthorService {

  private baseUrl = environment.apiUrls.authors;
  private endpoints = environment.api.authors;
  
  constructor(private http:HttpClient) { }

  getAuthors(searchFilter: AuthorRequest, page:number, pageSize: number): Observable<PagedResponse<Author>>{
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())    
    return this.http.post<PagedResponse<Author>>(this.baseUrl + this.endpoints.getAuthors, searchFilter, {params})  
  }

  getAllAuthors(): Observable<Author[]> {
    const params = new HttpParams().set('all', 'true');
    return this.http.post<PagedResponse<Author>>(
      this.baseUrl + this.endpoints.getAuthors, {}, { params }
    ).pipe(map(response => response.items));
  }

  getAuthorDetail(id: number, page: number, pageSize: number): Observable<AuthorDetail> {
      const params = new HttpParams()
          .set('page', page)
          .set('pageSize', pageSize)
      const url = this.baseUrl + this.endpoints.getAuthorDetail.replace('{id}', id.toString());
      return this.http.get<AuthorDetail>(url, { params });
  }

  addAuthor(request: AuthorRequest): Observable<number> {
    return this.http.post<number>(this.baseUrl + this.endpoints.addAuthor, request);
  }

  updateAuthor(id: number, request: AuthorRequest): Observable<void> {
    const url = this.baseUrl + this.endpoints.update.replace('{id}', id.toString());
    return this.http.put<void>(url, request);
  }  

  getAuthorById(id: number): Observable<Author> {
    const url = this.baseUrl + this.endpoints.getById.replace('{id}', id.toString());
    return this.http.get<Author>(url);
  }

  deleteAuthor(id: number): Observable<void> {
    const url = this.baseUrl + this.endpoints.delete.replace('{id}', id.toString());
    return this.http.delete<void>(url);
  }    
}
