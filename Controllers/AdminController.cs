using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QShop.Models.Data;
using QShop.Models.ViewModel.NewFolder;

namespace QShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ManageProductController> _logger;
        public AdminController(HttpClient httpClient, ILogger<ManageProductController> logger)
        {

            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<IActionResult> Dashboard()
        {

            List<ProductView> products = new();
            int? userAreaId = HttpContext.Session.GetInt32("AreaId");
            if (userAreaId == null)
            {
                _logger.LogWarning("User's AreaId is not set in the session.");
                return RedirectToAction("Login", "Account"); 
            }
            string apiUrl = $"https://localhost:44381/api/Product/GetProducts/GetProducts?areaId={userAreaId}"; 
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<ProductView>>(jsonData) ?? new List<ProductView>();
                }
                else
                {
                    _logger.LogWarning($"API returned {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching products: {ex.Message}");
            }
            return View(products);

        }

    }

    }

