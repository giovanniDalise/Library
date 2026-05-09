import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResponse } from '../models/pagination/paged-response';
import { BookDetail } from '../models/book/book-detail/book-detail';
import { BookRequest } from '../models/book/book/book-request';
import { Book } from '../models/book/book/book';

@Injectable({
  providedIn: 'root'
})
export class BookService {
  private baseUrl = `${environment.apiUrls.books}`;
  private endpoints = environment.api.books;


  constructor(private http:HttpClient) { }

  deleteBook(bookId: number): Observable<any> {
    return this.http.delete(this.baseUrl + this.endpoints.delete.replace('{id}', bookId.toString()));
  }

  createBookFormData(formData: FormData): Observable<Book> {
    return this.http.post<Book>(this.baseUrl + this.endpoints.create, formData);
  }

  updateBook(book: Book): Observable<BookRequest> {
    return this.http.put<Book>(this.baseUrl + this.endpoints.update.replace('{id}', book.id.toString()), book)
  }

  getBooks(criteria: BookRequest, page: number, pageSize: number): Observable<PagedResponse<Book>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());  
    return this.http.post<PagedResponse<Book>>(this.baseUrl + this.endpoints.getBooks, criteria, {params})
  }

  getBookDetail(id:number):Observable<BookDetail>{
    const url = this.baseUrl + this.endpoints.getById.replace('{id}', id.toString());
    return this.http.get<BookDetail>(url);
  }
  
}
