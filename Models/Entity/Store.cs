using System;
using System.Collections.Generic;

namespace QShop.Models.Entity;

public partial class Store
{
    public int StoreId { get; set; }

    public string StoreName { get; set; } = null!;

    public string? OwnerName { get; set; }

    public string Contact { get; set; } = null!;

    public string Address { get; set; } = null!;
}
