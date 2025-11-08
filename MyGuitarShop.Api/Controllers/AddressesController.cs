using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController(
        ILogger<AddressesController> logger,
        IRepository<AddressDto> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var addresses = await repo.GetAllAsync();
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving addresses");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving addresses");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var address = await repo.FindByIdAsync(id);
                if (address == null)
                    return NotFound();

                return Ok(address);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving address by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving address by ID");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddressAsync(AddressDto newAddress)
        {
            try
            {
                var numAddressesCreated = await repo.InsertAsync(newAddress);
                return Ok($"{numAddressesCreated} new addresses created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating address");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating address");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddressAsync(int id, AddressDto updatedAddress)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Address with id {id} not found");

                var numAddressesUpdated = await repo.UpdateAsync(id, updatedAddress);
                return Ok($"{numAddressesUpdated} addresses updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating address");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating address");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddressAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Address with id {id} not found");

                var numAddressesDeleted = await repo.DeleteAsync(id);
                return Ok($"{numAddressesDeleted} addresses deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting address");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting address");
            }
        }
    }
}
