namespace QShop.Models.ViewModel.NewFolder
{
    public class ProductIndexViewModel
    {
        public List<ProductView> Products { get; set; } = new();
        public List<AreaViewModel> Areas { get; set; } = new();
        public List<CategoryViewModel> Categories { get; set; } = new();
    }
}
