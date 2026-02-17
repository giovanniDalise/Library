using Library.BookService.Core.Application;
using Library.BookService.Core.Domain.Models;
using Library.BookService.Core.Ports;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Book;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Library.BookService.Infrastructure.Adapters
{
    [Route("library")]
    [ApiController]
    public class BookRESTController : ControllerBase
    {
        private readonly BookAppServicePort _bookAppService;
        private readonly ILoggerPort _logger;
        public BookRESTController(
            BookAppServicePort bookAppService,
            ILoggerPort logger)
        {
            _bookAppService = bookAppService;
            _logger = logger;
        }

        // POST /library
        [HttpPost]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<long>> AddBook([FromForm] BookRequest request)
        {
            _logger.Info($"Tentativo di aggiungere un nuovo libro: {request.Title}");

            try
            {
                // Creazione libro senza cover
                var bookDomain = BookDTOMapper.ToDomain(request, coverReference: null);
                string? coverFileName = request.Cover?.FileName;
                Stream? coverStream = request.Cover?.OpenReadStream();

                var createdBook = await _bookAppService.CreateBookAsync(
                    bookDomain, coverStream, coverFileName
                );
                _logger.Info($"Libro aggiunto con id: {createdBook.BookId}");

                return Ok(createdBook.BookId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Errore durante l'aggiunta del libro: {request.Title}", ex);
                return StatusCode(500, "Errore interno del server");
            }
        }

        // PUT /library/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<long>> UpdateBook(long id, [FromBody] BookRequest request)
        {
            _logger.Info($"Tentativo di aggiornare libro ID {id}");

            try
            {
                var bookDomain = BookDTOMapper.ToDomain(request);
                var updatedId = await _bookAppService.UpdateBookAsync(id, bookDomain);

                _logger.Info($"Libro aggiornato ID {updatedId}");
                return Ok(updatedId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Errore durante l'aggiornamento del libro ID {id}", ex);
                return StatusCode(500, "Errore interno del server");
            }
        }

        // DELETE /library/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<long>> DeleteBook(long id)
        {
            _logger.Info($"Called DeleteBook with ID {id}");
            try
            {
                var book = await _bookAppService.GetBookByIdAsync(id);
                if (book == null)
                {
                    _logger.Info($"Book not found with id: {id}");
                    return NotFound();
                }

                // L'AppService si occupa di cancellare dal DB e dallo storage
                var deletedId = await _bookAppService.DeleteBookAsync(id);

                if (deletedId <= 0)
                {
                    _logger.Warn($"Database delete failed for book ID: {id}");
                    return StatusCode(500, "Errore durante eliminazione dal database");
                }

                _logger.Info($"Book deleted successfully with ID {id}");
                return Ok(deletedId);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while deleting book with ID {id}", ex);
                return StatusCode(500, "Internal server error");
            }
        }


        // POST /library/GetBooks
        [HttpPost("GetBooks")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedBookResponse<BookResponse>>> GetBooks(
            [FromBody] BookRequest request,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            _logger.Info($"Chiamata a GetBooks per libro con titolo: {request.Title}");

            if (page < 1)
            {
                _logger.Warn($"Tentativo non valido con Page: {page}");
                return BadRequest(new { error = "Page deve essere >=1" });
            }
            if (pageSize < 1 || pageSize > 10)
            {
                _logger.Warn($"Tentativo non valido con PageSize: {pageSize}");
                return BadRequest(new { error = "Pagesize deve essere tra 1 e 100" });
            }

            try
            {
                var bookDomain = BookDTOMapper.ToDomain(request);
                var (books, totalRecords) = await _bookAppService.GetBooksAsync(bookDomain, page, pageSize);
                var response = new PagedBookResponse<BookResponse>
                {
                    BookResponse = BookDTOMapper.ToResponseList(books),
                    Page = page,
                    PageSize = pageSize,
                    TotalRecords = totalRecords
                };

                _logger.Info($"Trovati {response.TotalRecords} libri per oggetto BookRequest: {request.Title}");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Errore durante la ricerca libri per oggetto BookRequest: {request.Title}", ex);
                return StatusCode(500, "Errore interno del server");
            }
        }
    }
}
