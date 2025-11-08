using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGuitarShop.Common.Dtos
{
    public class ProductDto
    {
        public required int ProductID { get; set; }
        public int? CategoryID { get; set; } = null;
        [MaxLength(10)]
        public required string ProductCode { get; set; }
        [MaxLength(255)]
        public required string ProductName { get; set; }
        public required string Description { get; set; }
        public required decimal ListPrice { get; set; }
        public required decimal DiscountPercent { get; set; } = 0m;
        public DateTime? DateAdded { get; set; } = null;
    }
}
