using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.Dtos
{
    public class OrderItemDto
    {
        public int? ItemID { get; set; } = null;
        public int? OrderID { get; set; } = null;
        public int? ProductID { get; set; } = null;
        public required decimal ItemPrice { get; set; }
        public required decimal DiscountAmount { get; set; }
        public required int Quantity { get; set; }
    }
}
