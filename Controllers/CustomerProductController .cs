using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QShop.Models.ViewModel;
using QShop.Models.ViewModel.NewFolder;
using QShop.Models.ViewModel.Order;
using QShop.Service;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QShop.Controllers.Customer
{
    public class CustomerProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private string api;

        public CustomerProductController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            api = UserService.Apiurl;
        }

        public async Task<IActionResult> Index()
        {
            int? userAreaId = HttpContext.Session.GetInt32("AreaId");
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userAreaId == null&& userId == null)
            {
                //_logger.LogWarning("User's AreaId is not set in the session.");
                return View("Login", "Login");
            }
            string productApiUrl = $"http://localhost:5115/api/Product/GetProducts/GetProducts?areaId={userAreaId}";

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
            HttpResponseMessage response = await _httpClient.PostAsync($"http://localhost:5115/api/Cart/AddToCart?userId={userId}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Product added to cart successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Failed to add product to cart." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CartDetails()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var response = await _httpClient.GetAsync($"http://localhost:5115/api/Cart/GetCart?userId={userId}");
            ShoppingCartViewModel cartViewModel = new ShoppingCartViewModel();

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonConvert.DeserializeObject<CartApiResponse>(jsonData);
                if (jsonResponse?.Success == true && jsonResponse.CartItems != null)
                {
                    cartViewModel.Items = jsonResponse.CartItems;
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid response from server.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to load cart items.";
            }

            return View(cartViewModel);

        }
        
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

          
                var apiUrl = $"http://localhost:5115/api/Cart/RemoveFromCart/{model.ProductId}?userId={userId}";

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

        [HttpGet]
        public async Task<IActionResult> PlaceOrder()
        {
            var areaId = HttpContext.Session.GetInt32("AreaId");
            var userId = HttpContext.Session.GetInt32("UserId");
            if (areaId == null || userId == null)
            {
                TempData["Error"] = "Please log in or select an area.";
                return RedirectToAction("Login", "Account");
            }

            var orderViewModel = new OrderPlaceViewModel
            {
                AreaId = areaId.Value,
                UserId = userId.Value,
                Items = new List<OrderItemViewModel>()
            };

            var apiUrl = $"http://localhost:5115/api/Cart/GetCart?userId={userId}";
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {json}");
                var apiResponse = JsonConvert.DeserializeObject<CartApiResponse>(json);

                if (apiResponse != null && apiResponse.Success && apiResponse.CartItems != null)
                {
                    orderViewModel.Items = apiResponse.CartItems.Select(item => new OrderItemViewModel
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        ProductName = item.ProductName,
                        Price = item.Price,
                        //ProductImage = item.ProductImage
                    }).ToList();
                }
            }
            else
            {
                TempData["Error"] = "Failed to load cart items.";
            }

            return View(orderViewModel);

        }

        [HttpPost]
        public async Task<IActionResult> Placeorder(OrderPlaceViewModel modes)
        {


            var areaId = HttpContext.Session.GetInt32("AreaId");
            var userId = HttpContext.Session.GetInt32("UserId");


            if (!userId.HasValue || !areaId.HasValue)
            {
                TempData["Error"] = "Please log in or select an area to place an order.";
                return RedirectToAction("PlaceOrder");
            }

            if (modes.PaymentMethod != "COD")
            {
                TempData["Error"] = "Only Cash on Delivery (COD) is supported currently.";
                return RedirectToAction("PlaceOrder");
            }

            var items = modes.Items?.Select(item => new OrderItemRequest
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList() ?? new List<OrderItemRequest>();

            if (!items.Any())
            {
                TempData["Error"] = "Your cart is empty. Please add items to your order.";
                return RedirectToAction("PlaceOrder");
            }

            var request = new CreateOrderRequest
            {
                UserId = userId.Value,
                AreaId = areaId.Value,
                PaymentMethod = "COD",
                Items = items
            };

            try
            {
                var apiUrl = "http://localhost:5115/api/Order/PlaceOrder";
                var json = JsonConvert.SerializeObject(request);
                Console.WriteLine($"Request: {json}");
                var response = await _httpClient.PostAsJsonAsync(apiUrl, request);

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    if (apiResponse != null && apiResponse.Success)
                    {
                        TempData["Success"] = apiResponse.Message ?? "Order placed successfully";
                        var orderId = apiResponse.Data; // Access OrderId directly
                        return RedirectToAction("Index","CustomerProduct");
                    }
                    else
                    {
                        TempData["Error"] = $"Failed to place order: {apiResponse?.Message}";
                        return RedirectToAction("PlaceOrder");
                    }
                   
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Failed to place order: {errorContent}";
                    return RedirectToAction("PlaceOrder");
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred while placing the order: {ex.Message}";
                return RedirectToAction("PlaceOrder");
            }
        }

        [HttpGet]
        public IActionResult OrderSuccess(int orderId)
        {
            ViewBag.OrderId = orderId;
            return View();
        }


        [HttpGet]
        public async Task<ActionResult<List<OrderDTO>>> UserOrders(int? id)
        {

            var customerId = HttpContext.Session.GetInt32("UserId");
            if (!id.HasValue && customerId.HasValue)
            {
                id = customerId;
            }

            if (!id.HasValue)
            {
                return BadRequest("User  ID is not provided.");
            }

            try
            {
                string url = $"http://localhost:5115/api/Order/GetOrders?userId={id}";
                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response Content: {content}"); // Log the response content

                    // Deserialize the JSON content into the OrderResponse wrapper class
                    var orderResponse = JsonConvert.DeserializeObject<OrderResponse>(content);

                    // Check if the response indicates success and return the data
                    if (orderResponse != null && orderResponse.Success)
                    {
                        return View(orderResponse.Data); // Return the list of orders
                    }
                    else
                    {
                        return BadRequest("Failed to fetch orders. Response indicates failure.");
                    }
                }

                // Handle error cases
                return BadRequest($"Failed to fetch orders. Status Code: {response.StatusCode}");
            }
            catch (HttpRequestException ex)
            {
                // Log network-related exceptions
                Console.WriteLine($"Network error while fetching orders: {ex.Message}");
                return StatusCode(500, "Network error occurred while fetching orders.");
            }
            catch (Exception ex)
            {
                // Log any other exceptions
                Console.WriteLine($"Error fetching orders: {ex.Message}");
                return StatusCode(500, "An error occurred while fetching orders.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Cancelorder(int OrderID)
        {
            try
            {
                var api = $"http://localhost:5115/api/Order/CancelOrder?orderId={OrderID}";
                var response = await _httpClient.DeleteAsync(api);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    return Ok(result);
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to cancel order",
                        Errors = new List<string> { error }
                    });
                }
            }

            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while cancelling the order",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
