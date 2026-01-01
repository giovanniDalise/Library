using Library.BookService.Core.Ports;

namespace Library.BookService.Infrastructure.Adapters
{
    public class FileSystemMediaStorageAdapter : MediaStoragePort
    {
        private readonly string _basePath;
        private readonly string _baseUrl;
        public FileSystemMediaStorageAdapter(IConfiguration configuration)
        {
            _basePath = configuration["Media:BasePath"]
                        ?? throw new ArgumentNullException("BasePath not configured");

            // Base URL completa per il front-end
            _baseUrl = configuration["Media:BaseUrl"]
                       ?? throw new ArgumentNullException("BaseUrl not configured");
        }

        public async Task<string> SaveAsync(Stream content, string fileName, string contentType, long editorId, IEnumerable<long> authorIds, long? bookId)
        {
            var authorsFolder = authorIds.Count() == 1? authorIds.First().ToString(): string.Join("-", authorIds);

            var dirPath = Path.Combine(_basePath, editorId.ToString(), authorsFolder, bookId.ToString());
            Directory.CreateDirectory(dirPath); 

            var fullPath = Path.Combine(dirPath, fileName);

            using var fs = new FileStream(fullPath, FileMode.Create);
            await content.CopyToAsync(fs);

            // Genera path relativo pubblico
            var relativePath = $"/images/{editorId}/{authorsFolder}/{bookId}/{fileName}";

            return $"{_baseUrl.TrimEnd('/')}{relativePath}";
        }
    }
}
