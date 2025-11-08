using System;
using System.Collections.Generic;

namespace MyGuitarShop.Data.EFCore.Entities;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public int? ShippingAddressId { get; set; }

    public int? BillingAddressId { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
