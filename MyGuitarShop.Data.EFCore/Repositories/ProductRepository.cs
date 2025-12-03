using MyGuitarShop.Data.EFCore.Abstract;
using MyGuitarShop.Data.EFCore.Context;
using MyGuitarShop.Data.EFCore.Entities;

namespace MyGuitarShop.Data.EFCore.Repositories
{
    public class ProductRepository(MyGuitarShopContext dbContext)
        : RepositoryBase<Product>(dbContext)
    {

    }
}
