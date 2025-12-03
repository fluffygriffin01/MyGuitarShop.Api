using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class ProductRepository(
        ILogger<ProductRepository> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<ProductEntity, ProductDto>
    {
        public async Task<IEnumerable<ProductEntity>> GetAllAsync()
        {
            var products = new List<ProductEntity>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Products;", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var product = new ProductEntity
                    {
                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        ProductCode = reader.GetString(reader.GetOrdinal("ProductCode")),
                        ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        ListPrice = reader.GetDecimal(reader.GetOrdinal("ListPrice")),
                        DiscountPercent = reader.GetDecimal(reader.GetOrdinal("DiscountPercent")),
                        DateAdded = reader.IsDBNull(reader.GetOrdinal("DateAdded")) ? null : reader.GetDateTime(reader.GetOrdinal("DateAdded"))
                    };
                    products.Add(product);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving product list");
            }

            return products;
        }

        public async Task<ProductEntity?> FindByIdAsync(string id)
        {
            ProductEntity? product = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Products WHERE ProductID = @ProductID;", connection);
                command.Parameters.AddWithValue("@ProductID", id);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    product = new ProductEntity
                    {
                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        ProductCode = reader.GetString(reader.GetOrdinal("ProductCode")),
                        ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        ListPrice = reader.GetDecimal(reader.GetOrdinal("ListPrice")),
                        DiscountPercent = reader.GetDecimal(reader.GetOrdinal("DiscountPercent")),
                        DateAdded = reader.IsDBNull(reader.GetOrdinal("DateAdded")) ? null : reader.GetDateTime(reader.GetOrdinal("DateAdded"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving product {id} by ID");
            }
            return product;
        }

        public async Task<ProductEntity?> FindByUniqueAsync(string productCode)
        {
            ProductEntity? product = null;
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Products WHERE ProductCode = @ProductCode;", connection);
                command.Parameters.AddWithValue("@ProductCode", productCode);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    product = new ProductEntity
                    {
                        ProductID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? null : reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        ProductCode = reader.GetString(reader.GetOrdinal("ProductCode")),
                        ProductName = reader.GetString(reader.GetOrdinal("ProductName")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        ListPrice = reader.GetDecimal(reader.GetOrdinal("ListPrice")),
                        DiscountPercent = reader.GetDecimal(reader.GetOrdinal("DiscountPercent")),
                        DateAdded = reader.IsDBNull(reader.GetOrdinal("DateAdded")) ? null : reader.GetDateTime(reader.GetOrdinal("DateAdded"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving product with code {productCode}");
            }
            return product;
        }

        public async Task<bool> InsertAsync(ProductDto dto)
        {
            const string query = @"
                INSERT INTO Products (CategoryID, ProductCode, ProductName, Description, ListPrice, DiscountPercent, DateAdded)
                VALUES (@CategoryID, @ProductCode, @ProductName, @Description, @ListPrice, @DiscountPercent, @DateAdded);";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryID", dto.CategoryID);
                command.Parameters.AddWithValue("@ProductCode", dto.ProductCode);
                command.Parameters.AddWithValue("@ProductName", dto.ProductName);
                command.Parameters.AddWithValue("@Description", dto.Description);
                command.Parameters.AddWithValue("@ListPrice", dto.ListPrice);
                command.Parameters.AddWithValue("@DiscountPercent", dto.DiscountPercent);
                command.Parameters.AddWithValue("@DateAdded", DateTime.UtcNow);

                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new product");
                return false;
            }
        }

        public async Task<bool> UpdateAsync(string id, ProductDto dto)
        {
            const string query = @"
                UPDATE Products
                SET CategoryID = @CategoryID,
                    ProductCode = @ProductCode,
                    ProductName = @ProductName,
                    Description = @Description,
                    ListPrice = @ListPrice,
                    DiscountPercent = @DiscountPercent
                WHERE ProductID = @ProductID; ";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", id);
                command.Parameters.AddWithValue("@CategoryID", dto.CategoryID);
                command.Parameters.AddWithValue("@ProductCode", dto.ProductCode);
                command.Parameters.AddWithValue("@ProductName", dto.ProductName);
                command.Parameters.AddWithValue("@Description", dto.Description);
                command.Parameters.AddWithValue("@ListPrice", dto.ListPrice);
                command.Parameters.AddWithValue("@DiscountPercent", dto.DiscountPercent);
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error updating product {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string id)
        {
            const string query = @"DELETE FROM Products WHERE ProductID = @ProductID;";
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductID", id);
                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error deleting product {id}");
                return false;
            }
        }
    }
}
