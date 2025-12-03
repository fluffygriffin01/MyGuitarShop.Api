using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Repository;

namespace MyGuitarShop.Api.Controllers.AdoController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(
        ILogger<CategoriesController> logger,
        IRepository<CategoryDto> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var categories = await repo.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving categories");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving categories");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var category = await repo.FindByIdAsync(id);
                if (category == null)
                    return NotFound();

                return Ok(category);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving category by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving category by ID");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategoryAsync(CategoryDto newCategory)
        {
            try
            {
                var numCategoriesCreated = await repo.InsertAsync(newCategory);
                return Ok($"{numCategoriesCreated} new categories created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating category");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating category");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, CategoryDto updatedCategory)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Category with id {id} not found");

                var numCategoriesUpdated = await repo.UpdateAsync(id, updatedCategory);
                return Ok($"{numCategoriesUpdated} categories updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating category");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating category");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Category with id {id} not found");

                var numCategoriesDeleted = await repo.DeleteAsync(id);
                return Ok($"{numCategoriesDeleted} categories deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting category");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting category");
            }
        }
    }
}
