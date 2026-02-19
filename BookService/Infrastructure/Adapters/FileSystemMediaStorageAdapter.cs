using Library.BookService.Core.Ports;
using Library.Logging.Abstractions;
using System.IO;

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

        public async Task<string> SaveAsync(Stream content, string fileName, string contentType, long? bookId)
        {
            _logger.Info($"SaveAsync - Start | File={fileName} | BookId={bookId}");

            try
            {

                var dirPath = Path.Combine(_basePath, bookId.ToString());
                _logger.Debug($"SaveAsync - Creating directory | Path={dirPath}");
                Directory.CreateDirectory(dirPath);

                var fullPath = Path.Combine(dirPath, fileName);

                _logger.Debug($"SaveAsync - Writing file | Path={fullPath}");

                using var fs = new FileStream(fullPath, FileMode.Create);
                await content.CopyToAsync(fs);

                // Genera path relativo pubblico
                var relativePath = $"/images/{bookId}/{fileName}";
                var publicUrl = $"{_baseUrl.TrimEnd('/')}{relativePath}";

                _logger.Info($"SaveAsync - Completed | Url={publicUrl}");

                return publicUrl;
            }
            catch (Exception ex)
            {
                _logger.Error($"SaveAsync - Error | File={fileName}  | BookId={bookId}", ex);
                throw;
            }
        }

        public Task<bool> DeleteAsync(long bookId)
        {
            try
            {
                var bookDirectory = Path.Combine(_basePath, bookId.ToString());

                if (!Directory.Exists(bookDirectory))
                {
                    _logger.Warn($"Directory not found: {bookDirectory}");
                    return Task.FromResult(false);
                }

                Directory.Delete(bookDirectory, true);

                _logger.Info($"Directory Deleted {bookDirectory}");

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting directory for book {bookId}", ex);
                throw; 
            }
        }

    }
}
