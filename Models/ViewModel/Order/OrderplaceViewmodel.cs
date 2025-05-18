using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QShop.Models.ViewModel.Order
{
    public class OrderPlaceViewModel
    {
        [Required(ErrorMessage = "Area ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Area ID must be positive")]
        public int AreaId { get; set; }

        public int OrderId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be positive")]
        public int UserId { get; set; }

        public string? UserName { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string PaymentStatus { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<OrderItemViewModel> Items { get; set; } = new List<OrderItemViewModel>();
    }

    public class OrderItemViewModel
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal Price { get; set; }

        public decimal Total => Price * Quantity;

        [Required(ErrorMessage = "Product image is required")]
        public string? ProductImage { get; set; } 
    }

    public class CartItemViewModel
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        // Changed from FormFile to string to store image path/URL
        [Required(ErrorMessage = "Product image is required")]
        public string PImage { get; set; } = string.Empty;

        public decimal TotalPrice => Price * Quantity;
    }
    public class ShoppingCartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        public decimal TotalPrice => Items.Sum(item => item.TotalPrice);
    }

    public class OrderItemRequest
    {
        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be positive")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "User ID must be positive")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Area ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Area ID must be positive")]
        public int AreaId { get; set; }

        [Required(ErrorMessage = "Payment method is required")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required(ErrorMessage = "Order items are required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<OrderItemRequest> Items { get; set; } = new List<OrderItemRequest>();
    }

    public class CartApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
    }
    public class OrderApiResponse
    {
        public bool Success { get; set; }
        public List<OrderPlaceViewModel> Data { get; set; }
        public string Message { get; set; }
        public string Errors { get; set; }
    }

    public class VerifyPaymentRequest
    {
        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Razorpay Payment ID is required")]
        public string RazorpayPaymentId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Razorpay Order ID is required")]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Razorpay Signature is required")]
        public string RazorpaySignature { get; set; } = string.Empty;
    }

    public class RemoveCartModel
    {
        [Required(ErrorMessage = "Product ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Product ID must be positive")]
        public int ProductId { get; set; }
    }
}