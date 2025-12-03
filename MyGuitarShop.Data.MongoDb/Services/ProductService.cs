using MongoDB.Driver;
using MyGuitarShop.Data.MongoDb.Abstract;
using MyGuitarShop.Data.MongoDb.Models;

namespace MyGuitarShop.Data.MongoDb.Services
{
    public class ProductService(IMongoDatabase database) 
        : MongoService<ProductModel>(database)
    {

    }
}
