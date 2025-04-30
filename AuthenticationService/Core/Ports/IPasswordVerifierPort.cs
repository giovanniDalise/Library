namespace Library.AuthenticationService.Core.Ports
{
    public interface IPasswordVerifierPort
    {
        bool Verify(string plainPassword, string hashedPassword);
    }
}
