namespace Library.BookService.Core.Ports
{
    public interface MediaStoragePort
    {
        Task<string> SaveAsync(Stream content, string fileName, string contentType, long editorId, IEnumerable<long> authorIds, long? bookId);
        Task<bool> DeleteAsync(string streamPath);

    }
}
