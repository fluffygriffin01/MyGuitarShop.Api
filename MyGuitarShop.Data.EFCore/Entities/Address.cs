using System;
using System.Collections.Generic;

namespace MyGuitarShop.Data.EFCore.Entities;

public partial class Address
{
    public int AddressId { get; set; }

    public int? CustomerId { get; set; }

    public string Line1 { get; set; } = null!;

    public string? Line2 { get; set; }

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string ZipCode { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int Disabled { get; set; }

    public virtual Customer? Customer { get; set; }
}
