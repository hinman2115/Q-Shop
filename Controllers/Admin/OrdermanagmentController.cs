using Microsoft.AspNetCore.Mvc;
using QShop.Models.ViewModel.NewFolder;
using QShop.Models.ViewModel.Order;
using System.Text.Json;

namespace QShop.Controllers.Admin
{
    public class OrdermanagmentController : Controller
    {
        private readonly HttpClient _httpClient;

        public OrdermanagmentController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Order()
        {
            var areaId = HttpContext.Session.GetInt32("AreaId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (areaId == null || userId == null)
                return RedirectToAction("Login", "Account");

            var apiUrl = $"http://localhost:5195/api/Order/GetOrdersByArea?areaId={areaId}&userid={userId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to fetch orders.";
                return View(new List<OrderPlaceViewModel>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            OrderApiResponse apiResponse;
            try
            {
                apiResponse = JsonSerializer.Deserialize<OrderApiResponse>(json, options);
            }
            catch (JsonException ex)
            {
                // Log ex.Message somewhere
                ViewBag.Error = $"Deserialization failed: {ex.Message}";
                return View(new List<OrderPlaceViewModel>());
            }

            return View(apiResponse?.Data ?? new List<OrderPlaceViewModel>());
        }
    }
}
