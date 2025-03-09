namespace Library.AuthenticationService.Core.Domain.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }

        public AuthResponse( string token)
        {
            this.Token = token;
        }
    }
}
