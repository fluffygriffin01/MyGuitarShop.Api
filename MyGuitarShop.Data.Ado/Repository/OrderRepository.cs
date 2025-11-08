using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
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
    public class OrderRepository(
        ILogger<OrderRepository> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<OrderDto>
    {
        public async Task<IEnumerable<OrderDto>> GetAllAsync()
        {
            var orders = new List<OrderDto>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Orders;", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var order = new OrderDto
                    {
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        ShipAmount = reader.GetDecimal(reader.GetOrdinal("ShipAmount")),
                        TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                        ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                        ShipAddressID = reader.GetInt32(reader.GetOrdinal("ShipAddressID")),
                        CardType = reader.GetString(reader.GetOrdinal("CardType")),
                        CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                        CardExpires = reader.GetString(reader.GetOrdinal("CardExpires")),
                        BillingAddressID = reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                    orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving order list");
            }

            return orders;
        }

        public async Task<int> InsertAsync(OrderDto dto)
        {
            const string query = @"
                INSERT INTO Orders (CustomerID, OrderDate, ShipAmount, TaxAmount, ShipDate, ShipAddressID, CardType, CardNumber, CardExpires, BillingAddressID)
                VALUES (@CustomerID, @OrderDate, @ShipAmount, @TaxAmount, @ShipDate, @ShipAddressID, @CardType, @CardNumber, @CardExpires, @BillingAddressID);";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", dto.CustomerID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@ShipAmount", dto.ShipAmount);
                command.Parameters.AddWithValue("@TaxAmount", dto.TaxAmount);
                command.Parameters.AddWithValue("@ShipDate", dto.ShipDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShipAddressID", dto.ShipAddressID);
                command.Parameters.AddWithValue("@CardType", dto.CardType);
                command.Parameters.AddWithValue("@CardNumber", dto.CardNumber);
                command.Parameters.AddWithValue("@CardExpires", dto.CardExpires);
                command.Parameters.AddWithValue("@BillingAddressID", dto.BillingAddressID);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new order");
                return 0;
            }
        }

        public async Task<OrderDto?> FindByIdAsync(int id)
        {
            OrderDto? order = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Orders WHERE OrderID = @OrderID;", connection);
                command.Parameters.AddWithValue("@OrderID", id);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    order = new OrderDto
                    {
                        OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                        CustomerID = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        ShipAmount = reader.GetDecimal(reader.GetOrdinal("ShipAmount")),
                        TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                        ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                        ShipAddressID = reader.GetInt32(reader.GetOrdinal("ShipAddressID")),
                        CardType = reader.GetString(reader.GetOrdinal("CardType")),
                        CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                        CardExpires = reader.GetString(reader.GetOrdinal("CardExpires")),
                        BillingAddressID = reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving order {id} by ID");
            }
            return order;
        }

        public async Task<int> UpdateAsync(int id, OrderDto dto)
        {
            const string query = @"
                UPDATE Orders
                SET CustomerID = @CustomerID,
                    OrderDate = @OrderDate,
                    ShipAmount = @ShipAmount,
                    TaxAmount = @TaxAmount,
                    ShipDate = @ShipDate,
                    ShipAddressID = @ShipAddressID,
                    CardType = @CardType,
                    CardNumber = @CardNumber,
                    CardExpires = @CardExpires,
                    BillingAddressID = @BillingAddressID
                WHERE OrderID = @OrderID; ";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", id);
                command.Parameters.AddWithValue("@CustomerID", dto.CustomerID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@ShipAmount", dto.ShipAmount);
                command.Parameters.AddWithValue("@TaxAmount", dto.TaxAmount);
                command.Parameters.AddWithValue("@ShipDate", dto.ShipDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShipAddressID", dto.ShipAddressID);
                command.Parameters.AddWithValue("@CardType", dto.CardType);
                command.Parameters.AddWithValue("@CardNumber", dto.CardNumber);
                command.Parameters.AddWithValue("@CardExpires", dto.CardExpires);
                command.Parameters.AddWithValue("@BillingAddressID", dto.BillingAddressID);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error updating order {id}");
                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string query = @"DELETE FROM Orders WHERE OrderID = @OrderID;";
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", id);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error deleting order {id}");
                return 0;
            }
        }
    }
}
