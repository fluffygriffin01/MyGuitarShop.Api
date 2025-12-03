using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Enums;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;
using MyGuitarShop.Data.MongoDb.Models;
using MyGuitarShop.Data.MongoDb.Services;

namespace MyGuitarShop.Api.Controllers.MongoControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MongoSeedController(
        ILogger<MongoSeedController> logger,
        ProductService productService,
        IRepository<ProductEntity, int> productRepo) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SeedMongoFromSqlServer()
        {
            try
            {
                var allProductsFromSql = await productRepo.GetAllAsync();
                foreach (var product in allProductsFromSql)
                {
                    if (!await productService.InsertAsync(new Common.Dtos.ProductDto
                    {
                        CategoryID = product.CategoryID,
                        ProductCode = product.ProductCode,
                        ProductName = product.ProductName,
                        Description = product.Description,
                        ListPrice = product.ListPrice,
                        DiscountPercent = product.DiscountPercent,
                        DateAdded = product.DateAdded ?? DateTime.UtcNow,
                        Quantity = 1
                    })) throw new Exception($"Problemms insert {product} into Mongo");
                }
                return Ok("All products inserted to Mongo");
            }
            catch (Exception ex)
            {
                logger.LogError("Unable to insert product\n\nError:{message}", ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, "Error inserting products to MongoDB");
            }
        }
    }
}
