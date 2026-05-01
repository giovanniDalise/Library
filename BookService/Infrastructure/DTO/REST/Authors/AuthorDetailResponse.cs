namespace Library.BookService.Infrastructure.DTO.REST.Authors
{
    public class AuthorDetailResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public PagedResponse<string> Books { get; set; }
    }
}
