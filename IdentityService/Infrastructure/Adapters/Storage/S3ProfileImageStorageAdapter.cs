using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Library.IdentityService.Core.Exceptions;
using Library.IdentityService.Core.Ports;
using Library.IdentityService.Infrastructure.Exceptions;
using Library.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Library.IdentityService.Infrastructure.Adapters.Storage
{
    public class S3ProfileImageStorageAdapter : IProfileImageStoragePort
    {
        private readonly IAmazonS3 _s3;
        private readonly StorageOptions _options;
        private readonly ILoggerPort _logger;

        public S3ProfileImageStorageAdapter(
            IOptions<StorageOptions> options,
            ILoggerPort<S3ProfileImageStorageAdapter> logger)
        {
            _options = options.Value;
            _logger = logger;

            _s3 = new AmazonS3Client(
                new BasicAWSCredentials(_options.AccessKeyId, _options.SecretAccessKey),
                new AmazonS3Config
                {
                    ServiceURL = _options.ServiceUrl,
                    AuthenticationRegion = "auto",
                    ForcePathStyle = false
                });
        }

        public async Task SaveAsync(string key, Stream content, string contentType)
        {
            try
            {
                await _s3.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = _options.BucketName,
                    Key = key,
                    InputStream = content,
                    ContentType = contentType
                });
                _logger.Info($"Profile image saved: {key}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error saving profile image {key}", ex);
                throw new S3ProfileImageStorageException($"Error saving image: {ex.Message}", ex);
            }
        }

        public async Task DeleteAsync(string key)
        {
            try
            {
                await _s3.DeleteObjectAsync(_options.BucketName, key);
                _logger.Info($"Profile image deleted: {key}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting profile image {key}", ex);   // best effort, non blocca
            }
        }

        public string GetImageUrl(string key)
            => _s3.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = key,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(_options.UrlExpirationMinutes)
            });
    }
}