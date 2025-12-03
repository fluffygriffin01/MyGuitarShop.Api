using MongoDB.Driver;
using MyGuitarShop.Common.Dtos;
using MyGuitarShop.Data.MongoDb.Abstract;
using MyGuitarShop.Data.MongoDb.Models;

namespace MyGuitarShop.Data.MongoDb.Services
{
    public class ProductService(IMongoDatabase database) 
        : MongoService<ProductModel, ProductDto>(database)
    {

    }
}
