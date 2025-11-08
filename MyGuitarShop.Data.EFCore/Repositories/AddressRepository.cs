using MyGuitarShop.Data.EFCore.Abstract;
using MyGuitarShop.Data.EFCore.Context;
using MyGuitarShop.Data.EFCore.Entities;

namespace MyGuitarShop.Data.EFCore.Repositories
{
    public class AddressRepository(MyGuitarShopContext dbContext)
        : RepositoryBase<Address>(dbContext)
    {

    }
}
