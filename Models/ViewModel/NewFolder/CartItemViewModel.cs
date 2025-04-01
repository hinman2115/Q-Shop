namespace QShop.Models.ViewModel
{
    public class CartItemViewModel
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }  // Added ProductId for reference
        public string ProductName { get; set; } // Added Product Name
        public decimal Price { get; set; }  // Fixed Price property
        public int Quantity { get; set; }  // Fixed Quantity property
        public string? PImage { get; set; }

        // List of cart items (Only for grouping all cart items, not per item)
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        // Calculate Total Price
        public decimal TotalPrice => Items?.Sum(i => i.Price * i.Quantity) ?? 0;
    }


    }
