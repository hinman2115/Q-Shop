using Microsoft.AspNetCore.Mvc;
using QShop.Models.Data;

namespace QShop.Controllers
{
    public class AdminController : Controller
    {
        private readonly QshopContext _Context;

        public AdminController(QshopContext context)
        {
            _Context = context;
        }
        public IActionResult Dashboard()
        {
            ViewBag.Name = HttpContext.Session.GetString("Name");


            return View();
        }

        public IActionResult ManageUsers()
        {
            return View();
        }

        public IActionResult ManageOrders()
        {   

            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult Settings()
        {
            return View();
        }

        public IActionResult Logout()
        {
            // Implement logout logic
            return RedirectToAction("Index", "Home");
        }

    }
}
