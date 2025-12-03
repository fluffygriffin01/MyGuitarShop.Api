using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Api.Abstract;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Data.EFCore.Repositories;
using MyGuitarShop.Data.EFCore.Entities;

namespace MyGuitarShop.Api.Controllers.EFCoreControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersEFCoreController(
        CustomerRepository repository, 
        ILogger<CustomersEFCoreController> logger) 
        : BaseController<CustomerDto, Customer>(repository, logger)
    {

    }
}
