namespace Library.UserService.Core.Ports
{
    public interface IPasswordHasherPort
    {
        string Hash(string password);
    }
}
