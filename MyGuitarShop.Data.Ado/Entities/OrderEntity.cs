using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Data.Ado.Entities
{
    public class OrderEntity
    {
        public required int OrderID { get; set; }
        public int? CustomerID { get; set; } = null;
        public required DateTime OrderDate { get; set; }
        public required decimal ShipAmount { get; set; }
        public required decimal TaxAmount { get; set; }
        public DateTime? ShipDate { get; set; }
        public required int ShipAddressID { get; set; }
        [MaxLength(50)]
        public required string CardType { get; set; }
        [MaxLength(16)]
        public required string CardNumber { get; set; }
        [MaxLength(7)]
        public required string CardExpires { get; set; }
        public required int BillingAddressID { get; set; }
    }
}
