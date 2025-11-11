using HealthcareSystem.Services;
using HealthcareSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;          
using Microsoft.AspNetCore.Authentication.Cookies;    
using System.Security.Claims;

namespace HealthcareSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.RegisterAsync(model);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.LoginAsync(model);

            if (result.Success && result.User != null)
            {
                // ----------------------------------------------------
                // ✅ AUTHENTICATION FIX: Create Claims and Sign In
                // ----------------------------------------------------

                // 1. Create a list of claims (user data for the security ticket)
                var claims = new List<Claim>
                {
                    // ClaimTypes.NameIdentifier is the standard way to store the UserId
                    new Claim(ClaimTypes.NameIdentifier, result.User.UserId.ToString()), 
                    // ClaimTypes.Name is the username
                    new Claim(ClaimTypes.Name, result.User.Username),
                    // ClaimTypes.Role is the user's role
                    new Claim(ClaimTypes.Role, result.User.Role)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    // This tells the cookie how long it should last
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                };

                // THIS IS THE CRITICAL LINE: It creates the security cookie
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);


                // ----------------------------------------------------
                // Existing Session Logic (Preserved)
                // ----------------------------------------------------
                // Store user info in session
                HttpContext.Session.SetString("UserId", result.User.UserId.ToString());
                HttpContext.Session.SetString("Username", result.User.Username);
                HttpContext.Session.SetString("Role", result.User.Role);
                HttpContext.Session.SetString("FullName", $"{result.User.FirstName} {result.User.LastName}");


                // Redirect based on role
                return result.User.Role switch
                {
                    "Admin" => RedirectToAction("Index", "Admin"),
                    "Doctor" => RedirectToAction("Index", "Doctor"),
                    "Patient" => RedirectToAction("Index", "Patient"),
                    "ClinicStaff" => RedirectToAction("Index", "Staff"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}