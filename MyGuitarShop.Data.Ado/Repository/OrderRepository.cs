using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;
using Newtonsoft.Json;
using static Azure.Core.HttpHeader;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class OrderRepository(
        ILogger<OrderRepository> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<OrderEntity, OrderDto>
    {
        public async Task<IEnumerable<OrderEntity>> GetAllAsync()
        {
            var orders = new List<OrderEntity>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Orders;", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var order = new OrderEntity
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
            string query = @"
                BEGIN TRY
                    BEGIN TRANSACTION;

                    DECLARE @BillingAddressID INT;
                    DECLARE @ShipAddressID INT;
                    DECLARE @OrderID INT;
                    DECLARE @ItemPrice DECIMAL(18,2);
                    DECLARE @DiscountAmount DECIMAL(18,2);

                    SELECT @BillingAddressID = BillingAddressID from Customers where CustomerID = @CustomerID;
                    SELECT @ShipAddressID = ShippingAddressID from Customers where CustomerID = @CustomerID;

                    -- Insert Order
                    INSERT INTO Orders (CustomerID, OrderDate, ShipAmount, TaxAmount, ShipDate, ShipAddressID, CardType, CardNumber, CardExpires, BillingAddressID)
                    VALUES (@CustomerID, @OrderDate, @ShipAmount, @TaxAmount, @ShipDate, @ShipAddressID, @CardType, @CardNumber, @CardExpires, @BillingAddressID);
                    
                    SET @OrderID = SCOPE_IDENTITY();

            ";

            try
            {
                // Insert Order Items
                for (int i = 0; i < dto.Items.Count; i++)
                {
                    var item = dto.Items[i];
                    query += @"
                        SELECT @ItemPrice = ListPrice from Products where ProductID = @ProductID" + i + @";
                        SELECT @DiscountAmount = (@ItemPrice * DiscountPercent * 0.01) from Products where ProductID = @ProductID" + i + @";

                        INSERT INTO OrderItems (OrderID, ProductID, ItemPrice, DiscountAmount, Quantity)
                        VALUES (@OrderID, @ProductID" + i + @", @ItemPrice, @DiscountAmount, @Quantity" + i + @");
                        ";
                }

                // Complete Transaction
                query += @"
                    -- Commit if all succeeded
                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION;

                    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
                    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
                    DECLARE @ErrorState INT = ERROR_STATE();
                    RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
                    END CATCH; ";

                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);

                // Order parameters
                command.Parameters.AddWithValue("@CustomerID", dto.CustomerID);
                command.Parameters.AddWithValue("@OrderDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@ShipAmount", dto.ShipAmount);
                command.Parameters.AddWithValue("@TaxAmount", dto.TaxAmount);
                command.Parameters.AddWithValue("@ShipDate", DBNull.Value);
                command.Parameters.AddWithValue("@CardType", dto.CardType);
                command.Parameters.AddWithValue("@CardNumber", dto.CardNumber);
                command.Parameters.AddWithValue("@CardExpires", dto.CardExpires);

                // Order Items parameters
                for (int i = 0; i < dto.Items.Count; i++)
                {
                    var item = dto.Items[i];
                    command.Parameters.AddWithValue("@ProductID" + i, item.ProductID);
                    command.Parameters.AddWithValue("@ItemPrice" + i, item.ListPrice);
                    command.Parameters.AddWithValue("@DiscountAmount" + i, item.ListPrice * item.DiscountPercent * 0.01m);
                    command.Parameters.AddWithValue("@Quantity" + i, item.Quantity);
                }

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new order");
                return 0;
            }
        }

        public async Task<OrderEntity?> FindByIdAsync(int id)
        {
            OrderEntity? order = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Orders WHERE OrderID = @OrderID;", connection);
                command.Parameters.AddWithValue("@OrderID", id);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    order = new OrderEntity
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
                command.Parameters.AddWithValue("@CustomerID", dto.CustomerID);
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
