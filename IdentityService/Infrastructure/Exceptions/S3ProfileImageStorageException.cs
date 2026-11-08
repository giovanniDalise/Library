using Library.IdentityService.Core.Exceptions;

namespace Library.IdentityService.Infrastructure.Exceptions
{
    public class S3ProfileImageStorageException : ProfileImageStorageException
    {
        public S3ProfileImageStorageException(string message)
            : base(message) { }

        public S3ProfileImageStorageException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}