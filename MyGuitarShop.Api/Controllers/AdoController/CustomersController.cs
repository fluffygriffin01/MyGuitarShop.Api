using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;

namespace MyGuitarShop.Api.Controllers.AdoController
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController(
        ILogger<CustomersController> logger,
        IRepository<CustomerDto> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var customers = await repo.GetAllAsync();
                return Ok(customers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving customers");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving customers");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var customer = await repo.FindByIdAsync(id);
                if (customer == null)
                    return NotFound();

                return Ok(customer);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving customer by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving customer by ID");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CustomerDto newCustomer)
        {
            try
            {
                var numCustomersCreated = await repo.InsertAsync(newCustomer);
                return Ok($"{numCustomersCreated} new customers created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating customer");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating customer");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomerAsync(int id, CustomerDto updatedCustomer)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Customer with id {id} not found");

                var numCustomersUpdated = await repo.UpdateAsync(id, updatedCustomer);
                return Ok($"{numCustomersUpdated} customers updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating customer");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating customer");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Customer with id {id} not found");

                var numCustomersDeleted = await repo.DeleteAsync(id);
                return Ok($"{numCustomersDeleted} customers deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting customer");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting customer");
            }
        }
    }
}
