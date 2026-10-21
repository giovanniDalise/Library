Sulle eccezioni personalizzate vs throw new Exception

Hai ragione che tecnicamente funzionerebbe anche throw new Exception("messaggio") — ma le eccezioni personalizzate ti danno tre vantaggi concreti:

Catch selettivo — nel controller puoi fare:

csharp
catch (InvalidResetTokenException ex) 
    → 400 BadRequest
catch (AuthRepositoryADOException ex) 
    → 500 Internal Server Error
catch (Exception ex) 
    → 500 generico

Con Exception generica non puoi distinguere un errore di business da un errore di infrastruttura — restituiresti sempre 500 anche quando dovresti restituire 400.

Leggibilità — throw new InvalidResetTokenException("Token expired") è autoesplicativo. Chi legge il codice capisce immediatamente che è un errore di business, non un crash inaspettato.

Strato di astrazione — AuthRepositoryADOException estende AuthRepositoryException del Core. Domani cambi ADO.NET con Dapper — il Core non sa nulla della tecnologia usata, conosce solo AuthRepositoryException. Con Exception generica perderesti questa separazione.