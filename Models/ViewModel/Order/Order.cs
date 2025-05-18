namespace QShop.Models.ViewModel.Order
{
public class OrderDTO
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string PaymentStatus { get; set; }
        public List<OrderItemDTO> Items { get; set; }
        public string OrderStatus { get; set; } 
    }

  

public class OrderItemDTO
    {
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }



public class OrderResponse
    {
        public bool Success { get; set; }
        public List<OrderDTO> Data { get; set; }
    }


    public class CustomerOrdersViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public List<OrderDTO> Orders { get; set; }
    }
  

}
