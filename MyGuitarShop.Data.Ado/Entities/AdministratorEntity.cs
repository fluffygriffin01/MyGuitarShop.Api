using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Data.Ado.Entities
{
    public class AdministratorEntity
    {
        public required int AdminID { get; set; }
        [MaxLength(255)]
        public required string EmailAddress { get; set; }
        [MaxLength(255)]
        public required string Password { get; set; }
        [MaxLength(255)]
        public required string FirstName { get; set; }
        [MaxLength(255)]
        public required string LastName { get; set; }
    }
}
