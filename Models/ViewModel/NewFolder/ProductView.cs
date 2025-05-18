using System.ComponentModel.DataAnnotations;

namespace QShop.Models.ViewModel.NewFolder
{
    public class ProductView
    {
        public int ProductId { get; set; }
        public int AreaId { get; set; }
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Product Name is required.")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Stock Quantity is required.")]
        public int StockQuantity { get; set; }

        public string? PImage { get; set; } // To store the image path

        [Display(Name = "Product Image")]
        public IFormFile ImageFile { get; set; } // For file upload

        // Additional Properties
        public string AreaName { get; set; }
        public string CategoryName { get; set; }

        public List<AreaViewModel> Areas { get; set; } = new();
        public List<CategoryViewModel> Categories { get; set; } = new();
    }

}
