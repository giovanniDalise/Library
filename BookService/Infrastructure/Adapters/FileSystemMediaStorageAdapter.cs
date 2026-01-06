using Library.BookService.Core.Ports;
using Library.Logging.Abstractions;

namespace Library.BookService.Infrastructure.Adapters
{
    public class FileSystemMediaStorageAdapter : MediaStoragePort
    {
        private readonly string _basePath;
        private readonly string _baseUrl;
        private readonly ILoggerPort _logger;

        public FileSystemMediaStorageAdapter(
            IConfiguration configuration,
            ILoggerPort logger)
        {
            _logger = logger;

            _basePath = configuration["Media:BasePath"]
                ?? throw new ArgumentNullException("Media:BasePath not configured");

            _baseUrl = configuration["Media:BaseUrl"]
                ?? throw new ArgumentNullException("Media:BaseUrl not configured");
        }

        public async Task<string> SaveAsync(Stream content, string fileName, string contentType, long editorId, IEnumerable<long> authorIds, long? bookId)
        {
            _logger.Info($"SaveAsync - Start | File={fileName} | EditorId={editorId} | BookId={bookId}");

            try
            {
                var authorsFolder = authorIds.Count() == 1 ? authorIds.First().ToString() : string.Join("-", authorIds);

                var dirPath = Path.Combine(_basePath, editorId.ToString(), authorsFolder, bookId.ToString());
                _logger.Debug($"SaveAsync - Creating directory | Path={dirPath}");
                Directory.CreateDirectory(dirPath);

                var fullPath = Path.Combine(dirPath, fileName);

                _logger.Debug($"SaveAsync - Writing file | Path={fullPath}");

                using var fs = new FileStream(fullPath, FileMode.Create);
                await content.CopyToAsync(fs);

                // Genera path relativo pubblico
                var relativePath = $"/images/{editorId}/{authorsFolder}/{bookId}/{fileName}";
                var publicUrl = $"{_baseUrl.TrimEnd('/')}{relativePath}";

                _logger.Info($"SaveAsync - Completed | Url={publicUrl}");

                return publicUrl;
            }
            catch (Exception ex)
            {
                _logger.Error($"SaveAsync - Error | File={fileName} | EditorId={editorId} | BookId={bookId}", ex);
                throw;
            }
        }
    }
}
