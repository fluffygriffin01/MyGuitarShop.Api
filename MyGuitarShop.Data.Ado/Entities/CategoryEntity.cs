using System.ComponentModel.DataAnnotations;

namespace MyGuitarShop.Data.Ado.Entities
{
    internal class CategoryEntity
    {
        public required int CategoryID { get; set; }
        [MaxLength(255)]
        public required string CategoryName { get; set; }
    }
}
