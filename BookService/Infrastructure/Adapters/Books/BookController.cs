
using Library.BookService.Core.Ports.Books;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Books;
using Library.BookService.Infrastructure.DTO.REST.Mappers;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.BookService.Infrastructure.Adapters.Books
{
    [Route("library")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly BookAppServicePort _bookAppService;
        private readonly ILoggerPort _logger;
        public BookController(
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
        public async Task<ActionResult<PagedResponse<BookResponse>>> GetBooks(
            [FromBody] BookRequest request,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            _logger.Info($"Call to GetBooks");

            if (page < 1)
            {
                _logger.Warn($"Invalid attempt with Page: {page}");
                return BadRequest(new { error = "Page must be greater than or equal to 1." });
            }
            if (pageSize < 1 || pageSize > 10)
            {
                _logger.Warn($"Invalid attempt with PageSize: {pageSize}");
                return BadRequest(new { error = "PageSize must be between 1 and 10." });
            }

            try
            {
                var bookDomain = BookDTOMapper.ToDomain(request);
                var (books, totalRecords) = await _bookAppService.GetBooksAsync(bookDomain, page, pageSize);
                var response = new PagedResponse<BookResponse>
                {
                    Items = BookDTOMapper.ToResponseList(books),
                    Page = page,
                    PageSize = pageSize,
                    TotalRecords = totalRecords
                };

                _logger.Info($"Founded {response.TotalRecords} books");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error("Error while retrieving books.", ex);
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
