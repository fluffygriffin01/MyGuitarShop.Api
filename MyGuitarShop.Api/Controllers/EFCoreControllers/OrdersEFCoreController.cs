using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Api.Abstract;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Data.EFCore.Repositories;
using MyGuitarShop.Data.EFCore.Entities;

namespace MyGuitarShop.Api.Controllers.EFCoreControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersEFCoreController(
        OrderRepository repository, 
        ILogger<OrdersEFCoreController> logger) 
        : BaseController<OrderDto, Order>(repository, logger)
    {

    }
}
