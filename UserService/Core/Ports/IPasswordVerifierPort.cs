namespace Library.IdentityService.Core.Ports
{
    public interface IPasswordVerifierPort
    {
        bool Verify(string plainPassword, string hashedPassword);
    }
}
