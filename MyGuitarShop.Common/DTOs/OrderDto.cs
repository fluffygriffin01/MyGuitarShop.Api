using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.Dtos
{
    public class OrderDto
    {
        public int? OrderID { get; set; } = null;
        public CustomerDto? Customer { get; set; } = null;
        public required DateTime OrderDate { get; set; }
        public required decimal ShipAmount { get; set; }
        public required decimal TaxAmount { get; set; }
        public DateTime? ShipDate { get; set; }
        public int? ShipAddressID { get; set; } = null;
        [MaxLength(50)]
        public required string CardType { get; set; }
        [MaxLength(16)]
        public required string CardNumber { get; set; }
        [MaxLength(7)]
        public required string CardExpires { get; set; }
        public int? BillingAddressID { get; set; } = null;
        public required List<OrderItemDto> Items { get; set; }
    }
}
