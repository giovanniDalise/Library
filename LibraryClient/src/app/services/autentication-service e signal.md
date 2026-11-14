Introduzione dei Signal nell'AuthenticationService per la gestione della Navbar

La Navbar introduce alcuni elementi, come Profilo e Logout, soltanto dopo che l'utente ha effettuato il login. Inoltre, mostra la sezione Users esclusivamente se l'utente è ADMIN.

1. Il problema dell'SSR e dell'Hydration
In Angular abbiamo l'SSR (Server-Side Rendering), una tecnica che permette di generare le pagine HTML direttamente sul server del frontend (non quello del backend), prima di inviarle al browser (sul computer dell'utente).

Questo permette di mostrare all'utente una pagina già preparata, migliorando i tempi di visualizzazione iniziale e potenzialmente la SEO.

Abbiamo poi l'Hydration, cioè il processo con cui Angular rende interattiva una pagina HTML già generata dal server, riutilizzando il contenuto esistente senza ricrearlo completamente da zero.

In parole semplici:
1. Il server FE(non quello BE) prepara la pagina HTML.
2. Il browser riceve la pagina già disegnata.
3. Angular nel browser la rende interattiva.
Durante questo processo è importante che la struttura iniziale della pagina preparata dal server sia coerente con quella che Angular si aspetta nel browser.

2. Qual era il problema?
Nel nostro progetto il token viene salvato nel sessionStorage del browser quando l'utente effettua il login.

Il problema è che il server FE non può accedere al sessionStorage, perché questa memoria esiste soltanto nel browser.

Normalmente, quando chiudi la scheda, il sessionStorage viene eliminato. Invece quando aggiorni la pagina con F5 oppure quando Angular viene riavviato mantenendo la stessa sessione della scheda il sessionStarage resta e puoi avere questo mismatch.

Immaginiamo questa situazione:
1. L'utente effettua il login.
2. Il token viene salvato nel sessionStorage.
3. L'utente aggiorna la pagina con F5.
4. Il server FE prepara la Navbar senza poter leggere il token, quindi non sa che l'utente è autenticato.
5. Il browser, invece, trova il token e sa che l'utente è autenticato.

Potremmo quindi avere una differenza:
- Server: mostra Login.
- Browser: vorrebbe mostrare Profilo e Logout.
Questa differenza può creare problemi durante l'Hydration (sfarfallio e altre problematiche).

Per questo dobbiamo fare in modo che la pagina iniziale sia coerente tra server e browser, e recuperare lo stato di autenticazione nel momento appropriato, dopo l'Hydration.

3. Il problema della reattività della Navbar
Oltre al problema dell'SSR, avevamo un secondo problema.

La Navbar si trova nell'AppComponent ed è presente in tutte le pagine pubbliche. Quindi non viene distrutta e ricreata ogni volta che navighiamo.

Prima delle modifiche, il controllo dell'autenticazione veniva effettuato tramite una semplice funzione:
isAuthenticated(): boolean {
  return !!sessionStorage.getItem('authToken');
}

Questa funzione restituisce true o false soltanto nel momento in cui viene chiamata.

Quindi, se la Navbar controllava lo stato iniziale e successivamente l'utente effettuava login o logout, non veniva automaticamente informata del cambiamento.

La Navbar poteva quindi continuare a mostrare le informazioni precedenti.

Quindi ricapitlando prima del commit di questo .md di info nel codice originale avevo questo:
isAuthenticated(): boolean {
  if (typeof window !== 'undefined' && window.sessionStorage) {
    return !!sessionStorage.getItem('authToken');
  }
  return false;
}


Vediamo cosa significa.
- isAuthenticated() è una funzione che controlla se l'utente è autenticato.
- sessionStorage.getItem('authToken') cerca il token nel browser.
- !! trasforma il risultato in un valore booleano (true oppure false).

Quindi:
Situazione      	   Risultato
Il token esiste	         true
Il token non esiste	     false
Siamo sul server SSR     false

Nell'ultima siatuazione diciamo che sul server SSR il risultato è false non perchè l'utente non è autenticato ma perchè il server non può controllare il sessionStorage del browser, quindi questa funzione restituisce false per sicurezza.

Fin qui il codice funzionava.

Ma aveva un limite: questa funzione restituisce una risposta soltanto quando qualcuno la chiama.

Non avvisa automaticamente gli altri componenti quando il token cambia.

Facciamo un esempio:
1. La Navbar chiama isAuthenticated().
2. La funzione restituisce false.
3. L'utente fa login.
4. Il token viene salvato.
5. La Navbar non sa che il token è cambiato.
La funzione originale non è sbagliata: semplicemente non è sufficiente per aggiornare automaticamente la Navbar.

2. La prima modifica: importare signal e computed
Abbiamo aggiunto:
import { signal, computed } from '@angular/core';

Che cosa sono?

Sono due strumenti di Angular che ci permettono di gestire dati reattivi.

Per ora ricordati queste due definizioni:
- Signal: contiene un valore che può cambiare e comunica ad Angular quando cambia.
- Computed: calcola un valore partendo da uno o più Signal e si aggiorna quando cambiano.

Vediamoli nel tuo codice.
3. La riga più importante: _token
private readonly _token = signal<string | null>(null);

Questa riga probabilmente è quella che ti sembrerà più complicata, quindi la dividiamo in piccoli pezzi.

A. private

Significa che questa variabile può essere utilizzata direttamente soltanto all'interno di AuthenticationService.

Gli altri componenti non possono modificarla direttamente.

Per esempio, la Navbar non può fare:
this.auth._token.set('123');


Ed è una cosa positiva: vogliamo che sia il servizio a gestire le modifiche all'autenticazione.

B. readonly

Significa che non puoi assegnare nuovamente un altro Signal alla variabile _token.

Attenzione: non significa che il contenuto del Signal non possa cambiare!

Infatti puoi fare:
this._token.set('123');


C. signal<string | null>(null)

Qui c'è una cosa nuova.
- signal crea il nostro contenitore reattivo.
- string | null significa che può contenere una stringa oppure null.
- (null) significa che inizialmente il contenitore è vuoto.
Per esempio:
Il nostro contenitore _token
null
Inizialmente non abbiamo ancora caricato nessun token.
Dopo il login, il contenuto potrà diventare una stringa contenente il token.

Quindi abbiamo creato una variabile speciale che conserva il token e permette ad Angular di sapere quando cambia.

Perché è importante?

Perché adesso non dobbiamo più limitarci a leggere il token: possiamo anche osservare quando viene modificato.
4. La seconda riga: isLoggedIn
Avete aggiunto:
readonly isLoggedIn = computed(() => this._token() !== null);


Questa riga serve a capire se l'utente è loggato, senza dover ogni volta controllare direttamente il sessionStorage.

La dividiamo.

A. computed()

Significa: crea un valore che viene calcolato automaticamente in base a un altro valore.

Nel nostro caso dipende da _token.

B. this._token()

Attenzione! Quando utilizzi un Signal, per leggerne il contenuto devi mettere le parentesi tonde.

Quindi:
this._token()


significa: leggi il contenuto del Signal _token.

C. !== null

Significa: verifica che il token non sia uguale a null.

Quindi la riga completa dice:

"Controlla se _token contiene qualcosa. Se non è null, significa che l'utente è loggato."

Facciamo qualche esempio:
Contenuto di _token	isLoggedIn()
null	false
"abc123"	true
"xyz456"	true

La cosa bella è che non dobbiamo aggiornare manualmente isLoggedIn.

Si aggiorna da solo quando cambia _token.
Esempio pratico
PRIMA DEL LOGIN
_token
null

isLoggedIn
false



DOPO IL LOGIN
_token
abc123

isLoggedIn
true



Quindi abbiamo due informazioni diverse:
- _token contiene il token vero e proprio.
- isLoggedIn ci dice semplicemente se abbiamo un token oppure no.
Perché abbiamo fatto questa distinzione? Perché alla Navbar interessa sapere se l'utente è loggato, non necessariamente conoscere il contenuto del token.

5. La modifica dentro login()
Prima avevi:
tap(response => {
  if (response && response.token) {
    sessionStorage.setItem('authToken', response.token);
  }
})


Adesso avete aggiunto:
this._token.set(response.token);


Quindi il codice diventa:
tap(response => {
  if (response && response.token) {
    sessionStorage.setItem('authToken', response.token);
    this._token.set(response.token);
  }
})


Qui succede una cosa importante.

Ricordiamoci che login() comunica con il backend per effettuare l'accesso.

Quando il backend risponde correttamente, restituisce un token.

A questo punto facciamo due operazioni.

Prima operazione:
sessionStorage.setItem('authToken', response.token);


Salviamo il token nel browser.

Perché? Per conservarlo anche quando aggiorni la pagina con F5, nella stessa scheda.

Seconda operazione:
this._token.set(response.token);


Aggiorniamo il nostro Signal.

Perché? Per comunicare ad Angular che il token è cambiato.

E di conseguenza isLoggedIn diventa true.

La Navbar, che osserva isLoggedIn, può aggiornarsi mostrando Profilo e Logout.
Cosa succede quando fai login
1. Inserisci email e password.
2. Il backend verifica le credenziali e restituisce il token.
3. Angular salva il token nel sessionStorage.
4. Angular aggiorna _token con .set().
5. isLoggedIn passa a true.
6. La Navbar si aggiorna automaticamente.

Quindi perché abbiamo aggiunto .set()?

Perché prima salvavamo il token soltanto nel browser. Adesso aggiorniamo anche il nostro stato reattivo, che permette alla Navbar di sapere immediatamente che è avvenuto il login.

6. La modifica dentro logout()
Prima avevi:
logout(): void {
  if (typeof window !== 'undefined' && window.sessionStorage) {
    sessionStorage.removeItem('authToken');
  }
}


Adesso:
logout(): void {
  if (typeof window !== 'undefined' && window.sessionStorage) {
    sessionStorage.removeItem('authToken');
  }

  this._token.set(null);
}


Anche qui abbiamo aggiunto una sola riga:
this._token.set(null);


Perché?

Perché quando fai logout dobbiamo fare due cose:
1. Eliminare il token dal browser.
2. Comunicare ad Angular che l'utente non è più autenticato.
Quindi .set(null) svuota il Signal.

Di conseguenza:
isLoggedIn()


diventa false.

E la Navbar torna a mostrare Login, nascondendo Profilo, Logout e le eventuali sezioni riservate.

È esattamente il procedimento inverso del login.

7. L'ultima aggiunta: restoreSession()
Avete aggiunto in fondo al servizio:
restoreSession(): void {
  this._token.set(this.getToken());
}


Questa è una modifica molto importante perché risolve un problema diverso da quello del login e si collega al primo problema dell'hydration e dell'SSR (prima invece abbiamo visto il secondo problema dell'aggiornamento dei componenti al cambio di stato)

Ti faccio un esempio.

Immagina che:
1. Entri nel sito.
2. Fai login.
3. Il token viene salvato nel sessionStorage.
4. Aggiorni la pagina con F5.
Cosa succede?

Quando Angular ricomincia, il Signal _token viene creato nuovamente con il valore iniziale:
signal<string | null>(null);


Quindi inizialmente è null, anche se nel browser il token è ancora presente!

Attenzione: il Signal vive nella memoria dell'applicazione, mentre sessionStorage conserva i dati nel browser. Sono due cose diverse.

Ecco perché serve restoreSession().

Guardiamo il codice:
restoreSession(): void {
  this._token.set(this.getToken());
}


Significa:
1. Chiama getToken().
2. getToken() cerca il token nel sessionStorage.
3. Se lo trova, restituisce il token.
4. .set() inserisce quel token nel Signal _token.
Quindi recuperiamo il token precedentemente salvato e ripristiniamo lo stato di autenticazione.
Esempio: aggiorni la pagina con F5
1. IL BROWSER CONSERVA IL TOKEN
sessionStorage = "abc123"



2. ANGULAR RIPARTE
_token = null



3. CHIAMIAMO restoreSession()
getToken() recupera "abc123"



4. RIPRISTINIAMO IL SIGNAL
_token
abc123

isLoggedIn
true