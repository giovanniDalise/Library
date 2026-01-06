# Library.Logging.NLog & Library.Logging.Abstractions

## Scopo

Queste librerie sono state create per gestire il logging nei microservizi seguendo i principi dell’architettura esagonale:

### Library.Logging.Abstractions
- Contiene l’interfaccia `ILoggerPort`.
- Permette al core dei microservizi di usare il logging senza dipendere da una libreria concreta.

### Library.Logging.NLog
- Implementa `ILoggerPort` tramite NLog.
- Gestisce la scrittura dei log su file o console secondo le regole di `NLog.config`.

## Perché
- Mantiene il core indipendente dal provider di log.
- Consente di sostituire facilmente il motore di logging in futuro.
- Permette di riutilizzare lo stesso adapter in tutti i microservizi.

## Come usarlo
- Registrare `NLogAdapter` nel DI dei microservizi:

```csharp
builder.Services.AddSingleton<ILoggerPort>(_ => new NLogAdapter("ServiceName"));
