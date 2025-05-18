using Microsoft.EntityFrameworkCore;
using QShop.Models.Data;
using QShop.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database connection
builder.Services.AddDbContext<QshopContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache(); // Required for session storage
builder.Services.AddHttpClient();  // ✅ Add this line to fix the error
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(300); // Set session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Build application
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Moved here
app.UseAuthentication(); // Ensure authentication works
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
