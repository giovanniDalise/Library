namespace Library.IdentityService.Core.Exceptions
{
    public class InvalidResetTokenException:Exception
    {
        public InvalidResetTokenException(string message) : base(message) { }

    }
}
