using MyGuitarShop.Data.EFCore.Abstract;
using MyGuitarShop.Data.EFCore.Context;
using MyGuitarShop.Data.EFCore.Entities;

namespace MyGuitarShop.Data.EFCore.Repositories
{
    public class OrderItemRepository(MyGuitarShopContext dbContext)
        : RepositoryBase<OrderItem>(dbContext)
    {

    }
}
