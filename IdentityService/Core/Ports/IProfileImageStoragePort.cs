namespace Library.IdentityService.Core.Ports
{
    public interface IProfileImageStoragePort
    {
        Task SaveAsync(string key, Stream content, string contentType);
        Task DeleteAsync(string key);
        string GetImageUrl(string key);
    }
}
