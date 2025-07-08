using SafeVaultProject.Data;
using SafeVaultProject.Models;
using SafeVaultProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SafeVaultProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly SafeVaultContext _db;
        private readonly IInputSanitizer _sanitizer;

        public AccountController(SafeVaultContext db, IInputSanitizer sanitizer)
        {
            _db = db;
            _sanitizer = sanitizer;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string username, string email, string password)
        {
            username = _sanitizer.Sanitize(username);
            email = _sanitizer.Sanitize(email);

            if (_db.Users.Any(u => u.Username == username))
            {
                ModelState.AddModelError("", "Username already taken");
                return View();
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            _db.Users.Add(new User
            {
                Username = username,
                Email = email,
                PasswordHash = hash,
                Role = "User"
            });
            _db.SaveChanges();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            username = _sanitizer.Sanitize(username);
            var user = _db.Users.SingleOrDefault(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid credentials");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var id = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(id)
            );

            return RedirectToAction("Index", "Admin");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
