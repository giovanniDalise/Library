import { Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class UserRoleService {

  private roleClaim = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role';

  constructor() {}

  // 🔒 unico punto di accesso a sessionStorage
  private getToken(): string | null {
    if (typeof window === 'undefined') {
      return null;
    }
    return sessionStorage.getItem('authToken');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }

  isAdmin(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const decodedToken: any = jwtDecode(token);
      return decodedToken[this.roleClaim] === 'admin';
    } catch (e) {
      console.error('Errore nel decodificare il token', e);
      return false;
    }
  }

  getRole(): string | null {
    const token = this.getToken();
    if (!token) return null;

    try {
      const decodedToken: any = jwtDecode(token);
      return decodedToken[this.roleClaim] ?? null;
    } catch {
      return null;
    }
  }
}
