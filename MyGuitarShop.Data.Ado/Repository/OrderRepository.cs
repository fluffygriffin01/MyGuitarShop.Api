using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Factories;
using Newtonsoft.Json;

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
                        Customer = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        //OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        //ShipAmount = reader.GetDecimal(reader.GetOrdinal("ShipAmount")),
                        //TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                        //ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                        ShipAddress = reader.GetInt32(reader.GetOrdinal("ShipAddressID")),
                        //CardType = reader.GetString(reader.GetOrdinal("CardType")),
                        //CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                        //CardExpires = reader.GetString(reader.GetOrdinal("CardExpires")),
                        BillingAddress = reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
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
                BEGIN TRY
                    BEGIN TRANSACTION;

                    DECLARE @BillingAddressID INT;
                    DECLARE @ShipAddressID INT;
                    DECLARE @OrderID INT;

                    -- Insert Billing Address
                    INSERT INTO Addresses (CustomerID, Line1, Line2, City, State, ZipCode, Phone, Disabled)
                    VALUES (@B_CustomerID, @B_Line1, @B_Line2, @B_City, @B_State, @B_ZipCode, @B_Phone, @B_Disabled);

                    SET @BillingAddressID = SCOPE_IDENTITY();

                    -- Insert Shipping Address
                    INSERT INTO Addresses (CustomerID, Line1, Line2, City, State, ZipCode, Phone, Disabled)
                    VALUES (@S_CustomerID, @S_Line1, @S_Line2, @S_City, @S_State, @S_ZipCode, @S_Phone, @S_Disabled);

                    SET @ShipAddressID = SCOPE_IDENTITY();

                    -- Insert Order
                    INSERT INTO Orders (CustomerID, OrderDate, ShipAmount, TaxAmount, ShipDate, ShipAddressID, CardType, CardNumber, CardExpires, BillingAddressID)
                    VALUES (@CustomerID, @OrderDate, @ShipAmount, @TaxAmount, @ShipDate, @ShipAddressID, @CardType, @CardNumber, @CardExpires, @BillingAddressID);

                    SET @OrderID = SCOPE_IDENTITY();

                    -- Insert Order Items
                        INSERT INTO OrderItems (OrderID, ProductID, ItemPrice, DiscountAmount, Quantity)
                        SELECT 
                            @OrderID,
                            ProductID,
                            ItemPrice,
                            DiscountAmount,
                            Quantity
                        FROM OPENJSON(@OrderItems)
                        WITH (
                            ProductID INT,
                            ItemPrice DECIMAL(10,2),
                            DiscountAmount DECIMAL(10,2),
                            Quantity INT,
                        );

                    -- Commit if all succeeded
                    COMMIT TRANSACTION;
                END TRY
                BEGIN CATCH
                    ROLLBACK TRANSACTION;

                    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE('Error creating order');
                    DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
                    DECLARE @ErrorState INT = ERROR_STATE();
                    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
                END CATCH;";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", dto.Customer ?? (object)DBNull.Value);

                // For billing address
                command.Parameters.AddWithValue("@B_Line1", dto.BillingAddress.Line1);
                command.Parameters.AddWithValue("@B_Line2", dto.BillingAddress.Line2);
                command.Parameters.AddWithValue("@B_City", dto.BillingAddress.City);
                command.Parameters.AddWithValue("@B_State", dto.BillingAddress.State);
                command.Parameters.AddWithValue("@B_ZipCode", dto.BillingAddress.ZipCode);
                command.Parameters.AddWithValue("@B_Phone", dto.BillingAddress.Phone);
                command.Parameters.AddWithValue("@B_Disabled", dto.BillingAddress.Disabled);

                // For shipping address
                command.Parameters.AddWithValue("@S_Line1", dto.ShipAddress.Line1);
                command.Parameters.AddWithValue("@S_Line2", dto.ShipAddress.Line2);
                command.Parameters.AddWithValue("@S_City", dto.ShipAddress.City);
                command.Parameters.AddWithValue("@S_State", dto.ShipAddress.State);
                command.Parameters.AddWithValue("@S_ZipCode", dto.ShipAddress.ZipCode);
                command.Parameters.AddWithValue("@S_Phone", dto.ShipAddress.Phone);
                command.Parameters.AddWithValue("@S_Disabled", dto.ShipAddress.Disabled);

                // For order
                command.Parameters.AddWithValue("@OrderDate", DateTime.UtcNow);
                command.Parameters.AddWithValue("@ShipAmount", 10);
                command.Parameters.AddWithValue("@TaxAmount", 20);
                command.Parameters.AddWithValue("@ShipDate", DBNull.Value);
                command.Parameters.AddWithValue("@CardType", "Master");
                command.Parameters.AddWithValue("@CardNumber", "1001");
                command.Parameters.AddWithValue("@CardExpires", "04/2025");

                // For order items
                var jsonItems = JsonConvert.SerializeObject(dto.Items);
                command.Parameters.AddWithValue("@OrderItems", jsonItems);

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
                        Customer = reader.IsDBNull(reader.GetOrdinal("CustomerID")) ? null : reader.GetInt32(reader.GetOrdinal("CustomerID")),
                        //OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                        //ShipAmount = reader.GetDecimal(reader.GetOrdinal("ShipAmount")),
                        //TaxAmount = reader.GetDecimal(reader.GetOrdinal("TaxAmount")),
                        //ShipDate = reader.IsDBNull(reader.GetOrdinal("ShipDate")) ? null : reader.GetDateTime(reader.GetOrdinal("ShipDate")),
                        ShipAddress = reader.GetInt32(reader.GetOrdinal("ShipAddressID")),
                        //CardType = reader.GetString(reader.GetOrdinal("CardType")),
                        //CardNumber = reader.GetString(reader.GetOrdinal("CardNumber")),
                        //CardExpires = reader.GetString(reader.GetOrdinal("CardExpires")),
                        BillingAddress = reader.GetInt32(reader.GetOrdinal("BillingAddressID"))
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
                //command.Parameters.AddWithValue("@ShipAmount", dto.ShipAmount);
                //command.Parameters.AddWithValue("@TaxAmount", dto.TaxAmount);
                //command.Parameters.AddWithValue("@ShipDate", dto.ShipDate ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ShipAddressID", dto.ShipAddressID);
                //command.Parameters.AddWithValue("@CardType", dto.CardType);
                //command.Parameters.AddWithValue("@CardNumber", dto.CardNumber);
                //command.Parameters.AddWithValue("@CardExpires", dto.CardExpires);
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
