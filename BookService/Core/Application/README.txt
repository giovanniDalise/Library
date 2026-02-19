Perché abbiamo aggiunto il BookAppService?

In precedenza avevamo solo il BookService nel dominio, che si occupava di operazioni CRUD sul libro tramite il repository. Tutta la logica era centrata sul dominio puro: conoscere cosa fare con un libro, ma non come orchestrare altri sistemi esterni (come lo storage delle cover).

Per gestire casi più complessi, abbiamo introdotto il BookAppService. Questo strato ha lo scopo di:

Separare la logica di dominio dalla logica applicativa

Il dominio (BookService) conosce solo le regole di business dei libri.

L’applicazione (BookAppService) conosce come orchestrare le operazioni coinvolgendo più adapter.

Orchestrare altri adapter esterni

Ad esempio, lo storage delle cover (MediaStoragePort).

In questo modo, il controller REST non deve occuparsi di cancellare o salvare file, ma semplicemente chiama l’AppService.
Prima nel BookRESTController chiamavamo il FileSystemMediaStorageAdapter quindi legavamo insieme due adapter ed in più ogni adapter collegato avrebbe dovuto ripetere la logica e richiamare il mediastorage.

Se in futuro aggiungiamo altri adapter (gRPC, RabbitMQ, batch job…), l’AppService diventa il punto unico di orchestrazione: non duplichiamo la logica di gestione dello storage in più posti.

Ridurre la duplicazione del codice e semplificare i controller

Tutti i controller o consumer esterni (REST, Rabbit, ecc.) chiamano l’AppService.

L’AppService coordina dominio + repository + altri adapter esterni.

Schema logico
[Controller REST / gRPC / Rabbit / Batch] 
           │
           ▼
[BookAppService] ──> coordina:
           │        • BookService (logica di dominio)
           │        • BookRepositoryPort
           │        • MediaStoragePort
           │        • Altri adapter esterni
           ▼
[Adapter / Repository / Storage / Logger / …]


In sintesi: il BookAppService serve a centralizzare e orchestrare la logica applicativa, mantenendo il dominio puro e i controller leggeri, pronti a ricevere richieste da diversi canali senza duplicare codice.