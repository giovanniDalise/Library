using Library.Logging.Abstractions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using System.Resources;
using Library.IdentityService.Core.Ports;
using Library.IdentityService.Core.Domain.Models;
using Library.IdentityService.Infrastructure.Exceptions;

namespace Library.IdentityService.Infrastructure.Adapters
{
    public class UserRepositoryAdapter : IUserRepositoryPort
    {
        private readonly string _connectionString;
        private readonly ILoggerPort _logger;

        public UserRepositoryAdapter(IConfiguration configuration, ILoggerPort<UserRepositoryAdapter> logger)
        {
            _connectionString = configuration.GetConnectionString("IdentityServiceDbConnection");
            _logger = logger;
        }

        private static ResourceManager _rm;
        public ResourceManager Rm
        {
            get
            {
                if (_rm == null) _rm = new ResourceManager("Library.IdentityService.Infrastructure.Queries.Query", Assembly.GetExecutingAssembly());
                return _rm;
            }
        }

        public async Task<List<User>> ReadAsync()
        {
            _logger.Debug("ReadAsync called: reading all users");

            var users = new List<User>();

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger.Debug("Database connection opened for ReadAsync");

                string query = Rm.GetString("SelectAllUsers");
                using var command = new MySqlCommand(query, connection);
                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32("user_id"),
                        Name = reader.GetString("name"),
                        Surname = reader.GetString("surname"),
                        Email = reader.GetString("email"),
                        Password = reader.GetString("password"),
                        Role = reader.GetInt32("role")
                    });
                }

                _logger.Info($"ReadAsync completed: {users.Count} users retrieved");
            }
            catch (Exception ex)
            {
                _logger.Error("Error reading users from DB", ex);
                throw new UserRepositoryADOException($"Error reading users: {ex.Message}", ex);
            }

            return users;
        }

        public async Task<long> CreateAsync(User user)
        {
            _logger.Debug($"CreateAsync called for email={user.Email}");

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = Rm.GetString("InsertUser");
                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surname", user.Surname);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@confirmationToken", user.ConfirmationToken);
                command.Parameters.AddWithValue("@tokenExpiresAt", user.TokenExpiresAt);

                await command.ExecuteNonQueryAsync();
                _logger.Info($"User created successfully: email={user.Email}");
                return command.LastInsertedId;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error inserting user with email={user.Email}", ex);
                throw new UserRepositoryADOException($"Error inserting user: {ex.Message}", ex);
            }
        }

        public async Task<User?> GetByIdAsync(long id)
        {
            _logger.Debug($"GetByIdAsync called for id={id}");
            User? user = null;

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger.Debug("Database connection opened for GetByIdAsync");

                string query = Rm.GetString("SelectUserById");
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    user = new User
                    {
                        Id = reader.GetInt32("user_id"),
                        Name = reader.GetString("name"),
                        Surname = reader.GetString("surname"),
                        Email = reader.GetString("email"),
                        PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number")) ? null : reader.GetString("phone_number"),
                        ProfileImageUrl = reader.IsDBNull(reader.GetOrdinal("profile_image_url")) ? null : reader.GetString("profile_image_url"),
                        CreatedAt = reader.GetDateTime("created_at")
                    };
                    _logger.Info($"User retrieved with id={id}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving user with id={id}", ex);
                throw new UserRepositoryADOException($"User not found with id: {ex.Message}", ex);
            }

            return user;
        }

        public async Task<long> DeleteAsync(long id)
        {
            _logger.Debug($"DeleteAsync called for id={id}");

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger.Debug("Database connection opened for DeleteAsync");

                string query = Rm.GetString("DeleteUserById");
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                _logger.Info($"DeleteAsync completed for id={id}, rowsAffected={rowsAffected}");
                return rowsAffected;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error deleting user with id={id}", ex);
                throw new UserRepositoryADOException($"Error deleting user with id {id}: {ex.Message}", ex);
            }
        }

        public async Task<long> UpdateAsync(long id, User user)
        {
            _logger.Debug($"UpdateAsync called for id={id}, email={user.Email}");

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger.Debug("Database connection opened for UpdateAsync");

                string query = Rm.GetString("UpdateUserById");
                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surname", user.Surname);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@password", user.Password);
                command.Parameters.AddWithValue("@role", user.Role);

                var rowsAffected = await command.ExecuteNonQueryAsync();
                _logger.Info($"UpdateAsync completed for id={id}, rowsAffected={rowsAffected}");
                return rowsAffected;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating user with id={id}", ex);
                throw new UserRepositoryADOException($"Error updating user with id {id}: {ex.Message}", ex);
            }
        }
        public async Task<bool> UpdateProfileImageAsync(long userId, string key)
        {
            _logger.Debug($"UpdateProfileImageAsync called for id={userId}");
            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                string query = Rm.GetString("UpdateUserProfileImage");
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@key", key);
                command.Parameters.AddWithValue("@id", userId);

                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error updating profile image for id={userId}", ex);
                throw new UserRepositoryADOException($"Error updating profile image: {ex.Message}", ex);
            }
        }
    }
}
