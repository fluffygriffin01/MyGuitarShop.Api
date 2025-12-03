using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.Dtos
{
    public class ProductDto
    {
        public int? ProductID { get; set; } = null;
        public int? CategoryID { get; set; } = null;
        [MaxLength(10)]
        public required string ProductCode { get; set; }
        [MaxLength(255)]
        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public required decimal ListPrice { get; set; }
        public required decimal DiscountPercent { get; set; } = 0m;
        public required int Quantity { get; set; }
        public DateTime? DateAdded { get; set; } = null;
    }
}
