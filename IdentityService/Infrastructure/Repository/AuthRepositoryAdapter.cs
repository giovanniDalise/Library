using Library.IdentityService.Core.Domain.Models;
using Library.IdentityService.Core.Ports;
using Library.IdentityService.Infrastructure.Exceptions;
using Library.Logging.Abstractions;
using MySql.Data.MySqlClient;
using System.Data;

namespace Library.IdentityService.Infrastructure.Adapters.Repository
{
    public class AuthRepositoryAdapter : IAuthenticationRepositoryPort
    {
        private readonly string _connectionString;
        private readonly IPasswordVerifierPort _passwordVerifier;
        private readonly ILoggerPort _logger;

        public AuthRepositoryAdapter(
            IConfiguration configuration,
            IPasswordVerifierPort passwordVerifier,
            ILoggerPort logger)
        {
            _connectionString = configuration.GetConnectionString("IdentityServiceDbConnection");
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
        public async Task<bool> ConfirmUserAsync(string token)
        {
            _logger.Debug($"ConfirmUserAsync called for token={token}");
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = "UPDATE user SET is_confirmed = TRUE, confirmation_token = NULL, token_expires_at = NULL WHERE confirmation_token = @token AND token_expires_at > NOW() AND is_confirmed = FALSE";
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@token", token);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0; // true = confermato, false = token non valido o scaduto
            }
            catch (Exception ex)
            {
                _logger.Error($"Error confirming user with token={token}", ex);
                throw new AuthRepositoryADOException($"Error confirming user: {ex.Message}", ex);
            }
        }
        public async Task<(bool IsConfirmed, string? Token, DateTime? ExpiresAt)> GetUserConfirmationInfoAsync(string email)
        {
            _logger.Debug($"GetUserConfirmationInfoAsync for email: {email}");
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = "SELECT is_confirmed, confirmation_token, token_expires_at FROM user WHERE email = @Email";
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", email);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return (false, null, null);

                return (
                    reader.GetBoolean("is_confirmed"),
                    reader.IsDBNull(reader.GetOrdinal("confirmation_token")) ? null : reader.GetString("confirmation_token"),
                    reader.IsDBNull(reader.GetOrdinal("token_expires_at")) ? null : reader.GetDateTime("token_expires_at")
                );
            }
            catch (Exception e)
            {
                _logger.Error($"GetUserConfirmationInfoAsync error for email: {email}", e);
                throw new AuthRepositoryADOException($"Error getting confirmation info: {e.Message}", e);
            }
        }

        public async Task UpdateConfirmationTokenAsync(string email, string token, DateTime expiresAt)
        {
            _logger.Debug($"UpdateConfirmationTokenAsync for email: {email}");
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = "UPDATE user SET confirmation_token = @token, token_expires_at = @expiresAt WHERE email = @Email";
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@token", token);
                command.Parameters.AddWithValue("@expiresAt", expiresAt);
                command.Parameters.AddWithValue("@Email", email);

                await command.ExecuteNonQueryAsync();
                _logger.Info($"UpdateConfirmationTokenAsync completed for email: {email}");
            }
            catch (Exception e)
            {
                _logger.Error($"UpdateConfirmationTokenAsync error for email: {email}", e);
                throw new AuthRepositoryADOException($"Error updating confirmation token: {e.Message}", e);
            }
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            _logger.Debug($"GetByEmailAsync for email: {email}");
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = "SELECT user_id, name, surname, email FROM user WHERE email = @Email";
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", email);

                using var reader = await command.ExecuteReaderAsync();

                // se non trova l'utente restituisce null
                if (!await reader.ReadAsync()) return null;

                return new User
                {
                    Id = reader.GetInt64("user_id"),
                    Name = reader.GetString("name"),
                    Surname = reader.GetString("surname"),
                    Email = reader.GetString("email")
                };
            }
            catch (Exception e)
            {
                _logger.Error($"GetByEmailAsync error for email: {email}", e);
                throw new AuthRepositoryADOException($"Error getting user by email: {e.Message}", e);
            }
        }
        public async Task UpdateResetTokenAsync(string email, string resetToken, DateTime resetTokenExpiry)
        {
            _logger.Info($"UpdateResetTokenAsync for email: {email}");
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                // using viene utilizzato per il rilascio automatico della risorsa(in questo caso la connessione al db)
                // quando non viene più utilizzata (ossia quando usciamo dallo scope del metodo o quando questo va in eccezione)
                await connection.OpenAsync();

                string sql = "Update user SET reset_token = @token, reset_token_expires_at = @resetTokenExpiry where email = @email";
                using var command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@token", resetToken);
                command.Parameters.AddWithValue("@resetTokenExpiry", resetTokenExpiry);
                command.Parameters.AddWithValue("@email", email);

                await command.ExecuteNonQueryAsync();

                _logger.Info($"UpdateResetTokenAsync completed for email: {email}");

            }
            catch (Exception ex)
            {
                _logger.Error($"UpdateResetTokenAsync for email: {email}", ex);
                throw new AuthRepositoryADOException($"Error updating reset token {ex.Message}", ex);
            }
        }
        public async Task<(User? User, DateTime? ResetTokenExpiresAt)> GetUserByResetTokenAsync(string token)
        {
            _logger.Info($"GetUserByResetTokenAsync");
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                string sql = "Select name, surname, email, user_id, reset_token_expires_at from user where reset_token = @token";
                using var command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@token", token);

                using var reader = await command.ExecuteReaderAsync();
                if (!await reader.ReadAsync()) return (null, null);

                return 
                (
                    new User()
                    {
                        Id = reader.GetInt64("user_id"),
                        Name = reader.GetString("name"),
                        Surname = reader.GetString("surname"),
                        Email = reader.GetString("email")
                    },
                    reader.IsDBNull(reader.GetOrdinal("reset_token_expires_at")) ? null : reader.GetDateTime("reset_token_expires_at")
                    //Con GetOrdinal prima si prende il numero della colonna con quel nome,
                    // successivamente vede se il campo di quella colonna è vuoto nel caso lo sia restituisce
                    // null altrimenti fa il GetDateTime per prendersi il valore in DateTime
                );
            }
            catch (Exception ex)
            {
                _logger.Error($"GetUserByResetTokenAsync", ex);
                throw new AuthRepositoryADOException($"Error getting user by reset token {ex.Message}", ex);
            }
        }

        public async Task UpdateForgottenPasswordAsync(long? userId, string hashedPassword)
        {
            if (userId == null)
                throw new AuthRepositoryADOException("UserId cannot be null");

            _logger.Info($"UpdateForgottenPasswordAsync - Start | UserId: {userId}");

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 1 — aggiorna password
                string updatePasswordSql = "UPDATE user SET password = @password WHERE user_id = @userId";
                using var updatePasswordCmd = new MySqlCommand(updatePasswordSql, connection, transaction);
                updatePasswordCmd.Parameters.AddWithValue("@password", hashedPassword);
                updatePasswordCmd.Parameters.AddWithValue("@userId", userId);
                await updatePasswordCmd.ExecuteNonQueryAsync();

                // 2 — pulisci token
                string clearTokenSql = "UPDATE user SET reset_token = NULL, reset_token_expires_at = NULL WHERE user_id = @userId";
                using var clearTokenCmd = new MySqlCommand(clearTokenSql, connection, transaction);
                clearTokenCmd.Parameters.AddWithValue("@userId", userId);
                await clearTokenCmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
                _logger.Info($"UpdateForgottenPasswordAsync - Completed | UserId: {userId}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.Error($"UpdateForgottenPasswordAsync - Error | UserId: {userId}", ex);
                throw new AuthRepositoryADOException("Error updating forgotten password", ex);
            }
        }
    }
}
