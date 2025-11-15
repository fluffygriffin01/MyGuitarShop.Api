using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;

namespace MyGuitarShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderItemsController(
        ILogger<OrderItemsController> logger,
        IRepository<OrderItemEntity, OrderItemDto> repo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var orderItems = await repo.GetAllAsync();
                return Ok(orderItems);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving orderItems");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving orderItems");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var orderItem = await repo.FindByIdAsync(id);
                if (orderItem == null)
                    return NotFound();

                return Ok(orderItem);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving orderItem by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving orderItem by ID");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderItemAsync(OrderItemDto newOrderItem)
        {
            try
            {
                var numOrderItemsCreated = await repo.InsertAsync(newOrderItem);
                return Ok($"{numOrderItemsCreated} new orderItems created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating orderItem");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating orderItem");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderItemAsync(int id, OrderItemDto updatedOrderItem)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"OrderItem with id {id} not found");

                var numOrderItemsUpdated = await repo.UpdateAsync(id, updatedOrderItem);
                return Ok($"{numOrderItemsUpdated} orderItems updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating orderItem");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating orderItem");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderItemAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"OrderItem with id {id} not found");

                var numOrderItemsDeleted = await repo.DeleteAsync(id);
                return Ok($"{numOrderItemsDeleted} orderItems deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting orderItem");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting orderItem");
            }
        }
    }
}
