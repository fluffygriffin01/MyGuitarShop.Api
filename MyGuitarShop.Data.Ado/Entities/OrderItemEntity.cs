namespace MyGuitarShop.Data.Ado.Entities
{
    public class OrderItemEntity
    {
        public required int ItemID { get; set; }
        public int? OrderID { get; set; } = null;
        public int? ProductID { get; set; } = null;
        public required decimal ItemPrice { get; set; }
        public required decimal DiscountAmount { get; set; }
        public required int Quantity { get; set; }
    }
}
