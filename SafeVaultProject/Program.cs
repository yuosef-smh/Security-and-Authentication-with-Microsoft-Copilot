using SafeVaultProject.Data;
using SafeVaultProject.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// In-memory EF Core
builder.Services.AddDbContext<SafeVaultContext>(
    opts => opts.UseInMemoryDatabase("SafeVaultDB"));

// Input sanitization
builder.Services.AddSingleton<IInputSanitizer, InputSanitizer>();

// Cookie-based auth & RBAC policy
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => options.LoginPath = "/Account/Login");

builder.Services.AddAuthorization(options =>
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Default route → Login page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


using(var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SafeVaultContext>();
    if (!db.Users.Any(u => u.Role == "Admin"))
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("Admin123!");
        db.Users.Add(new User {
            Username     = "admin",
            Email        = "admin@safevault.com",
            PasswordHash = hash,
            Role         = "Admin"
        });
        db.SaveChanges();
    }
}


app.Run();
