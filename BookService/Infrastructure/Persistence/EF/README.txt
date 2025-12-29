Nel BookService (rispetto agli altri microservizi) ho delle entities separate dai modela del core per via dell'EF,
un po come faccio nei dto (request e resoponse per disaccoppiare i modelli del fe 
da quelli del core).

Entity Framework lavora con entity specifiche (BookEntity) che contengono
dettagli di persistenza (annotazioni, chiavi, relazioni).

Le entity (BookEntity) contengono annotazioni specifiche 
EF ([Key], [Column], virtual ICollection, relazioni ecc.).

EF gestisce il tracking delle entità, il caricamento lazy/eager delle relazioni,
e le chiavi generate dal database.

Non puoi semplicemente fare new Book(bookEntity) perché le entity EF non 
sono domain model e contengono logica/pattern che il domain non deve conoscere.

Questo inoltre asseconda l’architettura esagonale che
richiede di separare domain <-> persistence: il domain non deve conoscere EF.
Ma non per questo su tutti i microservizi è necessario separare i models del core 
con delle entity specifiche per gli adapters di persistenza utilizzati come ho fatto 
nel BoookService. 

Il punto centrale
È CORRETTO che un repository adapter usi le entity di dominio
È SBAGLIATO che il dominio dipenda dagli adapter