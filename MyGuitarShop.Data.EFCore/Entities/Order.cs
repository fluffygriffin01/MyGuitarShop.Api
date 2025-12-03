using System;
using System.Collections.Generic;

namespace MyGuitarShop.Data.EFCore.Entities;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal ShipAmount { get; set; }

    public decimal TaxAmount { get; set; }

    public DateTime? ShipDate { get; set; }

    public int ShipAddressId { get; set; }

    public string CardType { get; set; } = null!;

    public string CardNumber { get; set; } = null!;

    public string CardExpires { get; set; } = null!;

    public int BillingAddressId { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
