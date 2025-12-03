using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.Dtos
{
    public class AdministratorDto
    {
        public int? AdminID { get; set; } = null;
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
