using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.IO;
using System.Net.Http.Headers;
using System.Globalization;
using QShop.Models.ViewModel.NewFolder;
using QShop.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QShop.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ManageProductController> _logger;
        private const string BaseApiUrl = "http://localhost:5115/api/Product";
        private readonly IHttpClientFactory _clientFactory;
        public AdminController(HttpClient httpClient, ILogger<ManageProductController> logger, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                // 1. Validate user session
                int? userAreaId = GetUserAreaId();
                if (userAreaId == null)
                {
                    _logger.LogWarning("User's AreaId is not set in the session.");
                    return RedirectToLogin();
                }

                // 2. Fetch products and categories in parallel to improve performance
                var productsTask = FetchData<List<ProductView>>($"{BaseApiUrl}/GetProducts/GetProducts?areaId={userAreaId}");
                var categoriesTask = FetchData<List<CategoryViewModel>>("http://localhost:5115/api/Categories/GetCategories");

                await Task.WhenAll(productsTask, categoriesTask);

                var products = await productsTask;
                var categories = await categoriesTask;

                // 3. Handle null responses
                if (categories == null)
                {
                    _logger.LogError("Failed to fetch categories from API");
                    categories = new List<CategoryViewModel>();
                }

                // 4. Prepare ViewBag data
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
              
                // 5. Return view with proper data
                return View(products ?? new List<ProductView>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in Dashboard method");

                // Return empty collections to prevent view errors
                ViewBag.Categories = new List<SelectListItem>();
                return View(new List<ProductView>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddProduct()
        {
            try
            {
                string categoriesApiUrl = "http://localhost:5115/api/Categories/GetCategories";
                var categories = await FetchData<List<CategoryViewModel>>(categoriesApiUrl);

                if (categories == null)
                {
                    // Handle case when API returns no data
                    ViewBag.Categories = new List<SelectListItem>();
                }
                else
                {
                    ViewBag.Categories = categories.Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.CategoryName
                    }).ToList();
                }

                return View(new Productadd());
            }
            catch (Exception ex)
            {
                // Log the error
                // For now, return empty categories
                ViewBag.Categories = new List<SelectListItem>();
                return View(new Productadd());
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(Productadd model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var client = _clientFactory.CreateClient("ProductApi");
            var sessionAreaId = HttpContext.Session.GetInt32("AreaId");
            if (sessionAreaId == null)
            {
                ModelState.AddModelError("", "Area information is missing. Please try again.");
                return View(model);
            }

            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(model.ProductName), "ProductName");
                content.Add(new StringContent(model.Price.ToString(CultureInfo.InvariantCulture)), "Price");
                content.Add(new StringContent(model.StockQuantity.ToString()), "StockQuantity");
                content.Add(new StringContent(sessionAreaId.Value.ToString()), "AreaId");
                content.Add(new StringContent(model.CategoryId.ToString()), "CategoryId");

                if (model.PImage != null && model.PImage.Length > 0)
                {
                    var fileContent = new StreamContent(model.PImage.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.PImage.ContentType);
                    content.Add(fileContent, "PImage", model.PImage.FileName);
                }
                try
                {

                    var response = await client.PostAsync($"{BaseApiUrl}/PostProduct", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var result = await response.Content.ReadFromJsonAsync<ProductView>();

                        TempData["SuccessMessage"] = "Product created successfully!";
                        return RedirectToAction("Index","Admin");
                    }
                    else
                    {

                        var errorContent = await response.Content.ReadAsStringAsync();
                        ModelState.AddModelError("", $"API Error: {errorContent}");
                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error communicating with API: {ex.Message}");
                    return View(model);
                }
            }


        }


        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                // Define the API URL for the delete request
                string apiUrl = $"{BaseApiUrl}/DeleteProduct/{id}";

                // Call the API to delete the product
                var response = await _httpClient.DeleteAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"API returned {response.StatusCode} when deleting product {id}");
                    return Json(new { success = false, message = "Error deleting the product." });
                }

                // Successfully deleted product
                return Json(new { success = true, message = "Product deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception when deleting product {id}");
                return Json(new { success = false, message = "An error occurred while deleting the product." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            try
            {
                // 1. Fetch the product data /api/Product/GetProductById/{id}
                string productApiUrl = $"http://localhost:5115/api/Product/GetProductById/{id}";
                var product = await FetchData<ProductUpdate>(productApiUrl);
                if (product == null)
                {
                    return NotFound();
                }

                // 2. Fetch categories list
                string categoriesApiUrl = $"http://localhost:5115/api/Categories/GetCategories"; // Adjust the endpoint as needed
                var categories = await FetchData<List<CategoryViewModel>>(categoriesApiUrl);

                // 3. Pass both the product and categories to the view using ViewBag
                ViewBag.Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                }).ToList();
                return View(product);
            }
            catch (Exception ex)
            {
                // Handle exception (log it, etc.)
                ModelState.AddModelError(string.Empty, "Error loading product data: " + ex.Message);
                return View(new ProductUpdate()); // Return empty model with error
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(int id, ProductUpdate product)
        {
            if (!ModelState.IsValid)
            {
                return View(product);
            }

            // Create a DTO to match the API's expected input
            var productUpdateDto = new ProductUpdate
            {
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                CategoryId = product.CategoryId
            };

            // Call the API to update the product
            string apiUrl = $"http://localhost:5195/api/Product/PutProduct/{id}";
            var json = JsonConvert.SerializeObject(productUpdateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                _logger.LogWarning($"API returned {response.StatusCode} - {response.ReasonPhrase}");
                ModelState.AddModelError(string.Empty, "Error updating product. Please try again.");
                return View(product);
            }
        }
        //
        private int? GetUserAreaId()
        {
            return HttpContext.Session.GetInt32("AreaId") ?? 1; // Default to 1 for testing if not set
        }
        private IActionResult RedirectToLogin()
        {
            return RedirectToAction("Login", "Account");
        }
        private async Task<T> FetchData<T>(string apiUrl) where T : new()
        {
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(jsonData) ?? new T();
                }
                else
                {
                    _logger.LogWarning($"API returned {response.StatusCode} - {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching data: {ex.Message}");
            }

            return new T();
        }
        //

        //
    }

}

