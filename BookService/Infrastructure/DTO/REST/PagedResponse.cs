namespace Library.BookService.Infrastructure.DTO.REST
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set;} = new List<T>();
        public int TotalRecords { get; set; }
    }
}
