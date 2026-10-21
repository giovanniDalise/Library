EmailNotConfirmedException e InvalidResetTokenException non hanno bisogno del doppio strato. Il motivo è che sono eccezioni di dominio/business, non eccezioni di infrastruttura.

Il doppio strato che hai fatto per AuthRepositoryException/AuthRepositoryADOException ha senso perché:

AuthRepositoryException (Core) → astrazione, il Core la conosce
AuthRepositoryADOException (Infrastructure) → implementazione specifica ADO.NET

Domani potresti cambiare ADO.NET con Dapper o EF — l’eccezione di infrastruttura cambia ma il Core conosce solo AuthRepositoryException. È il pattern esagonale applicato alle eccezioni.

EmailNotConfirmedException e InvalidResetTokenException invece sono errori di regola di business — “l’email non è confermata” e “il token non è valido” sono concetti del dominio, non dell’infrastruttura. Non dipendono da ADO.NET, RabbitMQ o qualsiasi altra tecnologia. Quindi vivono solo nel Core senza un corrispettivo in Infrastructure:

Core/Exceptions/EmailNotConfirmedException.cs  ✅ solo qui
Core/Exceptions/InvalidResetTokenException.cs  ✅ solo qui

La regola generale è:

Eccezioni di infrastruttura (DB, rete, file system) → doppio strato Core/Infrastructure
Eccezioni di business (regole di dominio) → solo Core