using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace QShop.Models.ViewModel.NewFolder
{
    public class ProductUpdate
    {
        public int ProductId { get; set; }
        [Required]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string ProductName { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }
    }
}
