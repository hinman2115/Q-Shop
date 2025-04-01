using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using QShop.Models.ViewModel;
using QShop.Models.ViewModel.NewFolder;

public class ManageProductController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl = "https://localhost:44371/"; // Change this to match your API

    public ManageProductController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IActionResult> Index()
    {
        string productApiUrl = $"{_baseUrl}/api/Product/GetProducts";
        string areaApiUrl = $"{_baseUrl}/api/Areas/GetAreas";
        string categoryApiUrl = $"{_baseUrl}/api/Categories/GetCategories";

        List<ProductView> products = new();
        List<AreaViewModel> areas = new();
        List<CategoryViewModel> categories = new();

        try
        {
            // Fetch data in parallel for better performance
            var productTask = _httpClient.GetAsync(productApiUrl);
            var areaTask = _httpClient.GetAsync(areaApiUrl);
            var categoryTask = _httpClient.GetAsync(categoryApiUrl);

            await Task.WhenAll(productTask, areaTask, categoryTask);

            // Process Product Data
            HttpResponseMessage productResponse = await productTask;
            if (productResponse.IsSuccessStatusCode)
            {
                string jsonData = await productResponse.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<ProductView>>(jsonData) ?? new List<ProductView>();
                foreach (var product in products)
                {
                    if (!string.IsNullOrEmpty(product.PImage))
                    {
                        product.PImage = "/images/" + Path.GetFileName(product.PImage);
                    }
                }
            }

            // Process Area Data
            HttpResponseMessage areaResponse = await areaTask;
            if (areaResponse.IsSuccessStatusCode)
            {
                string jsonData = await areaResponse.Content.ReadAsStringAsync();
                areas = JsonConvert.DeserializeObject<List<AreaViewModel>>(jsonData) ?? new List<AreaViewModel>();
            }

            // Process Category Data
            HttpResponseMessage categoryResponse = await categoryTask;
            if (categoryResponse.IsSuccessStatusCode)
            {
                string jsonData = await categoryResponse.Content.ReadAsStringAsync();
                categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(jsonData) ?? new List<CategoryViewModel>();
            }

            // Match Area and Category by ID
            foreach (var product in products)
            {
                product.AreaName = areas.Find(a => a.AreaId == product.AreaId)?.AreaName ?? "Unknown";
                product.CategoryName = categories.Find(c => c.CategoryId == product.CategoryId)?.CategoryName ?? "Unknown";
            }
        }
        catch (HttpRequestException ex)
        {
            ViewBag.ErrorMessage = "Network error while fetching data: " + ex.Message;
        }
        catch (Exception ex)
        {
            ViewBag.ErrorMessage = "Unexpected error: " + ex.Message;
        }

        return View(products);
    }
}