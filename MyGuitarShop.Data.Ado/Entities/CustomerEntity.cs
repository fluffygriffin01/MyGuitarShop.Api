using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Data.Ado.Entities
{
    public class CustomerEntity
    {
        public required int CustomerID { get; set; }
        [MaxLength(255)]
        public required string EmailAddress { get; set; }
        [MaxLength(60)]
        public required string Password { get; set; }
        [MaxLength(60)]
        public required string FirstName { get; set; }
        [MaxLength(60)]
        public required string LastName { get; set; }
        public int? ShippingAddressID { get; set; } = null;
        public int? BillingAddressID { get; set; } = null;
    }
}
