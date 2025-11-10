using Microsoft.AspNetCore.Mvc;

namespace HealthcareSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            // Check if user is logged in and is Admin
            var userId = HttpContext.Session.GetString("UserId");
            var role = HttpContext.Session.GetString("Role");

            if (string.IsNullOrEmpty(userId) || role != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.FullName = HttpContext.Session.GetString("FullName");
            return View();
        }
    }
}