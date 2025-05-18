using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using QShop.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using QShop.Models.ViewModel.Login;
using QShop.Service;

namespace testapi.Controllers
{
    public class LoginController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LoginController> _logger;
        private readonly IMemoryCache _cache;
        private string api;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public LoginController(IHttpClientFactory httpClientFactory, ILogger<LoginController> logger, IMemoryCache cache)
        {
            _httpClient = httpClientFactory.CreateClient();
      
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            api = UserService.Apiurl;
            _logger = logger;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult AdminRegister() => View();

        [HttpPost]
        public async Task<IActionResult> AdminRegister(RegisterViewModel model) =>
            await RegisterUser(model, "Admin", "Login");

        [HttpGet("CustomerRegister")]
        public async Task<IActionResult> CustomerRegister()
        {
            if (!_cache.TryGetValue("Areas", out List<AreaViewModel> areas))
            {
                var response = await _httpClient.GetAsync($"http://localhost:5115/api/Areas/GetAreas");
                if (response.IsSuccessStatusCode)
                {
                    areas = JsonSerializer.Deserialize<List<AreaViewModel>>(await response.Content.ReadAsStringAsync(), JsonOptions);
                    _cache.Set("Areas", areas ?? new List<AreaViewModel>(), TimeSpan.FromMinutes(10));
                }
                else
                {
                    _logger.LogWarning("Failed to fetch areas. Status: {StatusCode}", response.StatusCode);
                    areas = new List<AreaViewModel>();
                }
            }
            ViewBag.Areas = new SelectList(areas, "AreaId", "AreaName");
            return View(new RegisterViewModel());
        }

        [HttpPost("CustomerRegister")]
        public async Task<IActionResult> CustomerRegister(RegisterViewModel model) =>
            await RegisterUser(model, "Customer", "Login");

        private async Task<IActionResult> RegisterUser(RegisterViewModel model, string role, string redirectAction)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                model.Role = role;
                var content = new StringContent(JsonSerializer.Serialize(model, JsonOptions), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5115/api/User/add", content);

                if (response.IsSuccessStatusCode) return RedirectToAction(redirectAction);

                _logger.LogWarning("Registration failed: {Status}", response.StatusCode);
                ViewBag.ErrorMessage = "Registration failed. Please try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration.");
                ViewBag.ErrorMessage = "An unexpected error occurred.";
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid) return View(loginViewModel);

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(loginViewModel, JsonOptions), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"http://localhost:5115/api/User", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
                    if (result == null)
                    {
                        _logger.LogWarning("API returned a null login response.");
                        ViewBag.ErrorMessage = "Login failed. Please try again.";
                        return View(loginViewModel);
                    }
                    HttpContext.Session.SetInt32("UserId", (int)result.userId); // Store UserId
                    HttpContext.Session.SetInt32("AreaId", (int)result.AreaId); // Store AreaId
                    HttpContext.Session.SetString("UserRole",result.Role);
                    if (result.AreaId.HasValue)
                        HttpContext.Session.SetInt32("AreaId", result.AreaId.Value);

                    return RedirectUserBasedOnRole(result.Role);
                }

                _logger.LogWarning("Login failed: {Status}", response.StatusCode);
                ViewBag.ErrorMessage = "Invalid credentials. Please try again.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login.");
                ViewBag.ErrorMessage = "An error occurred. Please try again.";
            }

            return View(loginViewModel);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private IActionResult RedirectUserBasedOnRole(string role) =>
            role switch
            {
                "Super Admin" => RedirectToAction("SuperAdd", "SuperAdmin"),
                "Admin" => RedirectToAction("Dashboard", "Admin"),
                "Customer" => RedirectToAction("Index", "CustomerProduct"),
                _ => RedirectToAction("Index", "Home")
            };

        public class LoginResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
            public int? AreaId { get; set; }
            public int? userId { get; set; }
        }
    }
}
