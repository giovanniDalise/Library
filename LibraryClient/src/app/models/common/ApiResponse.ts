export interface ApiResponse {
  message: string;
}
//Dal BE restituiamo nei messaggi di risposta oggetti anonimi come{new={error="..."}}
//con una proprietà message. Sul FE questo diventa JSON:
//{ "message": "If this email exists..." }
// Quindi per tutti i metodi del BE che restituiscono un ActionResult con solo il messaggio (come se fosseero void)
// e lato FE quei metodi nel servizi che non prevedono nulla di ritorno nell'Observable se non il messaggio
// abbiamo creato l'interfaccia ApiResponse che rappresenta il messaggio di risposta alle chiamate che facciamo alle varie Api