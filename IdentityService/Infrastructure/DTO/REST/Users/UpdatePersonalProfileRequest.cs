using System.ComponentModel.DataAnnotations;

namespace Library.IdentityService.Infrastructure.DTO.REST.Users
{
    public class UpdatePersonalProfileRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string Surname { get; set; } = "";

        [StringLength(20)]
        [RegularExpression(@"^(\+?[0-9 ]{6,20})?$", ErrorMessage = "Invalid phone number.")] public string? PhoneNumber { get; set; }

        // VALIDAZIONI
        //Con [ApiController] una richiesta non valida riceve da sola un 400,
        //senza codice nel controller.

        //[Required]: rifiuta un campo mancante, vuoto o fatto di soli spazi.
        //Il tuo Name senza attributo non sarebbe necessariamente obbligatorio:
        //nei progetti con Nullable abilitato ASP.NET tratta le stringhe non nullable
        //come obbligatorie da sé, ma non ho visto il.csproj dell’IdentityService.
        //L’attributo esplicito evita di dipendere da questa impostazione.

        //[StringLength(100)] e (20): sono i limiti delle colonne nel DB.Senza,
        //un testo più lungo arriverebbe fino a MySQL e darebbe un errore 500 invece di un 400 chiaro.

        //= "": toglie il warning “non nullable non inizializzato”. Non cambia la validazione,
        //perché una stringa vuota non passa[Required].

        // [RegularExpression]: sul telefono permette un + iniziale, cifre e spazi, da 6 a 20 caratteri.
        // Il gruppo è opzionale: la stringa vuota è ammessa e il service la trasforma in null
        // (nessun telefono). Se vuoi accettare anche trattini o parentesi, allarga il pattern.
        // Quando il campo non viene inviato (null) la regola non scatta.
    }
}
