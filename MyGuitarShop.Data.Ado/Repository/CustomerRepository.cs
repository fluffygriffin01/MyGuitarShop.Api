using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class CustomerRepository(
        ILogger<CustomerRepository> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<CustomerEntity, CustomerDto>
    {
        public async Task<IEnumerable<CustomerEntity>> GetAllAsync()
        {
            var customers = new List<CustomerEntity>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Customers;", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var customer = new CustomerEntity
                    {
                        CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        ShippingAddressID = reader.IsDBNull(reader.GetOrdinal("ShippingAddressID")) ? null : reader.GetInt32(reader.GetOrdinal("ShippingAddressID")),
                        BillingAddressID = reader.IsDBNull(reader.GetOrdinal("BillingAddressID")) ? null : reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                    customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving customer list");
            }

            return customers;
        }

        public async Task<CustomerEntity?> FindByIdAsync(string id)
        {
            CustomerEntity? customer = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Customers WHERE CustomerID = @CustomerID;", connection);
                command.Parameters.AddWithValue("@CustomerID", id);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    customer = new CustomerEntity
                    {
                        CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        ShippingAddressID = reader.IsDBNull(reader.GetOrdinal("ShippingAddressID")) ? null : reader.GetInt32(reader.GetOrdinal("ShippingAddressID")),
                        BillingAddressID = reader.IsDBNull(reader.GetOrdinal("BillingAddressID")) ? null : reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving customer {id} by ID");
            }
            return customer;
        }

        public async Task<CustomerEntity?> FindByUniqueAsync(string emailAddress)
        {
            CustomerEntity? customer = null;
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Customers WHERE EmailAddress = @EmailAddress;", connection);
                command.Parameters.AddWithValue("@EmailAddress", emailAddress);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    customer = new CustomerEntity
                    {
                        CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        EmailAddress = reader.GetString(reader.GetOrdinal("EmailAddress")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        LastName = reader.GetString(reader.GetOrdinal("LastName")),
                        ShippingAddressID = reader.IsDBNull(reader.GetOrdinal("ShippingAddressID")) ? null : reader.GetInt32(reader.GetOrdinal("ShippingAddressID")),
                        BillingAddressID = reader.IsDBNull(reader.GetOrdinal("BillingAddressID")) ? null : reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving customer with email address {emailAddress}");
            }
            return customer;
        }

        public async Task<bool> InsertAsync(CustomerDto dto)
        {
            const string query = @"
                INSERT INTO Customers (EmailAddress, Password, FirstName, LastName, ShippingAddressID, BillingAddressID)
                VALUES (@EmailAddress, @Password, @FirstName, @LastName, @ShippingAddressID, @BillingAddressID);";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmailAddress", dto.EmailAddress);
                command.Parameters.AddWithValue("@Password", dto.Password);
                command.Parameters.AddWithValue("@FirstName", dto.FirstName);
                command.Parameters.AddWithValue("@LastName", dto.LastName);
                command.Parameters.AddWithValue("@ShippingAddressID", dto.ShippingAddressID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@BillingAddressID", dto.BillingAddressID ?? (object)DBNull.Value);

                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new customer");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(string id, CustomerDto dto)
        {
            const string query = @"
                UPDATE Customers
                SET EmailAddress = @EmailAddress,
                    Password = @Password,
                    FirstName = @FirstName,
                    LastName = @LastName,
                    ShippingAddressID = @ShippingAddressID,
                    BillingAddressID = @BillingAddressID
                WHERE CustomerID = @CustomerID; ";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", id);
                command.Parameters.AddWithValue("@EmailAddress", dto.EmailAddress);
                command.Parameters.AddWithValue("@Password", dto.Password);
                command.Parameters.AddWithValue("@FirstName", dto.FirstName);
                command.Parameters.AddWithValue("@LastName", dto.LastName);
                command.Parameters.AddWithValue("@ShippingAddressID", dto.ShippingAddressID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@BillingAddressID", dto.BillingAddressID ?? (object)DBNull.Value);
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error updating customer {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            const string query = @"DELETE FROM Customers WHERE CustomerID = @CustomerID;";
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", id);
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error deleting customer {id}");
                return false;
            }
        }
    }
}
