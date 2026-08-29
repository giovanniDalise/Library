using Microsoft.AspNetCore.Mvc;

namespace Library.BookService.Infrastructure.REST.Common
{
    public static class PaginationValidator
    {
        // Una classe statica non mantiene uno stato (non creiamo istanze della classe ne abbiamo proprietà (non statiche) della classe
        // richiamabili con il this) e offre solo funzionalità. Non avendo istanze la chiamiamo cosi "PaginationValidator."
        // Una classe statica non può essere registrata nel container DI. Niente Dipendency Injection.
        // I vantaggi sono che non crea oggetti inutili allocondo memoria e una classe statica ci fa capire che non avendo stato
        // non avremo diverse istanze dello stessa classe. Sarà sempre uguale.
        public static ActionResult? Validate(int page, int pageSize, int maxPageSize = 10)
        {
            if (page < 1)
                return new BadRequestObjectResult(new { error = "Page must be greater than or equal to 1." });

            if (pageSize < 1 || pageSize > maxPageSize)
                return new BadRequestObjectResult(new { error = $"PageSize must be between 1 and {maxPageSize}." });

            return null;
        }
        // Una classe statica puo avere solo metodi statici mentre una classe non statica può avere sia metodi static che non static.
        // Un metodo statico in una classe non static non può chiamare le proprietà della classe (come il this) dato che appartengono all'instanza
        // della classe ed uscirebbero dallo scope static
    }
}