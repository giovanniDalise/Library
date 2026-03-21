namespace Library.BookService.Core.Ports
{
    public interface IMediaStoragePort
    {
        Task<string> SaveAsync(Stream content, string fileName, string contentType, long? bookId);
        Task<bool> DeleteAsync(long bookId);

    }
}
