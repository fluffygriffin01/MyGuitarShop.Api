using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Common.Dtos
{
    public class CategoryDto
    {
        public int? CategoryID { get; set; } = null;
        [MaxLength(255)]
        public required string CategoryName { get; set; }
    }
}
