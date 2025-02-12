using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QShop.Models.Data;
using QShop.Models.Entity;

namespace QShop.Controllers
{
    public class SuperAdminController : Controller
    {
        private readonly QshopContext _context;

        public SuperAdminController(QshopContext context)
        {
            _context = context;
            
        }
        public IActionResult SuperAdd()
        {
            ViewBag.names =  HttpContext.Session.GetString("Name");
            return View();
        }

        public IActionResult ManageAdmins()
        {
            var admins = _context.Users
    .Where(u => u.Role == "Admin")  // Filter first
    .Select(u => new { u.UserId, u.Name, u.Email })  // Then select required fields
    .ToList();

            return View(admins);
        }
    }
}
