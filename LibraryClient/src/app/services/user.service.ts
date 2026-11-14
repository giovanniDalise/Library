import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import { UserRequest } from "../models/user/user-request";
import { User } from "../models/user/user";

@Injectable({
  providedIn: 'root'
})
export class UserService {
    
    private baseUrl = environment.apiUrls.users;
    private endpoints = environment.api.users;

    constructor(private http:HttpClient) { }   

    addUser(user: UserRequest): Observable<number> {
        return this.http.post<number>(this.baseUrl + this.endpoints.addUser,user);  
    }  
    
    getMe(): Observable<User> {
        return this.http.get<User>(this.baseUrl + this.endpoints.me);
    }

    updatePhoto(file: File): Observable<User> {
        const formData = new FormData();
        formData.append('photo', file);   // stesso nome del parametro IFormFile photo nel controller
        return this.http.put<User>(this.baseUrl + this.endpoints.photo, formData);
    }    
}