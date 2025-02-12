using System;
using System.Collections.Generic;

namespace QShop.Models.Entity;

public partial class Area
{
    public int AreaId { get; set; }

    public string? AreaName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
