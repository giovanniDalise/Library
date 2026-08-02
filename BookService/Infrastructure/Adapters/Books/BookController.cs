
using Library.BookService.Core.Ports.Books;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Books;
using Library.BookService.Infrastructure.DTO.REST.Mappers;
using Library.BookService.Infrastructure.exceptions;
using Library.BookService.Infrastructure.REST.Common;
using Library.Logging.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.BookService.Infrastructure.Adapters.Books
{
    [Route("books")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookAppServicePort _bookAppService;
        private readonly ILoggerPort _logger;
        public BookController(
            IBookAppServicePort bookAppService,
            ILoggerPort logger)
        {
            _bookAppService = bookAppService;
            _logger = logger;
        }

        [HttpPost("AddBook")]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<long>> AddBook([FromForm] BookRequest request)
        {
            _logger.Info($"Attempting to add new book: {request.Title}");
            try
            {
                var bookDomain = BookDTOMapper.ToDomain(request, coverReference: null);
                string? coverFileName = request.Cover?.FileName;
                Stream? coverStream = request.Cover?.OpenReadStream();

                var createdBook = await _bookAppService.CreateBookAsync(bookDomain, coverStream, coverFileName);
                _logger.Info($"Book added with ID: {createdBook.Id}");
                return Ok(createdBook.Id);
            }
            catch (BookRepositoryEFException ex)
            {
                _logger.Error($"Validation error while adding book: {request.Title}", ex);
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error while adding book: {request.Title}", ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<long>> UpdateBook(long id, [FromForm] BookRequest request)
        {
            _logger.Info($"Tentativo di aggiornare libro ID {id}");

            try
            {
                var bookDomain = BookDTOMapper.ToDomain(request);
                Stream? coverStream = request.Cover?.OpenReadStream();
                string? coverFileName = request.Cover?.FileName;

                var updatedId = await _bookAppService.UpdateBookAsync(id, bookDomain, coverStream, coverFileName);

                _logger.Info($"Libro aggiornato ID {updatedId}");
                return Ok(updatedId);
            }
            catch (BookRepositoryEFException ex)
            {
                _logger.Error($"Validation error while updating book ID {id}", ex);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error($"Errore durante l'aggiornamento del libro ID {id}", ex);
                return StatusCode(500, "Errore interno del server");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> DeleteBook(long id)
        {
            _logger.Info($"Attempting to delete book | Id: {id}");
            try
            {
                await _bookAppService.DeleteBookAsync(id);
                _logger.Info($"Book successfully deleted | Id: {id}");
                return NoContent();
            }
            catch (BookRepositoryEFException ex)
            {
                _logger.Warn($"Business rule violation while deleting book | Id: {id} | {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.Error($"Error while deleting book | Id: {id}", ex);
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPost("GetBooks")]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResponse<BookResponse>>> GetBooks(
            [FromBody] BookRequest request,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            _logger.Info($"Call to GetBooks");

            var validationError = PaginationValidator.Validate(page, pageSize);
            if (validationError != null)
            {
                _logger.Warn($"Invalid pagination | Page={page} | PageSize={pageSize}");
                return validationError;
            }

            try
            {
                var bookDomain = BookDTOMapper.ToDomain(request);
                var (books, totalRecords) = await _bookAppService.GetBooksAsync(bookDomain, page, pageSize);
                var response = new PagedResponse<BookResponse>
                {
                    Items = BookDTOMapper.ToResponseList(books),
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

        [HttpGet("getBookDetail/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BookDetailResponse>> GetBookDetail(long id)
        {
            _logger.Info($"Chiamata a GetBookDetail() con ID {id}");

            try
            {
                var book = await _bookAppService.GetBookDetailAsync(id);
                if (book == null)
                {
                    _logger.Warn($"Libro non trovato per ID {id}");
                    return NotFound();
                }

                var response = BookDTOMapper.ToResponse(book);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.Error($"Errore durante il recupero del libro ID {id}", ex);
                return StatusCode(500, "Errore interno del server");
            }
        }
    }
}
