namespace Library.IdentityService.Infrastructure.Adapters.Storage
{
    public class StorageOptions
    {
        public string ServiceUrl { get; set; } = "";
        public string AccessKeyId { get; set; } = "";
        public string SecretAccessKey { get; set; } = "";
        public string BucketName { get; set; } = "";
        public int UrlExpirationMinutes { get; set; } = 15;
    }
}
