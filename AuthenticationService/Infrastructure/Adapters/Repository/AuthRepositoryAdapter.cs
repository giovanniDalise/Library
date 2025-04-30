using Library.AuthenticationService.Core.Ports;
using Library.AuthenticationService.Infrastructure.Exceptions;
using MySql.Data.MySqlClient;

namespace Library.AuthenticationService.Infrastructure.Adapters.Repository
{
    public class AuthRepositoryAdapter : AuthenticationRepositoryPort
    {
        private readonly string _connectionString;
        private readonly IPasswordVerifierPort _passwordVerifier;

        public AuthRepositoryAdapter(IConfiguration configuration, IPasswordVerifierPort passwordVerifier)
        {
            _connectionString = configuration.GetConnectionString("AuthServiceDbConnection");
            _passwordVerifier = passwordVerifier;
            _passwordVerifier = passwordVerifier;
        }

        public async Task<bool> CheckUserCredentials(string email, string password)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT password FROM user WHERE email = @Email";
                    //non stiamo decriptando ma precuperiamo l'ash della psw associata alla mail a cui è stato
                    //associato il salt in fase di creazione dell'utenza con la psw. Più sicuro del decrypting.

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        var result = await command.ExecuteScalarAsync();
                        if (result == null) return false;

                        string hashedPassword = result.ToString();
                        return _passwordVerifier.Verify(password, hashedPassword);
                    }
                }
            }
            catch (Exception e)
            {
                throw new AuthRepositoryADOException($"Error checking user credentials: {e.Message}", e);
            }
        }

        public async Task<string> GetUserRole(string email)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = @"
                        SELECT r.name 
                        FROM user u 
                        JOIN role r ON u.role = r.role_id 
                        WHERE u.email = @Email";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        var result = await command.ExecuteScalarAsync();
                        return result?.ToString() ?? throw new AuthRepositoryADOException("Role not found.");
                    }
                }
            }
            catch (Exception e)
            {
                throw new AuthRepositoryADOException($"Error getting user role by email: {e.Message}", e);
            }
        }
    }
}
