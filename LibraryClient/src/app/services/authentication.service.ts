import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { ApiResponse } from '../models/common/ApiResponse';
import { ResetPasswordRequest } from '../models/authentincation/reset-password-request';


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

  confirmEmail(token: string): Observable<ApiResponse>{
      const url = this.baseUrl + this.endpoints.confirmEmail.replace('{token}', token);
      return this.http.get<ApiResponse>(url);
  }   

  sendResetLink(email: string):Observable<ApiResponse> {
    // non devi creare un oggetto ApiResponse sul BE. Il Be ci restitisce qualcosa cosa { "message": "If this email exists..." }
    // e finché il campo si chiama message, il Fe riesce a mapparco com ApiResponse e in entrambi i lati funziona.
    return this.http.post<ApiResponse>(this.baseUrl + this.endpoints.forgotPassword, {email});
  // Nel body inviamo un oggetto JSON { "email": "mario@example.com" },
    // che il BE deserializza nel DTO ForgotPasswordRequest tramite [FromBody].
    // In questo modo il contratto della richiesta è esplicito e coerente
    // tra FE e BE.
    //
    // Avremmo potuto inviare anche direttamente l'email come stringa senza le graffe che creano l'oggetto anonimo
    // (this.baseUrl + this.endpoints.forgotPassword, email);,
    // ma avremmo dovuto serializzarla correttamente come stringa JSON
    // (es. "mario@example.com") e impostare il Content-Type appropriato (application/json), 
    //
    // this.http.post(this.baseUrl + this.endpoints.forgotPassword, JSON.stringify(email), {
    // headers: { 'Content-Type': 'application/json' }});
    //
    //mantenendo nel BE un parametro [FromBody] string.
    // Inviando invece un oggetto, Angular serializza automaticamente il body
    // in JSON e il BE può utilizzare il DTO ForgotPasswordRequest.
    // Per coerenza al nostro progetto dove utiliziamo tutti DTO per chiarezza va bene aver creato 
    // ForgotPasswordRequest anche con la sola proprietà string email 
  }

  resetPassword(request:ResetPasswordRequest):Observable<ApiResponse>{
    return this.http.post<ApiResponse>(this.baseUrl + this.endpoints.resetPassword, request);
  }

}