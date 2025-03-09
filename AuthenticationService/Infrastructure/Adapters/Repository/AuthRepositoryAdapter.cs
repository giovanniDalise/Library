using Library.AuthenticationService.Core.Ports;
using Library.AuthenticationService.Infrastructure.Exceptions;
using MySql.Data.MySqlClient;

namespace Library.AuthenticationService.Infrastructure.Adapters.Repository
{
    public class AuthRepositoryAdapter : AuthenticationRepositoryPort
    {
        private readonly string _connectionString;

        public AuthRepositoryAdapter(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthServiceDbConnection");
        }

        public async Task<bool> CheckUserCredentials(string email, string password)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string sql = "SELECT COUNT(1) FROM user WHERE email = @Email AND password = @Password";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Password", password);

                        int count = Convert.ToInt32(await command.ExecuteScalarAsync());
                        return count > 0; // Se il conteggio è > 0, l'utente esiste
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
