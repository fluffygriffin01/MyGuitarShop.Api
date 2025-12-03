using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Factories;

namespace MyGuitarShop.Data.Ado.Repository
{
    public class CategoryRepository(
        ILogger<CategoryRepository> logger,
        SqlConnectionFactory sqlConnectionFactory)
        : IRepository<CategoryDto>
    {
        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = new List<CategoryDto>();

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Categories;", connection);
                var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var category = new CategoryDto
                    {
                        CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                    };
                    categories.Add(category);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error retrieving category list");
            }

            return categories;
        }

        public async Task<int> InsertAsync(CategoryDto dto)
        {
            const string query = @"
                INSERT INTO Categories (CategoryName)
                VALUES (@CategoryName);";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryName", dto.CategoryName);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, "Error inserting new category");
                return 0;
            }
        }

        public async Task<CategoryDto?> FindByIdAsync(int id)
        {
            CategoryDto? category = null;

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Categories WHERE CategoryID = @CategoryID;", connection);
                command.Parameters.AddWithValue("@CategoryID", id);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    category = new CategoryDto
                    {
                        CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving category {id} by ID");
            }
            return category;
        }

        public async Task<CategoryDto?> FindByUniqueAsync(string categoryName)
        {
            CategoryDto? category = null;
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(cmdText: "SELECT * FROM Categories WHERE CategoryName = @CategoryName;", connection);
                command.Parameters.AddWithValue("@CategoryName", categoryName);
                var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    category = new CategoryDto
                    {
                        CategoryID = reader.GetInt32(reader.GetOrdinal("CategoryID")),
                        CategoryName = reader.GetString(reader.GetOrdinal("CategoryName"))
                    };
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error retrieving category with name {categoryName}");
            }
            return category;
        }

        public async Task<int> UpdateAsync(int id, CategoryDto dto)
        {
            const string query = @"
                UPDATE Categories
                SET CategoryName = @CategoryName
                WHERE CategoryID = @CategoryID; ";

            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryID", id);
                command.Parameters.AddWithValue("@CategoryName", dto.CategoryName);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error updating category {id}");
                throw;
            }
        }

        public async Task<int> DeleteAsync(int id)
        {
            const string query = @"DELETE FROM Categories WHERE CategoryID = @CategoryID;";
            try
            {
                await using var connection = await sqlConnectionFactory.OpenSqlConnectionAsync();
                await using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CategoryID", id);
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message, $"Error deleting category {id}");
                return 0;
            }
        }
    }
}
