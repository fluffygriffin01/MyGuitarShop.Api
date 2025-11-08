using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.Dtos
{
    public class CategoryDto
    {
        public required int CategoryID { get; set; }
        [MaxLength(255)]
        public required string CategoryName { get; set; }
    }
}
