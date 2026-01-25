namespace Library.BookService.Infrastructure.DTO.REST.Book
{
    public class PagedBookResponse
    {
        public IEnumerable<BookResponse> BookResponse { get; set;} = new List<BookResponse>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
    }
}
