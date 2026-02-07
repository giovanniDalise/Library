import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';


interface AuthResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthenticationService {

  private baseUrl = environment.apiUrls.auth;
  private endpoints = environment.api.auth;

  constructor(private http: HttpClient) { }

  login(email: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(this.baseUrl + this.endpoints.login, { email, password }).pipe(
      tap(response => {
        // Salva il token appena viene ricevuto
        if (response && response.token) {
          sessionStorage.setItem('authToken', response.token);
        }
      })
    );
  }
  

  logout(): void {
    if (typeof window !== 'undefined' && window.sessionStorage) {
      sessionStorage.removeItem('authToken');
    }
  }

  isAuthenticated(): boolean {
    // Verifica che sessionStorage sia disponibile
    if (typeof window !== 'undefined' && window.sessionStorage) {
      return !!sessionStorage.getItem('authToken');
    }
    return false;
  }

  getToken(): string | null {
    // Controlla se sessionStorage è disponibile nel client
    if (typeof window !== 'undefined' && window.sessionStorage) {
      const token = sessionStorage.getItem("authToken");
      
      // Log per vedere se il token è presente o meno
      console.log("Token recuperato dalla sessionStorage:", token); // Log aggiunto

      return token;
    }
    console.log("sessionStorage non disponibile"); // Log per il caso in cui sessionStorage non sia disponibile
    return null;
  }
  
}