using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;

namespace MyGuitarShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(
        ILogger<ProductsController> logger,
        IRepository<ProductEntity, ProductDto> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var products = await repo.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving products");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving products");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var product = await repo.FindByIdAsync(id);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving product by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving product by ID");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateProductAsync(ProductDto newProduct)
        {
            try
            {
                var numProductsCreated = await repo.InsertAsync(newProduct);
                return Ok($"{numProductsCreated} new products created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating product");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating product");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProductAsync(int id, ProductDto updatedProduct)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Product with id {id} not found");

                var numProductsUpdated = await repo.UpdateAsync(id, updatedProduct);
                return Ok($"{numProductsUpdated} products updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating product");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating product");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Product with id {id} not found");

                var numProductsDeleted = await repo.DeleteAsync(id);
                return Ok($"{numProductsDeleted} products deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting product");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting product");
            }
        }
    }
}
