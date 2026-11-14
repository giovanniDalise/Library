import { CommonModule } from '@angular/common';
import { Component, afterNextRender, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthenticationService } from '../../services/authentication.service';
import { UserRoleService } from '../../services/user-role.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './main-layout.component.html'
})
export class MainLayoutComponent {
  private authService = inject(AuthenticationService);
  private userRoleService = inject(UserRoleService);
  private router = inject(Router);

  // inject() e costruttore
  // Sono due modi per dire ad Angular “dammi questo service”. Fanno la stessa cosa:

  //   come negli altri tuoi componenti
  //   constructor(private authService: AuthenticationService) { }

  //   il modo nuovo
  //   private authService = inject(AuthenticationService);

  // Perché qui ho usato inject(). Nel layout ho scritto questa riga:

  //   readonly isLoggedIn = this.authService.isLoggedIn;

  // È un campo che usa authService nel momento in cui viene creato. Con la versione a costruttore,
  // in un progetto Angular recente(con il target ES2022 nel tsconfig) i campi vengono inizializzati
  // prima che il costruttore assegni i parametri. Il risultato è che authService è ancora vuoto quando
  // isLoggedIn lo usa, e TypeScript segnala l’errore “Property used before its initialization”.
  // Con inject() il service è disponibile subito, quindi il problema non si pone.
  // Non ho visto il tuo tsconfig, ma questo è un caso molto comune.

  readonly isLoggedIn = this.authService.isLoggedIn;
  // dipende dal signal: si ricalcola a ogni login/logout guardati la spiegazione del singnal nel
  // file md authentication-service e signal.md
  readonly isAdmin = computed(() => this.isLoggedIn() && this.userRoleService.isAdmin());

  constructor() {
    // solo nel browser, dopo il primo disegno: niente mismatch con l'SSR
    afterNextRender(() => this.authService.restoreSession());
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/books']);
  }
}
