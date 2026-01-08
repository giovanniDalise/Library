using Library.UserService.Core.Domain.Models;
using Library.UserService.Core.Ports;
using Library.UserService.Infrastructure.Exceptions;
using Library.Logging.Abstractions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using System.Resources;

namespace Library.UserService.Infrastructure.Adapters
{
    public class UserRepositoryAdapter : UserRepositoryPort
    {
        private readonly string _connectionString;
        private readonly ILoggerPort _logger;

        public UserRepositoryAdapter(IConfiguration configuration, ILoggerPort logger)
        {
            _connectionString = configuration.GetConnectionString("UserServiceDbConnection");
            _logger = logger;
        }

        private static ResourceManager _rm;
        public ResourceManager Rm
        {
            get
            {
                if (_rm == null) _rm = new ResourceManager("Library.UserService.Infrastructure.Queries.Query", Assembly.GetExecutingAssembly());
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
                _logger.Debug("Database connection opened for CreateAsync");

                string query = Rm.GetString("InsertUser");
                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@surname", user.Surname);
                command.Parameters.AddWithValue("@email", user.Email);
                command.Parameters.AddWithValue("@password", user.Password);

                await command.ExecuteNonQueryAsync();
                _logger.Info($"User created successfully: email={user.Email}, id={command.LastInsertedId}");
                return command.LastInsertedId;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error inserting user with email={user.Email}", ex);
                throw new UserRepositoryADOException($"Error inserting user: {ex.Message}", ex);
            }
        }

        public async Task<User> GetByIdAsync(long id)
        {
            _logger.Debug($"GetByIdAsync called for id={id}");
            var user = new User();

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
                        Password = reader.GetString("password"),
                        Role = reader.GetInt32("role")
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

        public async Task<List<User>> FindByTextAsync(string searchText)
        {
            _logger.Debug($"FindByTextAsync called with searchText='{searchText}'");
            var users = new List<User>();

            try
            {
                using var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();
                _logger.Debug("Database connection opened for FindByTextAsync");

                string query = Rm.GetString("SelectUserByText");
                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchText", searchText);

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

                _logger.Info($"FindByTextAsync retrieved {users.Count} users matching '{searchText}'");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error retrieving users matching '{searchText}'", ex);
                throw new UserRepositoryADOException($"Error retrieving users matching '{searchText}': {ex.Message}", ex);
            }

            return users;
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
    }
}
