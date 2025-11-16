using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Common.Interfaces;
using MyGuitarShop.Data.Ado.Entities;

namespace MyGuitarShop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(
        ILogger<OrdersController> logger,
        IRepository<OrderEntity, OrderDto> repo,
        IRepository<OrderItemEntity, OrderItemDto> itemRepo)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            try
            {
                var orders = await repo.GetAllAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving orders");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving orders");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            try
            {
                var order = await repo.FindByIdAsync(id);
                if (order == null)
                    return NotFound();

                return Ok(order);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving order by ID");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving order by ID");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync(OrderDto newOrder)
        {
            try
            {
                var neworder = new OrderDto
                {
                    CustomerID = newOrder.CustomerID,
                    OrderDate = DateTime.UtcNow,
                    ShipAmount = newOrder.ShipAmount,
                    TaxAmount = newOrder.TaxAmount,
                    ShipDate = newOrder.ShipDate,
                    CardType = newOrder.CardType,
                    CardNumber = newOrder.CardNumber,
                    CardExpires = newOrder.CardExpires,
                    Items = newOrder.Items
                };

                var numOrdersCreated = await repo.InsertAsync(newOrder);

                foreach (var item in newOrder.Items)
                {
                    var orderItem = new OrderItemDto
                    {
                        OrderID = neworder.OrderID,
                        ProductID = item.ProductID,
                        ItemPrice = item.ItemPrice,
                        DiscountAmount = item.DiscountAmount,
                        Quantity = item.Quantity
                    };
                    await itemRepo.InsertAsync(orderItem);
                }

                return Ok($"{numOrdersCreated} new orders created");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating order");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error creating order");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrderAsync(int id, OrderDto updatedOrder)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Order with id {id} not found");

                var numOrdersUpdated = await repo.UpdateAsync(id, updatedOrder);
                return Ok($"{numOrdersUpdated} orders updated");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating order");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error updating order");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrderAsync(int id)
        {
            try
            {
                if (await repo.FindByIdAsync(id) == null)
                    return NotFound($"Order with id {id} not found");

                var numOrdersDeleted = await repo.DeleteAsync(id);
                return Ok($"{numOrdersDeleted} orders deleted");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting order");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error deleting order");
            }
        }
    }
}
