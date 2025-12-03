using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Data.EFCore.Abstract;
using MyGuitarShop.Data.EFCore.Context;
using MyGuitarShop.Data.EFCore.Entities;

namespace MyGuitarShop.Data.EFCore.Repositories
{
    public class CustomerRepository(MyGuitarShopContext dbContext)
        : RepositoryBase<Customer, CustomerDto>(dbContext)
    {

    }
}
