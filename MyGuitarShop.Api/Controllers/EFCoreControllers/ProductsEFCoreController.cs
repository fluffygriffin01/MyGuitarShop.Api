using Microsoft.AspNetCore.Mvc;
using MyGuitarShop.Api.Abstract;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Data.Ado.Repository;
using MyGuitarShop.Data.EFCore.Entities;

namespace MyGuitarShop.Api.Controllers.EFCoreControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsEFCoreController(ProductRepository repository, ILogger<ProductsEFCoreController> logger) 
        : BaseController<ProductDto, Product>((Common.Interfaces.IRepository<Product>)repository, logger)
    {

    }
}
