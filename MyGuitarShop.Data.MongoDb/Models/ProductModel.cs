using MyGuitarShop.Common.Enums;
using MyGuitarShop.Data.MongoDb.Abstract;

namespace MyGuitarShop.Data.MongoDb.Models
{
    public class ProductModel : MongoModel
    {
        public CategoryType Category { get; set; }
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public decimal ListPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
