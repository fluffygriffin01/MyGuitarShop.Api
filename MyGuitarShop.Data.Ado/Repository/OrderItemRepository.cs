using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class OrderItemRepository(
        ILogger<OrderItemRepository> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<OrderItemDto>
    {
        public async Task<IEnumerable<OrderItemDto>> GetAllAsync()
        {
            var orderItems = new List<OrderItemDto>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM OrderItems;", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var orderItem = new OrderItemDto
                    {
                        ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                        OrderID = reader.IsDBNull(reader.GetOrdinal("OrderID")) ? null : reader.GetInt32(reader.GetOrdinal("OrderID")),
                        ProductID = reader.IsDBNull(reader.GetOrdinal("ProductID")) ? null : reader.GetInt32(reader.GetOrdinal("ProductID")),
                        ItemPrice = reader.GetDecimal(reader.GetOrdinal("ItemPrice")),
                        DiscountAmount = reader.GetDecimal(reader.GetOrdinal("DiscountAmount")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                    };
                    orderItems.Add(orderItem);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving orderItem list");
            }

            return orderItems;
        }

        public async Task<int> InsertAsync(OrderItemDto dto)
        {
            const string query = @"
                INSERT INTO OrderItems (OrderID, ProductID, ItemPrice, DiscountAmount, Quantity)
                VALUES (@OrderID, @ProductID, @ItemPrice, @DiscountAmount, @Quantity);";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderID", dto.OrderID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProductID", dto.ProductID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ItemPrice", dto.ItemPrice);
                command.Parameters.AddWithValue("@DiscountAmount", dto.DiscountAmount);
                command.Parameters.AddWithValue("@Quantity", dto.Quantity);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new orderItem");
                return 0;
            }
        }

        public async Task<OrderItemDto?> FindByIdAsync(int id)
        {
            OrderItemDto? orderItem = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM OrderItems WHERE ItemID = @ItemID;", connection);
                command.Parameters.AddWithValue("@ItemID", id);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    orderItem = new OrderItemDto
                    {
                        ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                        OrderID = reader.IsDBNull(reader.GetOrdinal("OrderID")) ? null : reader.GetInt32(reader.GetOrdinal("OrderID")),
                        ProductID = reader.IsDBNull(reader.GetOrdinal("ProductID")) ? null : reader.GetInt32(reader.GetOrdinal("ProductID")),
                        ItemPrice = reader.GetDecimal(reader.GetOrdinal("ItemPrice")),
                        DiscountAmount = reader.GetDecimal(reader.GetOrdinal("DiscountAmount")),
                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving orderItem {id} by ID");
            }
            return orderItem;
        }

        public async Task<int> UpdateAsync(int id, OrderItemDto dto)
        {
            const string query = @"
                UPDATE OrderItems
                SET OrderID = @OrderID,
                    ProductID = @ProductID,
                    ItemPrice = @ItemPrice,
                    DiscountAmount = @DiscountAmount,
                    Quantity = @Quantity
                WHERE ItemID = @ItemID; ";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemID", id);
                command.Parameters.AddWithValue("@OrderID", dto.OrderID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProductID", dto.ProductID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ItemPrice", dto.ItemPrice);
                command.Parameters.AddWithValue("@DiscountAmount", dto.DiscountAmount);
                command.Parameters.AddWithValue("@Quantity", dto.Quantity);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error updating orderItem {id}");
                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string query = @"DELETE FROM OrderItems WHERE ItemID = @ItemID;";
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ItemID", id);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error deleting orderItem {id}");
                return 0;
            }
        }
    }
}
