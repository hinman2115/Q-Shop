using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QShop.Models.viewmodel;
using QShop.Models.ViewModel;
using QShop.Models.ViewModel.NewFolder;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace testapi.Controllers.Customer
{
    public class CustomerProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:44381"; // Change this to match your API

        public CustomerProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            int? userAreaId = HttpContext.Session.GetInt32("AreaId");
            if (userAreaId == null)
            {
                //_logger.LogWarning("User's AreaId is not set in the session.");
                return View("Login", "Login");
            }
            string productApiUrl = $"{_baseUrl}/api/Product/GetProducts/GetProducts?areaId={userAreaId}";

            List<Product> products = new();

            try
            {
                HttpResponseMessage productResponse = await _httpClient.GetAsync(productApiUrl);
                if (productResponse.IsSuccessStatusCode)
                {
                    string jsonData = await productResponse.Content.ReadAsStringAsync();
                    products = JsonConvert.DeserializeObject<List<Product>>(jsonData) ?? new List<Product>();

                    // Debug information
                    Console.WriteLine($"Retrieved {products.Count} products from API");

                    foreach (var product in products)
                    {
                        Console.WriteLine($"Product: {product.ProductName}, Image: {product.PImage}");
                    }
                }
                else
                {
                    Console.WriteLine($"API returned status code: {productResponse.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = "Network error while fetching data: " + ex.Message;
                Console.WriteLine("Network error: " + ex.Message);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Unexpected error: " + ex.Message;
                Console.WriteLine("Unexpected error: " + ex.Message);
            }

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, string productName, decimal price)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var cartItem = new
            {
                ProductId = productId,
                ProductName = productName,
                Price = price
            };

            // Convert data to JSON format
            var jsonContent = new StringContent(JsonConvert.SerializeObject(cartItem), Encoding.UTF8, "application/json");

            // Call the API to add product to the cart
            HttpResponseMessage response = await _httpClient.PostAsync($"{_baseUrl}/api/Cart/AddToCart?userId={userId}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Product added to cart successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to add product to cart." });
            }
        }
        
        public async Task<IActionResult> CartDetails()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var apiUrl = $"{_baseUrl}/api/Cart/GetCart?userId={userId}";
            var response = await _httpClient.GetAsync(apiUrl);

            // Create a new CartItemViewModel to match the view's expectation
            var cartViewModel = new CartItemViewModel
            {
                UserId = userId.Value,
                Items = new List<CartItemViewModel>()
            };

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Failed to load cart items.";
                return View(cartViewModel);
            }

            var jsonData = await response.Content.ReadAsStringAsync();
            Console.WriteLine("API Response: " + jsonData); // Debugging log

            try
            {
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(jsonData);
                if (jsonResponse == null || jsonResponse.success != true || jsonResponse.cartItems == null)
                {
                    TempData["ErrorMessage"] = "Invalid response from server.";
                    return View(cartViewModel);
                }

                // Debugging output to check raw cartItems JSON
                Console.WriteLine("Raw cartItems JSON: " + jsonResponse.cartItems.ToString());

                // Deserialize cart items
                var cartItemsJson = jsonResponse.cartItems.ToString();
                var cartItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartItemsJson);

                // Debugging output to check parsed cart items
                foreach (var item in cartItems)
                {
                    Console.WriteLine($"ProductId: {item.ProductId}, ProductName: {item.ProductName}");
                }

                cartViewModel.Items = cartItems ?? new List<CartItemViewModel>();

                return View(cartViewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error processing cart data.";
                Console.WriteLine("Exception: " + ex.Message);
                return View(cartViewModel);
            }

        }
        //
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart([FromBody] RemoveCartModel model)
        {
            // Validate input
            if (model == null || model.ProductId <= 0)
            {
                return BadRequest(new { message = "Invalid product ID" });
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized(new { message = "User not logged in" });
            }

            try
            {
                // Construct the full API URL with both productId and userId
                var apiUrl = $"{_baseUrl}Cart/RemoveFromCart/RemoveFromCart/{model.ProductId}?userId={userId}";

                var response = await _httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Product removed successfully" });
                }

                // Log the error response
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Remove from cart error: {errorContent}");

                return StatusCode(500, new { message = "Failed to remove product from cart" });
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception in RemoveFromCart: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while removing the product" });
            }
        }
        //
    }
}