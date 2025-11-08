using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.DTOs;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class AddressRepository(
        ILogger<AddressRepository> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<AddressDto>
    {
        public async Task<IEnumerable<AddressDto>> GetAllAsync()
        {
            var addresses = new List<AddressDto>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Addresses;", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var address = new AddressDto
                    {
                        AddressID = reader.GetInt32(reader.GetOrdinal("AddressID")),
                        CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        Line1 = reader.GetString(reader.GetOrdinal("Line1")),
                        Line2 = reader.GetString(reader.GetOrdinal("Line2")),
                        City = reader.GetString(reader.GetOrdinal("City")),
                        State = reader.GetString(reader.GetOrdinal("State")),
                        ZipCode = reader.GetString(reader.GetOrdinal("ZipCode")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        Disabled = reader.GetInt32(reader.GetOrdinal("Disabled"))
                    };
                    addresses.Add(address);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving address list");
            }

            return addresses;
        }

        public async Task<int> InsertAsync(AddressDto dto)
        {
            const string query = @"
                INSERT INTO Addresses (CustomerID, Line1, Line2, City, State, ZipCode, Phone, Disabled)
                VALUES (@CustomerID, @Line1, @Line2, @City, @State, @ZipCode, @Phone, @Disabled);";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", dto.CustomerID);
                command.Parameters.AddWithValue("@Line1", dto.Line1);
                command.Parameters.AddWithValue("@Line2", dto.Line2);
                command.Parameters.AddWithValue("@City", dto.City);
                command.Parameters.AddWithValue("@State", dto.State);
                command.Parameters.AddWithValue("@ZipCode", dto.ZipCode);
                command.Parameters.AddWithValue("@Phone", dto.Phone);
                command.Parameters.AddWithValue("@Disabled", dto.Disabled);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new address");
                return 0;
            }
        }

        public async Task<AddressDto?> FindByIdAsync(int id)
        {
            AddressDto? address = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Addresses WHERE AddressID = @AddressID;", connection);
                command.Parameters.AddWithValue("@AddressID", id);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    address = new AddressDto
                    {
                        AddressID = reader.GetInt32(reader.GetOrdinal("AddressID")),
                        CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        Line1 = reader.GetString(reader.GetOrdinal("Line1")),
                        Line2 = reader.GetString(reader.GetOrdinal("Line2")),
                        City = reader.GetString(reader.GetOrdinal("City")),
                        State = reader.GetString(reader.GetOrdinal("State")),
                        ZipCode = reader.GetString(reader.GetOrdinal("ZipCode")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        Disabled = reader.GetInt32(reader.GetOrdinal("Disabled"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving address {id} by ID");
            }
            return address;
        }

        public async Task<int> UpdateAsync(int id, AddressDto dto)
        {
            const string query = @"
                UPDATE Addresses
                SET CustomerID = @CustomerID,
                    Line1 = @Line1,
                    Line2 = @Line2,
                    City = @City,
                    State = @State,
                    ZipCode = @ZipCode,
                    Phone = @Phone,
                    Disabled = @Disabled
                WHERE AddressID = @AddressID; ";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AddressID", id);
                command.Parameters.AddWithValue("@CustomerID", dto.CustomerID);
                command.Parameters.AddWithValue("@Line1", dto.Line1);
                command.Parameters.AddWithValue("@Line2", dto.Line2);
                command.Parameters.AddWithValue("@City", dto.City);
                command.Parameters.AddWithValue("@State", dto.State);
                command.Parameters.AddWithValue("@ZipCode", dto.ZipCode);
                command.Parameters.AddWithValue("@Phone", dto.Phone);
                command.Parameters.AddWithValue("@Disabled", dto.Disabled);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error updating address {id}");
                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string query = @"DELETE FROM Addresses WHERE AddressID = @AddressID;";
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AddressID", id);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error deleting address {id}");
                return 0;
            }
        }
    }
}
