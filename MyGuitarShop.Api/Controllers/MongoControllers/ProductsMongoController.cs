using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Enums;
using MyGuitarShop.Data.MongoDb.Models;
using MyGuitarShop.Data.MongoDb.Services;

namespace MyGuitarShop.Api.Controllers.MongoControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsMongoController(
        ILogger<ProductsMongoController> logger,
        ProductService productService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var products = await productService.GetAllAsync();
                if (products.Count() != 0)
                    return Ok(products);
                return NotFound("No products found in MongoDB");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving products from MongoDB");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internel server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                var product = await productService.FindByIdAsync(id);
                if (product == null)
                    return NotFound($"Product with id {id} not found in MongoDB");

                return Ok(product);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving product by ID from MongoDB");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving product by ID");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(ProductDto newProduct)
        {
            try
            {
                var model = new ProductDto
                {
                    CategoryID = newProduct.CategoryID,
                    ProductCode = newProduct.ProductCode,
                    ProductName = newProduct.ProductName,
                    Description = newProduct.Description,
                    ListPrice = newProduct.ListPrice,
                    DiscountPercent = newProduct.DiscountPercent,
                    Quantity = 1
                };

                var numProductsCreated = await productService.InsertAsync(model);
                return Ok($"{numProductsCreated} new products created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating product");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating product");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(string id, ProductDto updatedProduct)
        {
            try
            {
                var existingProduct = await productService.FindByIdAsync(id);
                if (existingProduct == null)
                    return NotFound($"Product with id {id} not found in MongoDB");

                existingProduct.Category = CategoryType.None;
                existingProduct.ProductCode = updatedProduct.ProductCode;
                existingProduct.ProductName = updatedProduct.ProductName;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.ListPrice = updatedProduct.ListPrice;
                existingProduct.DiscountPercent = updatedProduct.DiscountPercent;
                /*if (await productService.UpdateAsync(id, existingProduct))
                    return Ok($"Product with id {id} updated");*/
                return BadRequest("Failed to update product in MongoDB");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating product");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating product");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(string id)
        {
            try
            {
                var existingProduct = await productService.FindByIdAsync(id);
                if (existingProduct == null)
                    return NotFound($"Product with id {id} not found in MongoDB");

                if (await productService.DeleteAsync(id))
                    return Ok($"Product with id {id} deleted");
                return BadRequest("Failed to delete product from MongoDB");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting product");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting product");
            }
        }
    }
}
