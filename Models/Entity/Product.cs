using System;
using System.Collections.Generic;

namespace QShop.Models.Entity;

public partial class Product
{
    public int ProductId { get; set; }

    public int AreaId { get; set; }

    public int CategoryId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public string? PImage { get; set; }

    public virtual Area Area { get; set; } = null!;

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual Category Category { get; set; } = null!;
}
