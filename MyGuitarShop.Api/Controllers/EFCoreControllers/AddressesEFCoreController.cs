using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Api.Abstract;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Data.EFCore.Repositories;
using MyGuitarShop.Data.EFCore.Entities;

namespace MyGuitarShop.Api.Controllers.EFCoreControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesEFCoreController(
        AddressRepository repository, 
        ILogger<AddressesEFCoreController> logger) 
        : BaseController<AddressDto, Address>(repository, logger)
    {

    }
}
