using System;
using System.Collections.Generic;

namespace MyGuitarShop.Data.EFCore.Entities;

public partial class Administrator
{
    public int AdminId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
}
