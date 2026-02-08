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

        public async Task<bool> DeleteAsync(string streamPath)
        {
            _logger.Info($"DeleteAsync {streamPath}");
            try
            {
                if (string.IsNullOrWhiteSpace(streamPath))
                {
                    _logger.Warn("DeleteAsync - StreamPath is null or empty");
                    return false;
                }

                // Trova la parte dopo /images/ nell'URL, per capire da dove comincia la parte “relativa” del file sul disco.
                var uri = new Uri(streamPath);
                var imagesIndex = uri.AbsolutePath.IndexOf("/images/", StringComparison.OrdinalIgnoreCase);

                if(imagesIndex < 0)
                {
                    _logger.Warn($"DeleteAsync - invalid URL, cannot find /images/ in {streamPath}");
                    return false;
                }
                // Estrae la parte del path relativa a /images/  e converte / in \ su Windows es. images\1\3\1\file.jpg
                var relativePath = uri.AbsolutePath.Substring(imagesIndex + 1).Replace('/', Path.DirectorySeparatorChar);
                
                // Combina _basePath(cartella base fisica sul disco) con il path relativo del file e rimuove images\
                var fullPath = Path.Combine(_basePath, relativePath.Substring("images".Length + 1));

                _logger.Debug($"DeleteAsync - fullPath = {fullPath}");

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.Info($"DeleteAsync - File Deleted {fullPath}");
                }
                else
                {
                    _logger.Warn($"DeleteAsync - File not found: {fullPath}");
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"DeleteAsync - Error deleting file: {streamPath}", ex);
                throw;
            }
        }

    }
}
