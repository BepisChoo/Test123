using HealthcareSystem.Services;
using HealthcareSystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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