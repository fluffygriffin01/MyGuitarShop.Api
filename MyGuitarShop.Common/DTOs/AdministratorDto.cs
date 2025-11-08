using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.Dtos
{
    public class AdministratorDto
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
