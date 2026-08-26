import { Injectable } from "@angular/core";
import { environment } from "../../environments/environment";
import { HttpClient } from "@angular/common/http";
import { User } from "../models/user/user";
import { Observable } from "rxjs";
import { UserRequest } from "../models/user/user-request";

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
}