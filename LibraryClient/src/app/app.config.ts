import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideClientHydration } from '@angular/platform-browser';
import { provideHttpClient, withInterceptors, withFetch  } from '@angular/common/http';
import { provideAnimations } from '@angular/platform-browser/animations';
import { JwtInterceptor } from './interceptors/jwt.interceptor';  // Importa la funzione dell'interceptor


export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideClientHydration(),
    provideHttpClient(
      withFetch(),  
      withInterceptors([JwtInterceptor])  // Usa l'interceptor come funzione
    ),
    provideAnimations(),  // Fornisce le animazioni
  ]
};
