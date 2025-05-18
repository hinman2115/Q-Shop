using System.ComponentModel.DataAnnotations;

namespace QShop.Models.ViewModel.NewFolder
{
    public class Productadd
    {

        [Required]
        public string ProductName { get; set; }

        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } = 0;

        [Required]
        public int AreaId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public IFormFile? PImage { get; set; }
    }

}
