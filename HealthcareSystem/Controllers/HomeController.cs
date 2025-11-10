using Microsoft.AspNetCore.Mvc;

namespace HealthcareSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // If logged in, show dashboard based on role
            var role = HttpContext.Session.GetString("Role");
            return role switch
            {
                "Admin" => RedirectToAction("Index", "Admin"),
                "Doctor" => RedirectToAction("Index", "Doctor"),
                "Patient" => RedirectToAction("Index", "Patient"),
                "ClinicStaff" => RedirectToAction("Index", "Staff"),
                _ => View()
            };
        }
    }
}