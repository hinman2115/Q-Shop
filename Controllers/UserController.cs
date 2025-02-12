using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QShop.Models.Data;
using QShop.Models.Entity;
using QShop.Models.ViewModel;
using System.Data;

namespace QShop.Controllers
{
    public class UserController : Controller
    {
        private readonly QshopContext _context;

        public UserController(QshopContext dbcontect)
        {
            _context = dbcontect;

        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Areas = _context.Areas.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult>Add(AddUserViewModel user, string newArea)
        {
            int areaID;

            if (!string.IsNullOrEmpty(newArea))
            {
                var existingarea = _context.Areas.FirstOrDefault(a => a.AreaName == newArea);
                if (existingarea == null)
                {
                    var area = new Area
                    {
                        AreaName = newArea
                    };
                    await _context.Areas.AddAsync(area);
                    await _context.SaveChangesAsync();
                    areaID = area.AreaId;
                }
                else
                {
                    areaID = existingarea.AreaId;
                }
            }
            else
            {
                areaID = user.AreaId;
            }

            var U = new User
            {
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                PasswordHash = user.Password,
                AreaId = areaID

            };
        

            await _context.Users.AddAsync(U);
            await _context.SaveChangesAsync();
            if (user.Role == "Super Admin" || user.Role == "Admin")
            {
                return RedirectToAction("Login", "User");
            }
           
            return View();

        }

        [HttpGet]
        public IActionResult CustomerAdd() 
        {
            ViewBag.Areas = _context.Areas.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerAdd(CustomerViewModel custom, string newArea)
        {
            int Areid;
            if (!string.IsNullOrEmpty(newArea))
            {

                var existingarea = _context.Areas.FirstOrDefault(a => a.AreaName == newArea);
                if (existingarea == null)
                {
                    var area = new Area
                    {
                        AreaName = newArea
                    };
                    await _context.Areas.AddAsync(area);
                    await _context.SaveChangesAsync();
                    Areid = area.AreaId;
                }
                else
                {
                    Areid = existingarea.AreaId;



                }

            }
            else
            {
                Areid = custom.AreaId;
            }



            var c = new User
            {
                Name = custom.Name,
                Email = custom.Email,
                Phone = custom.Phone,
                Address = custom.Address,
                Role = "Customer",
                PasswordHash = custom.Password,
                AreaId = Areid
            };

            await _context.Users.AddAsync(c);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult>Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Find user by email
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || user.PasswordHash != model.Password)
            {
                ViewData["Error"] = "Invalid Email or Password!";
                return View();
            }

            HttpContext.Session.SetString("UserId", user.UserId.ToString());  // Storing User ID
            HttpContext.Session.SetString("Email", user.Email);               // Storing Email
            HttpContext.Session.SetString("Name", user.Name);                 // Storing Name
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("AreaId", user.AreaId.ToString());

            // Redirect based on role
            if (user.Role == "Super Admin")
            {
                return RedirectToAction("SuperAdd", "SuperAdmin");
            }
            if (user.Role == "Admin")
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

    }
}
