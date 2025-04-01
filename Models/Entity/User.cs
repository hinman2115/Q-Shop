using System;
using System.Collections.Generic;

namespace QShop.Models.Entity;

public partial class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public int? AreaId { get; set; }

    public string? Role { get; set; }

    public string PasswordHash { get; set; } = null!;

    public virtual Area? Area { get; set; }

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
}
