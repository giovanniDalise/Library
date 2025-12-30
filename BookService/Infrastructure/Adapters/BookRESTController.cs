using Library.BookService.Core.Ports;
using Library.BookService.Infrastructure.DTO.REST;
using Library.BookService.Infrastructure.DTO.REST.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.BookService.Infrastructure.Adapters
{
    [Route("library")]
    [ApiController]
    public class BookRESTController : ControllerBase
    {
        private readonly BookServicePort _bookService;
        private readonly MediaStoragePort _mediaStorage;

        public BookRESTController(BookServicePort bookService, MediaStoragePort mediaStorage)
        {
            _bookService = bookService;
            _mediaStorage = mediaStorage;
        }

        // GET /library
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<BookResponse>>> GetBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            var response = BookDTOMapper.ToResponseList(books);
            return Ok(response);
        }

        // GET /library/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<BookResponse>> GetBookById(long id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            var response = BookDTOMapper.ToResponse(book);
            return Ok(response);
        }

        // POST /library
        [HttpPost]
        [Authorize(Roles = "admin")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<long>> AddBook([FromForm] BookRequest request)
        {
            // 1️⃣ Crei il dominio senza cover
            var bookDomain = BookDTOMapper.ToDomain(request, coverReference: null);

            // 2️⃣ Salvi il libro nel DB (gli ID vengono generati e restituiti)
            var createdBook = await _bookService.CreateBookAsync(bookDomain);

            string? coverUrl = null;

            // 3️⃣ Se è presente la cover, salvala sul filesystem usando gli ID effettivi
            if (request.Cover is not null)
            {
                using var stream = request.Cover.OpenReadStream();

                var editorId = createdBook.Editor.Id; // ID reale dell'editor
                var authorIds = createdBook.Authors.Select(a => a.Id); // ID reali degli autori
                var bookId = createdBook.BookId.Value; // .Value perché BookId è nullable

                coverUrl = await _mediaStorage.SaveAsync(
                    stream,
                    request.Cover.FileName,
                    request.Cover.ContentType,
                    editorId,
                    authorIds,
                    bookId
                );

                // 4️⃣ Aggiorna il libro con la cover
                createdBook.CoverReference = coverUrl;
                await _bookService.UpdateBookAsync(bookId, createdBook);
            }

            return Ok(createdBook.BookId);
        }



        // PUT /library/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<long>> UpdateBook(long id, [FromBody] BookRequest request)
        {
            var bookDomain = BookDTOMapper.ToDomain(request);
            var updatedId = await _bookService.UpdateBookAsync(id, bookDomain);
            return Ok(updatedId);
        }

        // DELETE /library/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<long>> DeleteBook(long id)
        {
            return Ok(await _bookService.DeleteBookAsync(id));
        }

        // GET /library/findByString?param=...
        [HttpGet("findByString")]
        [AllowAnonymous]
        public async Task<ActionResult<List<BookResponse>>> FindBooksByString([FromQuery] string param)
        {
            var books = await _bookService.GetBooksByTextAsync(param);
            var response = BookDTOMapper.ToResponseList(books);
            return Ok(response);
        }

        // POST /library/findByBook
        [HttpPost("findByBook")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<BookResponse>>> FindBooksByBook([FromBody] BookRequest request)
        {
            var bookDomain = BookDTOMapper.ToDomain(request);
            var books = await _bookService.GetBooksByObjectAsync(bookDomain);
            var response = BookDTOMapper.ToResponseList(books);
            return Ok(response);
        }
    }
}
