using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class AdministratorRepository(
        ILogger<AdministratorRepository> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<AdministratorDto>
    {
        public async Task<IEnumerable<AdministratorDto>> GetAllAsync()
        {
            var administrators = new List<AdministratorDto>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Administrators;", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var administrator = new AdministratorDto
                    {
                        AdminID = reader.GetInt32(reader.GetOrdinal("AdminID")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName"))
                    };
                    administrators.Add(administrator);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving administrator list");
            }

            return administrators;
        }

        public async Task<int> InsertAsync(AdministratorDto dto)
        {
            const string query = @"
                INSERT INTO Administrators (EmailAddress, Password, FirstName, LastName)
                VALUES (@EmailAddress, @Password, @FirstName, @LastName);";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmailAddress", dto.EmailAddress);
                command.Parameters.AddWithValue("@Password", dto.Password);
                command.Parameters.AddWithValue("@FirstName", dto.FirstName);
                command.Parameters.AddWithValue("@LastName", dto.LastName);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new administrator");
                return 0;
            }
        }

        public async Task<AdministratorDto?> FindByIdAsync(int id)
        {
            AdministratorDto? administrator = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Administrators WHERE AdminID = @AdminID;", connection);
                command.Parameters.AddWithValue("@AdminID", id);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    administrator = new AdministratorDto
                    {
                        AdminID = reader.GetInt32(reader.GetOrdinal("AdminID")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving administrator {id} by ID");
            }
            return administrator;
        }

        public async Task<int> UpdateAsync(int id, AdministratorDto dto)
        {
            const string query = @"
                UPDATE Administrators
                SET EmailAddress = @EmailAddress,
                    Password = @Password,
                    FirstName = @FirstName,
                    LastName = @LastName
                WHERE AdminID = @AdminID; ";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AdminID", id);
                command.Parameters.AddWithValue("@EmailAddress", dto.EmailAddress);
                command.Parameters.AddWithValue("@Password", dto.Password);
                command.Parameters.AddWithValue("@FirstName", dto.FirstName);
                command.Parameters.AddWithValue("@LastName", dto.LastName);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error updating administrator {id}");
                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string query = @"DELETE FROM Administrators WHERE AdminID = @AdminID;";
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AdminID", id);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error deleting administrator {id}");
                return 0;
            }
        }
    }
}
