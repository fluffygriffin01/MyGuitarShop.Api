using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Data.Ado.Entities
{
    public class ProductEntity
    {
        public required int ProductID { get; set; }
        public int? CategoryID { get; set; }
        [MaxLength(10)]
        public required string ProductCode { get; set; }
        [MaxLength(255)]
        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public required decimal ListPrice { get; set; }
        public required decimal DiscountPercent { get; set; }
        public DateTime? DateAdded { get; set; }
    }
}
