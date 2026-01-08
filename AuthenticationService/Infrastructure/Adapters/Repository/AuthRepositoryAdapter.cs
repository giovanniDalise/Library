using Library.AuthenticationService.Core.Ports;
using Library.AuthenticationService.Infrastructure.Exceptions;
using Library.Logging.Abstractions;
using MySql.Data.MySqlClient;

namespace Library.AuthenticationService.Infrastructure.Adapters.Repository
{
    public class AuthRepositoryAdapter : AuthenticationRepositoryPort
    {
        private readonly string _connectionString;
        private readonly IPasswordVerifierPort _passwordVerifier;
        private readonly ILoggerPort _logger;

        public AuthRepositoryAdapter(
            IConfiguration configuration,
            IPasswordVerifierPort passwordVerifier,
            ILoggerPort logger)
        {
            _connectionString = configuration.GetConnectionString("AuthServiceDbConnection");
            _passwordVerifier = passwordVerifier;
            _logger = logger;
        }

        public async Task<bool> CheckUserCredentials(string email, string password)
        {
            _logger.Debug($"Checking user credentials for email: {email}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    _logger.Debug("Opening database connection for credential check");
                    await connection.OpenAsync();

                    string sql = "SELECT password FROM user WHERE email = @Email";
                    //non stiamo decriptando ma precuperiamo l'ash della psw associata alla mail a cui è stato
                    //associato il salt in fase di creazione dell'utenza con la psw. Più sicuro del decrypting.

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        var result = await command.ExecuteScalarAsync();

                        if (result == null)
                        {
                            _logger.Info($"No user found with email: {email}");
                            return false;
                        }

                        string hashedPassword = result.ToString();
                        var verified = _passwordVerifier.Verify(password, hashedPassword);

                        _logger.Info(
                            verified
                                ? $"Password verification succeeded for email: {email}"
                                : $"Password verification failed for email: {email}"
                        );

                        return verified;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Error while checking credentials for email: {email}", e);
                throw new AuthRepositoryADOException($"Error checking user credentials: {e.Message}", e);
            }
        }

        public async Task<string> GetUserRole(string email)
        {
            _logger.Debug($"Retrieving user role for email: {email}");

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    _logger.Debug("Opening database connection for role retrieval");
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

                        _logger.Info($"Role retrieved for email {email}: {result}");

                        return result?.ToString()
                               ?? throw new AuthRepositoryADOException("Role not found.");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Error($"Error while retrieving role for email: {email}", e);
                throw new AuthRepositoryADOException($"Error getting user role by email: {e.Message}", e);
            }
        }
    }
}
