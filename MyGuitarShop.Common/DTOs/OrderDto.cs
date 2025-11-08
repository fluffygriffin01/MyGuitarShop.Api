using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Common.DTOs
{
    public class OrderDto
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
