namespace QShop.Models.ViewModel.NewFolder
{
    public class Product
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int AreaId { get; set; }
        public int CategoryId { get; set; }
        public string PImage { get; set; }
    }
}