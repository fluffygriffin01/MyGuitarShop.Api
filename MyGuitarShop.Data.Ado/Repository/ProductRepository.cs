using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.Ado.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class ProductRepository(
        ILogger<ProductRepository> logger,
        SqlConnectionFactory sqlConnectionFactory)
    {
        public async Task<IEnumerable<ProductEntity>> GetAllProductsAsync()
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
                        ProductName =  reader.GetString(reader.GetOrdinal("ProductName")),
                        Description = reader.GetString(reader.GetOrdinal("Description")),
                        ListPrice = reader.GetDecimal(reader.GetOrdinal("ListPrice")),
                        DiscountPercent = reader.GetDecimal(reader.GetOrdinal("DiscountPercent")),
                        DateAdded = reader.IsDBNull(reader.GetOrdinal("DateAdded")) ? null : reader.GetDateTime(reader.GetOrdinal("DateAdded"))
                    };
                    products.Add(product);
                }
            }
            catch(Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving product list");
            }

            return products;
        }
    }
}
