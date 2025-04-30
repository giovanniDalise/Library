using Library.UserService.Core.Domain.Models;
using Library.UserService.Core.Ports;
using Library.UserService.Infrastructure.Exceptions;
using MySql.Data.MySqlClient;
using System.Data;
using System.Reflection;
using System.Resources;

namespace Library.UserService.Infrastructure.Adapters
{
    public class UserRepositoryAdapter: UserRepositoryPort
    {
        private readonly string _connectionString;

        public UserRepositoryAdapter(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("UserServiceDbConnection");
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
            var users = new List<User>();

            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = Rm.GetString("SelectAllUsers");

                    using (var command = new MySqlCommand(query, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
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
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserRepositoryADOException($"Error reading users: {ex.Message}", ex);
            }

            return users;
        }
        public async Task<long> CreateAsync(User user)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = Rm.GetString("InsertUser");

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", user.Name);
                        command.Parameters.AddWithValue("@surname", user.Surname);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@password", user.Password);

                        await command.ExecuteNonQueryAsync();
                        return command.LastInsertedId;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserRepositoryADOException($"Error inserting user: {ex.Message}", ex);
            }
        }
        public async Task<User> GetByIdAsync (long id)
        {
            var user = new User();
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = Rm.GetString("SelectUserById");

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
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
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserRepositoryADOException($"User not found with id: {ex.Message}", ex);
            }      
            return user;
        }
        public async Task<List<User>> FindByTextAsync(string searchText)
        {
            var users = new List<User>();
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = Rm.GetString("SelectUserByText");

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@searchText", searchText);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
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
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserRepositoryADOException($"Error retrieving users matching '{searchText}': {ex.Message}", ex);
            }
            return users;
        }
        public async Task<long> DeleteAsync(long id)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = Rm.GetString("DeleteUserById");

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserRepositoryADOException($"Error deleting user with id {id}: {ex.Message}", ex);
            }
        }
        public async Task<long> UpdateAsync(long id, User user)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    string query = Rm.GetString("UpdateUserById");

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.Parameters.AddWithValue("@name", user.Name);
                        command.Parameters.AddWithValue("@surname", user.Surname);
                        command.Parameters.AddWithValue("@email", user.Email);
                        command.Parameters.AddWithValue("@password", user.Password);
                        command.Parameters.AddWithValue("@role", user.Role);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new UserRepositoryADOException($"Error updating user with id {id}: {ex.Message}", ex);
            }
        }
    }
}
