using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.Dtos
{
    public class OrderDto
    {
        public int? OrderID { get; set; } = null;
        public CustomerDto? Customer { get; set; } = null;
        public required AddressDto ShipAddress { get; set; }
        public required AddressDto BillingAddress { get; set; }
        public required List<OrderItemDto> Items { get; set; }
    }
}
