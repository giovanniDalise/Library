namespace Library.IdentityService.Core.Ports
{
    public interface IPasswordHasherPort
    {
        string Hash(string password);
    }
}
