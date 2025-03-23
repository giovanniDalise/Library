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
                throw new UserRepositoryADOException($"Error checking user credentials: {ex.Message}", ex);
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
    }
}
