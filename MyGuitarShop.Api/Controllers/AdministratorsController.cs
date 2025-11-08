using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministratorsController(
        ILogger<AdministratorsController> logger,
        IRepository<AdministratorDto> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var administrators = await repo.GetAllAsync();
                return Ok(administrators);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving administrators");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving administrators");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var administrator = await repo.FindByIdAsync(id);
                if (administrator == null)
                    return NotFound();

                return Ok(administrator);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving administrator by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving administrator by ID");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdministratorAsync(AdministratorDto newAdministrator)
        {
            try
            {
                var numAdministratorsCreated = await repo.InsertAsync(newAdministrator);
                return Ok($"{numAdministratorsCreated} new administrators created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating administrator");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating administrator");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdministratorAsync(int id, AdministratorDto updatedAdministrator)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Administrator with id {id} not found");

                var numAdministratorsUpdated = await repo.UpdateAsync(id, updatedAdministrator);
                return Ok($"{numAdministratorsUpdated} administrators updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating administrator");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating administrator");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdministratorAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Administrator with id {id} not found");

                var numAdministratorsDeleted = await repo.DeleteAsync(id);
                return Ok($"{numAdministratorsDeleted} administrators deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting administrator");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting administrator");
            }
        }
    }
}
