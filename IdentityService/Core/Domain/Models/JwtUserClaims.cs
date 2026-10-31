using System.Diagnostics.Metrics;

namespace Library.IdentityService.Core.Domain.Models
{
    public record JwtUserClaims(long UserId, string Email, string Role);
}
//Un record dice che quel tipo è un contenitore di dati immutabile, e JwtUserClaims è proprio questo.
//Cosa ti dà il record qui
//Immutabilità: con la sintassi posizionale le proprietà sono init-only. Dopo la creazione nessuno può modificare id, email o ruolo che finiscono nel token, e questo è desiderabile per dati che rappresentano “ciò che è stato autenticato”.
//Concisione: una riga al posto di una classe con tre proprietà e un costruttore.
//Uguaglianza per valore e with: utili in test e in altri contesti, ma qui non servono davvero. Non è un motivo sufficiente da solo.

//Perché il resto del codice usa classi?
//User, Credentials e AuthResult sono modelli che vengono deserializzati dal JSON, popolati dal repository o mappati, quindi hanno setter pubblici e costruttore vuoto. JwtUserClaims invece si crea solo in un punto, con tutti i valori noti, e poi viene solo letto. Un tipo mutabile qui sarebbe una libertà inutile.

//Se preferisci restare sulle classi, l’equivalente moderno è:
//public class JwtUserClaims
//{
//    public required long UserId { get; init; }
//    public required string Email { get; init; }
//    public required string Role { get; init; }
//}

//Stessa immutabilità(init), ma più verboso, e nel punto di chiamata usi l’object initializer invece del costruttore.


//    Con init puoi assegnare i valori:
//-Durante la creazione dell'oggetto.
//- All'interno del costruttore.
//Ma non puoi modificarli successivamente.

//Esempio:
//var claims = new JwtUserClaims
//{
//UserId = 10,
//Email = "test@email.com",
//Role = "Admin"
//};

//// ERRORE: non puoi modificare una proprietà init
//claims.Role = "User";


//Questa è la caratteristica che ti interessa per l'immutabilità.
//2. required: obbliga a inizializzare la proprietà
//Immagina invece di avere:
//public class JwtUserClaims
//{
//    public required long UserId { get; init; }
//    public required string Email { get; init; }
//    public required string Role { get; init; }
//}


//required dice al compilatore:
//Quando crei un oggetto di questo tipo, devi necessariamente assegnare un valore a queste proprietà.

//Per esempio:
//// ERRORE: mancano Email e Role
//var claims = new JwtUserClaims
//{
//    UserId = 10
//};


//Mentre questo è corretto:
//var claims = new JwtUserClaims
//{
//    UserId = 10,
//    Email = "test@email.com",
//    Role = "Admin"
//};


//Attenzione: required non impedisce di modificare successivamente una proprietà. È init a farlo.
//3. record: cosa aggiunge realmente?
//Torniamo alla tua implementazione iniziale:
//public record JwtUserClaims(long UserId, string Email, string Role);


//Questa sintassi si chiama record posizionale.

//Il compilatore genera automaticamente:
//-Le tre proprietà pubbliche con init.
//- Il costruttore che accetta i tre parametri.
//- L'uguaglianza basata sui valori delle proprietà.
//- Il metodo ToString().
//- La possibilità di utilizzare with.
//Per esempio:
//var claims = new JwtUserClaims(10, "test@email.com", "Admin");

//// Crea un nuovo record modificando il ruolo
//var updatedClaims = claims with { Role = "User" };


//Nota: with non modifica l'oggetto originale, ne crea uno nuovo.